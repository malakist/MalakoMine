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
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

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
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Malako", action = "Index", id = UrlParameter.Optional } // Parameter defaults
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

            Uri uri = new Uri("http://tempuri.org/");
            ICredentials credentials = CredentialCache.DefaultCredentials;
            NetworkCredential credential = credentials.GetCredential(uri, "Basic");

            TFS.TFSMalako malako = new TFS.TFSMalako();

            malako.ServerName = ConfigurationManager.AppSettings["TFSServer"];
            malako.ProjectName = ConfigurationManager.AppSettings["TFSProject"];
            malako.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials; 
            malako.Connect();
            Session.Add("TFSMalako", malako);
        }
    }
}