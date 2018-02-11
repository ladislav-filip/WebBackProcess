using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebBackProcess
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            // projdu vše co během requestu vzniklo a je disposable...
            foreach (var item in HttpContext.Current.Items.Values)
            {
                // ...a zruším to
                var disposableItem = item as IDisposable;
                disposableItem?.Dispose();
            }
        }
    }
}
