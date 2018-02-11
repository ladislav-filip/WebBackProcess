using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace WebBackProcess.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            using (var prt = Protocol.Instance)
            {
                var idp = System.Diagnostics.Process.GetCurrentProcess().Id;
                var rnd = new Random();
                var m_rnd = rnd.Next(100, 999);
                for (var i = 0; i < 30; i++)
                {
                    prt.AddToQueue($"[{idp} / {m_rnd}] volani: {i}");
                    Thread.Sleep(500);
                }
            }
           
            return View();
        }

        private bool m_execFinnish;

        public ActionResult Exec()
        {
            var idp = System.Diagnostics.Process.GetCurrentProcess().Id;
            var rnd = new Random();
            var m_rnd = rnd.Next(100, 999);

            using (var ex = Executor.Instance)
            {
                ex.AfterExec += AfterExec;
                for (var i = 0; i < 30; i++)
                {
                    ex.ExecToQueue($"[{idp} / {m_rnd}] volani: {i}");
                    while (!m_execFinnish)
                    {
                        Thread.Sleep(50);
                    }
                    m_execFinnish = false;
                }

            }
            return View("Index");
        }

        private void AfterExec(string data)
        {
            m_execFinnish = true;
            Debug.WriteLine(data);
        }
    }
}