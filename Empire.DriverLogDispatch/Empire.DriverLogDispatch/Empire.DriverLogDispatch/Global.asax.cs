using System;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Empire.DriverLog
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            log4net.Config.XmlConfigurator.Configure();
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie oHttpAuthCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (oHttpAuthCookie == null)
            {
                return;
            }
            var oFormsAuthenticationTicket = FormsAuthentication.Decrypt(oHttpAuthCookie.Value);
            var oGenericIdentity = new GenericIdentity(oFormsAuthenticationTicket.Name, "Forms");
            string[] aRoles = { "User" };
            this.Context.User = new GenericPrincipal(oGenericIdentity, aRoles);
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
           Response.Headers.Add("Allow", "GET,POST,PUT,DELETE,OPTIONS");
        }
    }
}
