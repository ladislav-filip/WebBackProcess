﻿using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Web;

namespace WebBackProcess
{
    public class Protocol : IProtocol
    {
        private bool m_stopRequest;
        private readonly DateTime m_created = DateTime.Now;
        private Thread m_thread;
        private readonly ConcurrentQueue<string> m_queue = new ConcurrentQueue<string>();

        /// <summary>
        /// V rámci requestu pouze jediná instance 
        /// </summary>
        public static IProtocol Instance
        {
            get
            {
                var result = HttpContext.Current.Items[typeof(Protocol)] as Protocol;
                if (result == null)
                {
                    result = new Protocol();
                    HttpContext.Current.Items[typeof(Protocol)] = result;
                }
                return result;
            }  
        } 

        public void StopRequest()
        {
            m_stopRequest = true;
        }

        public void AddToQueue(string data)
        {
            m_queue.Enqueue(data);
            if (m_thread == null)
            {
                m_stopRequest = false;
                m_thread = new Thread(StartWorking);
                m_thread.Start();
            }
        }

        private void StartWorking()
        {            
            Debug.WriteLine("...start Protocol thread");
            while (!m_stopRequest)
            {
                ProcessQueue();
                Thread.Sleep(100);
                //Debug.WriteLine($"...working {m_created:HH:mm:ss tt}");
            }
            m_thread = null;
            Debug.WriteLine("...stop Protocol thread");
        }

        private void ProcessQueue()
        {
            var data = string.Empty;
            while (!m_stopRequest && m_queue.TryDequeue(out data))
            {
                Debug.WriteLine($"{data} - /{m_created:HH:mm:ss tt}/");
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
                    m_thread = null;                    
                }
                Debug.WriteLine("...stopping and disposing Protocol");
            }

            m_disposed = true;
        }

        ~Protocol()
        {
            Dispose(false);
        }

        #endregion
    }
}