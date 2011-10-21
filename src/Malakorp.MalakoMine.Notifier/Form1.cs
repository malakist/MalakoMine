using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Malakorp.MalakoMine.TFSProxy;

namespace Malakorp.MalakoMine.Notifier
{
    public partial class Form1 : Form
    {
        private Proxy proxy = null;
        private System.Windows.Forms.Timer timer = null;
        private DateTime dataUltimaAnalise = DateTime.MinValue; //TODO: isto deveria poder ser gravado e recuperado de um arquivo
        NotifyIcon notIcon = null;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Notifications(string texto, bool playSound = true)
        {
            if (playSound) PlaySound();
            ShowBaloonText(texto);
        }

        private void ShowBaloonText(string texto)
        {
            notIcon.ShowBalloonTip(4000, "Malako Notifier", texto, ToolTipIcon.Info);
        }

        private void PlaySound()
        {
            ThreadPool.QueueUserWorkItem((o) => { PlayMarioSong(); });
        }

        private void PlayMarioSong()
        {
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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 10000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Enabled = true;
            
            notIcon = new NotifyIcon();
            notIcon.Icon = new System.Drawing.Icon(this.Icon, 16, 16);
            notIcon.Visible = true;

            Notifications("O Notificador Malako está em funcionamento", false);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Notifications("Tem trabalho pra você. Consulte o TFS ou o MalakoMine para maiores informações");            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            notIcon.Visible = false;
        }
    }
}
