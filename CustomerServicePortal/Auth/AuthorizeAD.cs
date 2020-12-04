using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;



namespace CustomerServicePortal.Auth
{
    public class AuthorizeAD : System.Web.Mvc.AuthorizeAttribute
    {
        Transaction objtran = new Transaction();

        private bool _authenticated;
        private bool _authorized;

        public string Groups { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }


        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            try
            {
                base.HandleUnauthorizedRequest(filterContext);

                if (_authenticated && !_authorized)
                {
                    filterContext.Result = new System.Web.Mvc.RedirectResult("~/Account/Login");
                }
            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");

            }

        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {


            String username = "";
            if (HttpContext.Current.Session["form_UserName"] != null && HttpContext.Current.Session["form_UserName"].ToString() != "" && HttpContext.Current.Session["Password"] != null && HttpContext.Current.Session["Password"].ToString() != "")
            {
                Groups = ConfigurationManager.AppSettings["ADGroup"];
                username = HttpContext.Current.Session["form_UserName"].ToString();// GetUser.UserName;
                //Password = HttpContext.Current.Session["Password"].ToString();

            }
            else
            {
                Groups = ConfigurationManager.AppSettings["ADGroup"];
                string strDomainName = HttpContext.Current.User.Identity.Name; // System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                int index = strDomainName.IndexOf(@"\");
                username = strDomainName.Substring(index + 1);


            }

            _authenticated = base.AuthorizeCore(httpContext);

            if (_authenticated)
            {
                if (string.IsNullOrEmpty(Groups))
                {
                    _authorized = true;
                    return _authorized;
                }

                var groups = Groups.Split(',');

                //if (UserName!="" && UserName!=null)
                //{
                //    username = UserName;
                //}
                //else
                //{

                //}

                //string username = httpContext.User.Identity.Name;

                try
                {

                    if ((HttpContext.Current.Session["form_UserName"] != null && HttpContext.Current.Session["form_UserName"].ToString() != "" && HttpContext.Current.Session["Password"] != null && HttpContext.Current.Session["Password"].ToString() != ""))
                    {
                        _authorized = LDAPHelper.UserIsMemberOfGroups(username, groups, HttpContext.Current.Session["Password"].ToString());
                    }
                    else
                    {

                        _authorized = LDAPHelper.UserIsMemberOfGroups(username, groups, string.Empty);
                    }

                    if (_authorized)
                    {
                        if (HttpContext.Current.Session["FirstName"] != null && HttpContext.Current.Session["LastName"] != null && HttpContext.Current.Session["Email"] != null)
                        {
                            objtran.Action = "Check_login";
                            objtran.user_id = Convert.ToString(HttpContext.Current.Session["UserName"]);
                            objtran.first_name = Convert.ToString(HttpContext.Current.Session["FirstName"]);
                            objtran.last_name = Convert.ToString(HttpContext.Current.Session["LastName"]);
                            objtran.email = Convert.ToString(HttpContext.Current.Session["Email"]);
                            DataTable dt = objtran.LoginAuth();
                            if (dt != null)
                            {
                                if (dt.Rows.Count > 0)
                                {
                                    HttpContext.Current.Session["UserFullName"] = Convert.ToString(dt.Rows[0]["Fullname"]);
                                    HttpContext.Current.Session["UserEmail"] = Convert.ToString(dt.Rows[0]["email"]);
                                    HttpContext.Current.Session["UserEmpID"] = Convert.ToInt32(dt.Rows[0]["employee_id"]);
                                    HttpContext.Current.Session["Role"] = Convert.ToString(dt.Rows[0]["Role"]);
                                    //HttpContext.Current.Session["UserName"] = username;

                                }
                                else
                                {
                                    _authorized = false;
                                }


                            }
                            else
                            {
                                _authorized = false;
                            }

                        }





                    }
                    return _authorized;
                }
                catch (Exception ex)
                {
                    //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");
                    //this.Log().Error(() => "Error attempting to authorize user", ex);
                    _authorized = false;
                    return _authorized;
                }
            }

            _authorized = false;
            return _authorized;
        }
    }
}