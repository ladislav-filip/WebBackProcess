using System;
using System.Collections.Generic;
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
    }
}