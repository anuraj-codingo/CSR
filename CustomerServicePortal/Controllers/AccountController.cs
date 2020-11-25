using CustomerServicePortal.DAL;
using CustomerServicePortal.Models;
using System;
using System.Data;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CustomerServicePortal.Controllers
{
    public class AccountController : Controller
    {
        private DBManager db = new DBManager("CustomerServicePortal");

        // GET: Account
        public ActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel loginModel)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    return View(loginModel);
                //}
                if (loginModel.UserName != "" && loginModel.UserPassword != "")
                {
                    DataTable dt = new DataTable();
                    string commandText = " select * from  [dbo].[UserLogin] where [UserName]='" + loginModel.UserName + "' and [PassWord]='" + loginModel.UserPassword + "'";
                    dt = db.GetDataTable(commandText, CommandType.Text);

                    LoginModel userModel = new LoginModel();
                    if (dt.Rows.Count > 0)
                    {
                        FormsAuthentication.SetAuthCookie(loginModel.UserName, false);

                        var authTicket = new FormsAuthenticationTicket(1, loginModel.UserName, DateTime.Now, DateTime.Now.AddMinutes(20), false, loginModel.UserName);
                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                        var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        HttpContext.Response.Cookies.Add(authCookie);

                        Session["loginModel"] = loginModel;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["Message"] = "Invalid login attempt.";
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(loginModel);
                    }
                    //while (rdr.Read())
                    //{
                    //    userModel.UserName = (rdr["UserName"]).ToString();
                    //    //userModel.UserPassword = (rdr["PassWord"]).ToString();
                    //    userModel.SSN = (rdr["SSN"]).ToString();
                    //}
                }
                else
                {
                    TempData["Message"] = "Invalid login attempt.";
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(loginModel);
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Login failed.Username and Password not supplied";
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}