using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Web;

namespace WebBackProcess
{
    public class Executor : IExecutor, IDisposable
    {
        private bool m_stopRequest;
        private readonly DateTime m_created = DateTime.Now;
        private Thread m_thread;
        private readonly ConcurrentQueue<string> m_queue = new ConcurrentQueue<string>();

        public static IExecutor Instance
        {
            get
            {
                var result = HttpContext.Current.Items["Executor"] as Executor;
                if (result == null)
                {
                    result = new Executor();
                    HttpContext.Current.Items["Executor"] = result;
                }
                return result;
            }
        }

        public void StopRequest()
        {
            m_stopRequest = true;
        }

        public void ExecToQueue(string data)
        {
            m_queue.Enqueue(data);
            if (m_thread == null)
            {
                m_thread = new Thread(StartWorking);
                m_thread.Start();
            }
        }

        public AfterExecCallback AfterExec { get; set; }

        private void StartWorking()
        {
            Debug.WriteLine("...start Executor thread");
            while (!m_stopRequest)
            {
                ProcessQueue();
                Thread.Sleep(100);
                //Debug.WriteLine($"...working {m_created:HH:mm:ss tt}");
            }
        }

        private void ProcessQueue()
        {
            var data = string.Empty;
            while (m_queue.TryDequeue(out data))
            {
                Debug.WriteLine($"{data} - /{m_created:HH:mm:ss tt}/");
                AfterExec("hotovo");
            }
        }

        #region IDisposable

        private bool m_disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            if (disposing)
            {
                if (m_thread != null)
                {
                    StopRequest();
                    m_thread.Join();
                    m_thread = null;
                    Debug.WriteLine("...stopping and disposing Executor");
                }
            }

            m_disposed = true;
        }

        ~Executor()
        {
            Dispose(false);
        }

        #endregion
    }
}