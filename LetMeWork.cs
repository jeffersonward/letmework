using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace LetMeWork
{
    public partial class LetMeWork : Form
    {
        private const string StopText = "&Stop";
        private const string StartText = "&Start";

        private Thread _thread;
        private bool _running;

        public LetMeWork()
        {
            InitializeComponent();

            notifyIcon.Icon = SystemIcons.Shield;
            notifyIcon.ContextMenuStrip = contextMenuStrip;

            startStopToolStripMenuItem.Text = StartText;
            _running = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false;
            base.OnLoad(e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stop();
            Application.Exit();
        }

        private void killItToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_running)
            {
                _thread.Interrupt();
            }
            else
            {
                KillIt();
            }
        }

        private void startStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_running)
            {
                Stop();
            }
            else
            {
                Start();
            }
        }

        private void Start()
        {
            _running = true;
            _thread = new Thread(Run);
            _thread.Start();
            startStopToolStripMenuItem.Text = StopText;
        }

        private void Stop()
        {
            if (!_running) return;

            _running = false;
            _thread.Interrupt();
            _thread.Abort();
            startStopToolStripMenuItem.Text = StartText;
        }

        private void Run()
        {
            const int tenSeconds = 10 * 1000;
            const int increment = tenSeconds;
            const int oneMinute = 60 * 1000;
            const int fiveMinutes = 5 * 60 * 1000;

            var sleep = tenSeconds;
            while (_running)
            {
                var justKilledIt = KillIt();

                if (justKilledIt)
                {
                    sleep = tenSeconds;
                }
                else
                {
                    sleep = sleep + increment;
                    if (sleep > oneMinute)
                    {
                        sleep = fiveMinutes;
                    }
                }

                try
                {
                    Thread.Sleep(sleep);
                }
                catch (ThreadInterruptedException)
                {
                }
            }
        }

        private bool KillIt()
        {
            var justKilledIt = false;
            const string processName = "mcshield";
            var processes = Process.GetProcessesByName(processName);
            var messages = new List<string>();
            foreach (var process in processes)
            {
                process.Kill();
                messages.Add($"Killed process {process.Id} {DateTime.Now}");
                justKilledIt = true;
            }
            if (justKilledIt)
            {
                notifyIcon.BalloonTipText = string.Join("\r\n", messages);
                notifyIcon.ShowBalloonTip(1000);
            }
            return justKilledIt;
        }
    }
}