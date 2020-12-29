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

              
                if (loginModel.UserName != "" && loginModel.UserPassword != "")
                {
                    DataTable dt = new DataTable();
                    string commandText = " select * from  [dbo].[UserLogin] where [UserName]='" + loginModel.UserName + "' and [PassWord]='" + loginModel.UserPassword + "'";
                    dt = db.GetDataTable(commandText, CommandType.Text);

                    UserModel userModel = new UserModel();
                    if (dt.Rows.Count > 0)
                    {

                        userModel.FirstName = dt.Rows[0]["FirstName"].ToString();
                        userModel.LastName = dt.Rows[0]["LastName"].ToString();
                        userModel.Role = dt.Rows[0]["Role"].ToString();
                        userModel.UserName = dt.Rows[0]["UserName"].ToString();
                        userModel.UserId = (long)dt.Rows[0]["Id"];

                        FormsAuthentication.SetAuthCookie(userModel.UserName, false);

                        var authTicket = new FormsAuthenticationTicket(1, userModel.UserName, DateTime.Now, DateTime.Now.AddMinutes(20), false, userModel.Role);
                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                        var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        HttpContext.Response.Cookies.Add(authCookie);

                        Session["UserModel"] = userModel;
                        if (userModel.Role== "ABC_User")
                        {
                            return RedirectToAction("Index", "ABCDashBoard");
                        }
                        else if(userModel.Role == "Fund_User")
                        {
                            string Commandtext = "select fund as Client from [dbo].[User_Funds] where UserId="+userModel.UserId;
                            object client = db.GetScalarValue(Commandtext, CommandType.Text);
                            return RedirectToAction("Index", "Home",new { Client=client.ToString() });
                        }
                        else if (userModel.Role == "Admin")
                        {
                            return RedirectToAction("Index", "ABCDashBoard");
                        }
                       
                    }
                    else
                    {
                        TempData["Message"] = "Invalid login attempt.";
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(loginModel);
                    }
              
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