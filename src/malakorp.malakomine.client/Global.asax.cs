using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Net;
using System.Configuration;

namespace Malakorp.MalakoMine.Client
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Malako", action = "Index", id = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Session_Start()
        {
            var malako = new TFS.MalakoQueryProvider(
                ConfigurationManager.AppSettings["TFSServer"],
                ConfigurationManager.AppSettings["TFSProject"],
                System.Net.CredentialCache.DefaultNetworkCredentials //CredentialCache.DefaultCredentials.GetCredential(new Uri("http://tempuri.org/"), "Basic")
            );
            
            malako.Connect();
            Session.Add("TFSMalako", malako);
        }
    }
}