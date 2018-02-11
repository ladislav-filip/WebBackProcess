using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace WebBackProcess.Controllers
{
    public enum ExecWorkingThread { Continue, Done, Terminate }

    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var prt = Protocol.Instance;
            //using (var prt = Protocol.Instance)
            {
                var idp = System.Diagnostics.Process.GetCurrentProcess().Id;
                var rnd = new Random();
                var m_rnd = rnd.Next(100, 999);
                for (var i = 0; i < 30; i++)
                {
                    prt.AddToQueue($"[{idp} / {m_rnd}] volani: {i}");
                    Thread.Sleep(500);
                    if (i == 12)
                    {
                        prt.StopRequest();
                        break;
                    }
                }
            }
           
            return View();
        }

        private ExecWorkingThread m_execState;

        public ActionResult Exec()
        {
            var idp = System.Diagnostics.Process.GetCurrentProcess().Id;
            var rnd = new Random();
            var m_rnd = rnd.Next(100, 999);

            var ex = Executor.Instance;
            {
                ex.AfterExec += AfterExec;
                for (var i = 0; i < 30; i++)
                {
                    if (m_execState == ExecWorkingThread.Terminate)
                    {
                        break;
                    }
                    m_execState = ExecWorkingThread.Continue;
                    ex.ExecToQueue($"[{idp} / {m_rnd}] volani: {i}");
                    while (m_execState == ExecWorkingThread.Continue)
                    {
                        Thread.Sleep(50);
                    }                    
                }
            }
            return View("Index");
        }

        private void AfterExec(ExecDTO data)
        {
            m_execState = ExecWorkingThread.Done;
            Debug.WriteLine(data.Data);            
        }
    }
}