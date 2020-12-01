using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace CustomerServicePortal
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //protected void Application_BeginRequest(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        bool cookieFound = false;

        //        HttpCookie authCookie = null;

        //        for (int i = 0; i < Request.Cookies.Count; i++)
        //        {
        //            HttpCookie cookie = Request.Cookies[i];
        //            if (cookie.Name == ".MVCAUTH")
        //            {
        //                cookieFound = true;
        //                authCookie = cookie;
        //                break;
        //            }
        //        }

        //        if (cookieFound)
        //        {
        //            // Extract the roles from the cookie, and assign to our current principal, which is attached to the HttpContext.
        //            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
        //            HttpContext.Current.User = new GenericPrincipal(new FormsIdentity(ticket), ticket.UserData.Split(';'));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}


        //protected void Application_AuthenticateRequest()
        //{
        //    var returnUrl = Request.QueryString["ReturnUrl"];
        //    if (!Request.IsAuthenticated && !String.IsNullOrWhiteSpace(returnUrl))
        //    {
        //        var returnUrlCookie = new HttpCookie(".MVCRETURNURL", returnUrl) { HttpOnly = true };
        //        Response.Cookies.Add(returnUrlCookie);
        //    }
        //}
    }
}
