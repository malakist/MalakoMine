using System;
using System.Timers;
using System.Linq;
using System.Threading;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using Malako.TFSProxy;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;

public class MalakoService : ServiceBase
{
    private Malako.TFSProxy.Proxy proxy = null;
    private System.Timers.Timer timer = null;
    private DateTime dataUltimaAnalise = DateTime.MinValue; //TODO: isto deveria poder ser gravado e recuperado de um arquivo


#if __malakosnd
    [DllImport("kernel32.dll", EntryPoint = "Beep")]
    private static extern int PlaySound(int freq, int duration);
#else
    private static void PlaySound(int freq, int duration) { System.Console.Beep(freq, duration); }
#endif
    
    private void CleanObj(object o) 
    {
        if (o != null && !(o is System.ValueType))
        {
            IDisposable d = (o as IDisposable);
            if (d != null) d.Dispose();
            o = null;
        }
    }

    void timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        var list = from task in proxy.GetTasks()
                   where task.DataDigitacao > this.dataUltimaAnalise
                   select task;
        
        if (list.Count<Task>() > 0) {
            VerificarNotifier();
        }

        dataUltimaAnalise = e.SignalTime;
    }

    private void VerificarNotifier()
    {
        Process[] proc = Process.GetProcessesByName("mlknotifier");
        if (proc.Length == 0)
        {
            ProcessStartInfo psi = new ProcessStartInfo(@"c:\temp\mlknotifier.exe");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = true;
            Process malakoProc = Process.Start(psi);
        }

    }    

    public MalakoService()
    {
#if DEBUG
        Debugger.Break();
#endif

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

            timer = new System.Timers.Timer();
            timer.AutoReset = true;
            timer.Interval = 10000;
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