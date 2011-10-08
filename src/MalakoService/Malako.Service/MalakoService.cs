using System;
using System.Timers;
using System.Linq;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using Malako.TFSProxy;
using System.Diagnostics;

public class MalakoService : ServiceBase
{
    private Malako.TFSProxy.Proxy proxy = null;
    private System.Timers.Timer timer = null;
    private DateTime dataUltimaAnalise = DateTime.MinValue; //TODO: isto deveria poder ser gravado e recuperado de um arquivo

    [DllImport("malakosnd.dll", EntryPoint = "MalakoSound")]
    private static extern void PlaySound();
    
    private void CleanObj(object o) 
    {
        if (o != null && !(o is System.ValueType))
        {
            IDisposable d = (o as IDisposable);
            if (d != null) d.Dispose();
            o = null;
        }
    }

    private void ShowNotifications()
    {
        PlaySound();
        // EventLog.WriteEntry("Numero retornado: " + MalakoNumber().ToString());
    }

    void timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        var list = from task in proxy.GetTasks()
                   where task.DataDigitacao > this.dataUltimaAnalise
                   select task;
        
        if (list.Count<Task>() > 0) {
            ShowNotifications();
        }

        dataUltimaAnalise = e.SignalTime;
    }    

    public MalakoService()
    {
        // Debugger.Break();

        ServiceName = "MalakoService";
        CanStop = true;
        CanShutdown = true;
        CanPauseAndContinue = true;
        AutoLog = true;
    }

    protected override void OnStart(string[] args)
    {
        base.OnStart(args);

        CleanObj(proxy);
        CleanObj(timer);

        try
        {
            proxy = new Proxy();
            proxy.Inicializar();

            timer = new Timer();
            timer.AutoReset = true;
            timer.Interval = 1000;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }
        catch (Exception ex)
        {
            EventLog.WriteEntry("Erro ao inicializar a comunicação com o TFS: " + ex.ToString());

            CleanObj(proxy);
            CleanObj(timer);

            Stop();
        }
    }

    protected override void OnStop()
    {
        CleanObj(proxy);
        CleanObj(timer);
    }
}