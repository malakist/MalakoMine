using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Malakorp.MalakoMine.TFS;
using System.Net;

namespace Malakorp.MalakoMine.Notifier
{
    public partial class Form1 : Form
    {
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
#if PLAY_SOUND
            ThreadPool.QueueUserWorkItem((o) => { PlayMarioSong(); });
#endif
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
            try
            {
                TFS.TFSMalako malako = new TFS.TFSMalako();

                malako.ServerName = "http://itgvs17:8080/tfs/defaultcollection";
                malako.ProjectName = "Boletos";
                // malako.Credentials = System.Security.Principal.WindowsIdentity.GetCurrent().;
                malako.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

                //Uri uri = new Uri("http://tempuri.org/");
                //ICredentials credentials = CredentialCache.DefaultCredentials;
                //NetworkCredential credential = credentials.GetCredential(uri, "Basic");

                malako.Connect();
                if (malako.ThereAreNewTasksSince(DateTime.Today))
                    Notifications("Tem trabalho pra você. Consulte o TFS ou o MalakoMine para maiores informações");                
            }
            catch
            {
                //TODO: não, não vai ficar desse jeito, não vou engolir a exception... Não sou tão coxinha assim
                Notifications("Ocorreu um erro durante a conexão com o TFS", false);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            notIcon.Visible = false;
        }
    }
}
