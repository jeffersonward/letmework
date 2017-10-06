using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LetMeWork
{
    public partial class LetMeWork : Form
    {
        private const string StopText = "&Stop";
        private const string StartText = "&Start";

        private readonly ProcessKiller[] _killers;
        private bool _running;

        public LetMeWork(IEnumerable<string> processes)
        {
            _killers = processes.Select(process => new ProcessKiller(process, Notify)).ToArray();

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
            ForAll(killer => killer.Stop());
            Application.Exit();
        }

        private void killItToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ForAll(killer => killer.KillIt());
        }

        private void startStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_running)
            {
                ForAll(killer => killer.Stop());
                startStopToolStripMenuItem.Text = StopText;
            }
            else
            {
                ForAll(killer => killer.Start());
                startStopToolStripMenuItem.Text = StopText;
            }
            _running = !_running;
        }

        private void Notify(ProcessKiller killer, string message)
        {
            notifyIcon.BalloonTipText = $"{killer.ProcessName} {message}";
            notifyIcon.ShowBalloonTip(1000);
        }

        private void ForAll(Action<ProcessKiller> action)
        {
            foreach (var killer in _killers)
            {
                action(killer);
            }
        }
    }
}