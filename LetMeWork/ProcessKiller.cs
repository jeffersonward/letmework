using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace LetMeWork
{
    public class ProcessKiller
    {
        private readonly Action<ProcessKiller, string> _logger;

        private bool _running;
        private Thread _thread;

        public ProcessKiller(string processName, Action<ProcessKiller, string> logger)
        {
            ProcessName = processName;
            _logger = logger;
        }

        public string ProcessName { get; }

        public void Start()
        {
            _running = true;
            _thread = new Thread(Run)
            {
                Name = $"Process Killer {ProcessName}",
                Priority = ThreadPriority.Highest
            };
            _thread.Start();
        }

        public void Stop()
        {
            if (!_running) return;

            _running = false;
            _thread.Interrupt();
            _thread.Abort();
        }

        public void KillIt()
        {
            if (_running)
            {
                _thread.Interrupt();
            }
            else
            {
                DoKillIt();
            }
        }

        private void Run()
        {
            const int tenSeconds = 10 * 1000;
            const int increment = tenSeconds;
            const int oneMinute = 60 * 1000;

            var sleep = tenSeconds;
            while (_running)
            {
                var justKilledIt = DoKillIt();

                if (justKilledIt)
                {
                    sleep = tenSeconds;
                }
                else
                {
                    sleep = sleep + increment;
                    if (sleep > oneMinute)
                    {
                        sleep = oneMinute;
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

        private bool DoKillIt()
        {
            var justKilledIt = false;
            var processes = Process.GetProcessesByName(ProcessName);
            var messages = new List<string>();
            foreach (var process in processes)
            {
                try
                {
                    process.Kill();
                    messages.Add($"Killed process {process.Id} {DateTime.Now}");
                }
                catch (Exception e)
                {
                    messages.Add(e.Message);
                }

                justKilledIt = true;
            }
            if (justKilledIt)
            {
                _logger?.Invoke(this, string.Join("\r\n", messages));
            }
            return justKilledIt;
        }
    }
}