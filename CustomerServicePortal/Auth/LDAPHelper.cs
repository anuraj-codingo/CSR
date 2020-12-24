using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace CustomerServicePortal.Auth
{

    public static class LDAPHelper
    {
        public static string GetLDAPContainer()
        {
            Uri ldapUri;
            ParseLDAPConnectionString(out ldapUri);

            return HttpUtility.UrlDecode(ldapUri.PathAndQuery.TrimStart('/'));
        }

        public static string GetLDAPHost()
        {
            Uri ldapUri;
            ParseLDAPConnectionString(out ldapUri);

            return ldapUri.Host;
        }

        public static bool ParseLDAPConnectionString(out Uri ldapUri)
        {
            string connString = ConfigurationManager.ConnectionStrings["ADConnectionString"].ConnectionString;

            return Uri.TryCreate(connString, UriKind.Absolute, out ldapUri);
        }

        public static bool UserIsMemberOfGroups(string username, string[] groups, string Password)
        {
            /* Return true immediately if the authorization is not
            locked down to any particular AD group */
            if (groups == null || groups.Length == 0)
            {
                return true;
            }

            // Verify that the user is in the given AD group (if any)
            using (var context = BuildPrincipalContext())
            {
                var userPrincipal = UserPrincipal.FindByIdentity(context,
                    IdentityType.SamAccountName,
                    username);

                foreach (var group in groups)
                {
                    if (userPrincipal.IsMemberOf(context, IdentityType.Name, group))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static PrincipalContext BuildPrincipalContext()
        {
            string container = LDAPHelper.GetLDAPContainer();
            return new PrincipalContext(ContextType.Domain, null, container);
        }
    }

    //public static class LDAPHelper
    //{
    //    public static string GetLDAPContainer()
    //    {

    //        Uri ldapUri;
    //        ParseLDAPConnectionString(out ldapUri);

    //        return HttpUtility.UrlDecode(ldapUri.PathAndQuery.TrimStart('/'));
    //    }

    //    public static string GetLDAPHost()
    //    {
    //        Uri ldapUri;
    //        ParseLDAPConnectionString(out ldapUri);

    //        return ldapUri.Host;
    //    }

    //    public static bool ParseLDAPConnectionString(out Uri ldapUri)
    //    {
    //        string connString = ConfigurationManager.ConnectionStrings["ADConnectionString"].ConnectionString;

    //        return Uri.TryCreate(connString, UriKind.Absolute, out ldapUri);
    //    }

    //    public static bool UserIsMemberOfGroups(string username, string[] groups, string Password)
    //    {
    //        /* Return true immediately if the authorization is not
    //        locked down to any particular AD group */
    //        if (groups == null || groups.Length == 0)
    //        {
    //            return true;
    //        }

    //        // Verify that the user is in the given AD group (if any)





    //        HttpContext.Current.Session["Error"] = "Test: " + HttpContext.Current.User.Identity.Name;



    //        using (var context = BuildPrincipalContext(username, Password))
    //        {
    //            try
    //            {
    //                HttpContext.Current.Session["Error"] += " Connected Server :" + context.ConnectedServer + " Container :" + context.Container + " Context Type: " + context.ContextType + " Name :" + context.Name;


    //                var userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName,
    //                    username);
    //                HttpContext.Current.Session["Error"] += " User:" + userPrincipal;

    //                UserPrincipal user = UserPrincipal.FindByIdentity(context, username);

    //                HttpContext.Current.Session["Error"] += " User Email:" + user.EmailAddress;


    //                DirectoryEntry de = (user.GetUnderlyingObject() as DirectoryEntry);
    //                HttpContext.Current.Session["Error"] += " Directory Path:" + de.Path + " Directory Name" + de.Name;

    //                List<string> result = new List<string>();
    //                WindowsIdentity wi = new WindowsIdentity(username);
    //                HttpContext.Current.Session["Error"] += " windows Identity " + wi.Groups[0];

    //                foreach (IdentityReference group in wi.Groups)
    //                {




    //                    try
    //                    {
    //                        result.Add(group.Translate(typeof(NTAccount)).ToString());
    //                    }
    //                    catch (Exception ex)
    //                    {

    //                        //ExceptionLog.WriteToErrorLog(ex.Message + " " + HttpContext.Current.Session["Error"], ex.StackTrace, "LDAP UserIsMemberOfGroups ");

    //                    }
    //                }
    //                result.Sort();
    //                //return result;

    //                // ArrayList grp = GetGroups();
    //                string strgrp = groups[0];


    //                foreach (var grp in result)
    //                {

    //                    if (grp.ToLower().Contains(HttpContext.Current.User.Identity.Name.Substring(0, (HttpContext.Current.User.Identity.Name.IndexOf("\\"))).ToLower() + "\\" + strgrp.ToLower()))
    //                    {

    //                        HttpContext.Current.Session["FirstName"] = userPrincipal.GivenName; // First Name
    //                        HttpContext.Current.Session["LastName"] = userPrincipal.Surname; // Last Name
    //                        HttpContext.Current.Session["Email"] = userPrincipal.UserPrincipalName; //Email
    //                        HttpContext.Current.Session["UserName"] = userPrincipal.SamAccountName;
    //                        return true;
    //                    }
    //                }
    //            }
    //            catch (Exception ex)
    //            {


    //                //ExceptionLog.WriteToErrorLog(ex.Message + HttpContext.Current.Session["Error"], ex.StackTrace, "Error Count on LDAP");

    //                return false;
    //            }

    //        }




    //        return false;

    //    }

    //    public static PrincipalContext BuildPrincipalContext(string str_userName, string str_Pwd)
    //    {

    //        string container = string.Empty;


    //        string str_Domain = string.Empty;
    //        string str_DomainProperty = string.Empty;

    //        try
    //        {
    //            container = LDAPHelper.GetLDAPContainer();

    //            str_Domain = ConfigurationManager.AppSettings["DomainName"];
    //            str_DomainProperty = ConfigurationManager.AppSettings["DomainProperty"];



    //            if (str_Pwd != "" && str_Pwd != null)
    //            {

    //                return new PrincipalContext(ContextType.Domain, str_Domain, str_DomainProperty, str_userName, str_Pwd);
    //            }
    //            else
    //            {
    //                str_userName = ConfigurationManager.AppSettings["DomainUser"];
    //                str_Pwd = ConfigurationManager.AppSettings["DomainPassword"];
    //                // HttpContext.Current.Session["Error"] += container + ContextType.Domain + str_DomainProperty+str_userName;

    //                return new PrincipalContext(ContextType.Domain, str_Domain, str_DomainProperty);

    //            }

    //        }
    //        catch (Exception ex)
    //        {


    //            //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "BuildPrincipalContext");


    //            return null;

    //        }



    //        //return new PrincipalContext(ContextType.Domain, "americanbenefitcorp.com", "DC=americanbenefitcorp,DC=com","adsync","@dSyncP@ss0!");
    //    }
    //    public static ArrayList GetGroups()
    //    {
    //        ArrayList groups = new ArrayList();
    //        foreach (System.Security.Principal.IdentityReference group in
    //            System.Web.HttpContext.Current.Request.LogonUserIdentity.Groups)
    //        {
    //            groups.Add(group.Translate(typeof
    //                (System.Security.Principal.NTAccount)).ToString());
    //        }
    //        return groups;
    //    }
    //}
}