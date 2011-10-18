using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Malako.TFSProxy;

namespace MalakoNotifier
{
    public partial class Form1 : Form
    {
        private Malako.TFSProxy.Proxy proxy = null;
        private System.Windows.Forms.Timer timer = null;
        private DateTime dataUltimaAnalise = DateTime.MinValue; //TODO: isto deveria poder ser gravado e recuperado de um arquivo
        NotifyIcon notIcon = null;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void ShowNotifications()
        {
            // PlaySound(750, 300);
            // EventLog.WriteEntry("Numero retornado: " + MalakoNumber().ToString());
            Console.Beep(659, 125);
            Console.Beep(659, 125);
            Thread.Sleep(125);
            Console.Beep(659, 125);
            Thread.Sleep(167);
            Console.Beep(523, 125);
            Console.Beep(659, 125);
            Thread.Sleep(125);
            Console.Beep(784, 125);
            Thread.Sleep(375);
            Console.Beep(392, 125);
            Thread.Sleep(375);
            Console.Beep(523, 125);
            Thread.Sleep(250);
            Console.Beep(392, 125);
            Thread.Sleep(250);
            Console.Beep(330, 125);
            Thread.Sleep(250);
            Console.Beep(440, 125);
            Thread.Sleep(125);
            Console.Beep(494, 125);
            Thread.Sleep(125);
            Console.Beep(466, 125);
            Thread.Sleep(42);
            Console.Beep(440, 125);
            Thread.Sleep(125);
            Console.Beep(392, 125);
            Thread.Sleep(125);
            Console.Beep(659, 125);
            Thread.Sleep(125);
            Console.Beep(784, 125);
            Thread.Sleep(125);
            Console.Beep(880, 125);
            Thread.Sleep(125);
            Console.Beep(698, 125);
            Console.Beep(784, 125);
            Thread.Sleep(125);
            Console.Beep(659, 125);
            Thread.Sleep(125);
            Console.Beep(523, 125);
            Thread.Sleep(125);
            Console.Beep(587, 125);
            Console.Beep(494, 125);
            Thread.Sleep(125);
            Console.Beep(523, 125);
            Thread.Sleep(250);
            Console.Beep(392, 125);
            Thread.Sleep(250);
            Console.Beep(330, 125);
            Thread.Sleep(250);
            Console.Beep(440, 125);
            Thread.Sleep(125);
            Console.Beep(494, 125);
            Thread.Sleep(125);
            Console.Beep(466, 125);
            Thread.Sleep(42);
            Console.Beep(440, 125);
            Thread.Sleep(125);
            Console.Beep(392, 125);
            Thread.Sleep(125);
            Console.Beep(659, 125);
            Thread.Sleep(125);
            Console.Beep(784, 125);
            Thread.Sleep(125);
            Console.Beep(880, 125);
            Thread.Sleep(125);
            Console.Beep(698, 125);
            Console.Beep(784, 125);
            Thread.Sleep(125);
            Console.Beep(659, 125);
            Thread.Sleep(125);
            Console.Beep(523, 125);
            Thread.Sleep(125);
            Console.Beep(587, 125);
            Console.Beep(494, 125);
            Thread.Sleep(375);
            Console.Beep(784, 125);
            Console.Beep(740, 125);
            Console.Beep(698, 125);
            Thread.Sleep(42);
            Console.Beep(622, 125);
            Thread.Sleep(125);
            Console.Beep(659, 125);
            Thread.Sleep(167);
            Console.Beep(415, 125);
            Console.Beep(440, 125);
            Console.Beep(523, 125);
            Thread.Sleep(125);
            Console.Beep(440, 125);
            Console.Beep(523, 125);
            Console.Beep(587, 125);
            Thread.Sleep(250);
            Console.Beep(784, 125);
            Console.Beep(740, 125);
            Console.Beep(698, 125);
            Thread.Sleep(42);
            Console.Beep(622, 125);
            Thread.Sleep(125);
            Console.Beep(659, 125);
            Thread.Sleep(167);
            Console.Beep(698, 125);
            Thread.Sleep(125);
            Console.Beep(698, 125);
            Console.Beep(698, 125);
            Thread.Sleep(625);
            Console.Beep(784, 125);
            Console.Beep(740, 125);
            Console.Beep(698, 125);
            Thread.Sleep(42);
            Console.Beep(622, 125);
            Thread.Sleep(125);
            Console.Beep(659, 125);
            Thread.Sleep(167);
            Console.Beep(415, 125);
            Console.Beep(440, 125);
            Console.Beep(523, 125);
            Thread.Sleep(125);
            Console.Beep(440, 125);
            Console.Beep(523, 125);
            Console.Beep(587, 125);
            Thread.Sleep(250);
            Console.Beep(622, 125);
            Thread.Sleep(250);
            Console.Beep(587, 125);
            Thread.Sleep(250);
            Console.Beep(523, 125);
            Thread.Sleep(1125);
            Console.Beep(784, 125);
            Console.Beep(740, 125);
            Console.Beep(698, 125);
            Thread.Sleep(42);
            Console.Beep(622, 125);
            Thread.Sleep(125);
            Console.Beep(659, 125);
            Thread.Sleep(167);
            Console.Beep(415, 125);
            Console.Beep(440, 125);
            Console.Beep(523, 125);
            Thread.Sleep(125);
            Console.Beep(440, 125);
            Console.Beep(523, 125);
            Console.Beep(587, 125);
            Thread.Sleep(250);
            Console.Beep(784, 125);
            Console.Beep(740, 125);
            Console.Beep(698, 125);
            Thread.Sleep(42);
            Console.Beep(622, 125);
            Thread.Sleep(125);
            Console.Beep(659, 125);
            Thread.Sleep(167);
            Console.Beep(698, 125);
            Thread.Sleep(125);
            Console.Beep(698, 125);
            Console.Beep(698, 125);
            Thread.Sleep(625);
            Console.Beep(784, 125);
            Console.Beep(740, 125);
            Console.Beep(698, 125);
            Thread.Sleep(42);
            Console.Beep(622, 125);
            Thread.Sleep(125);
            Console.Beep(659, 125);
            Thread.Sleep(167);
            Console.Beep(415, 125);
            Console.Beep(440, 125);
            Console.Beep(523, 125);
            Thread.Sleep(125);
            Console.Beep(440, 125);
            Console.Beep(523, 125);
            Console.Beep(587, 125);
            Thread.Sleep(250);
            Console.Beep(622, 125);
            Thread.Sleep(250);
            Console.Beep(587, 125);
            Thread.Sleep(250);
            Console.Beep(523, 125);
            Thread.Sleep(625);
        }


        private void ShowNotifyIcon()
        {
            notIcon = new NotifyIcon();
            notIcon.Icon = new System.Drawing.Icon(this.Icon, 16, 16);
            notIcon.Visible = true;
            notIcon.ShowBalloonTip(4000, "Title", "Texto", ToolTipIcon.Info);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 10000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Enabled = true;
            ShowNotifyIcon();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            //var list = from task in proxy.GetTasks()
            //           where task.DataDigitacao > this.dataUltimaAnalise
            //           select task;

            //if (list.Count<Task>() > 0)
            //{
            //    ShowNotifications();
            //}

            //dataUltimaAnalise = DateTime.Now;

            ShowNotifications();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            notIcon.Visible = false;
        }
    }
}
