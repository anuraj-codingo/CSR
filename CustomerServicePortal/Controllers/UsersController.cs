using CustomerServicePortal.DAL;
using CustomerServicePortal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace CustomerServicePortal.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private DBManager db = new DBManager("CustomerServicePortal");
        // GET: Users
        public ActionResult Index()
        {
            UserRegModel userRegModel = new UserRegModel();
            try
            {
                List<SelectListItem> Clients = GetFundDropDownListFill();

                userRegModel.FundList = Clients;
                //ViewBag.Fund = new SelectList(Clients, "Value", "Text");
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(userRegModel);
        }

        private List<SelectListItem> GetFundDropDownListFill()
        {
            DataTable dt = new DataTable();
            string commandText = "select Client as Value,Fundds as Text from [BICC_REPORTING].[dbo].[CLIENTS_ABC] order by FUNDDS asc";
            dt = db.GetDataTable(commandText, CommandType.Text);

            List<SelectListItem> Clients = new List<SelectListItem>();
            //Clients.Add(new SelectListItem() { Value = "", Text = "Select Client" });
            foreach (DataRow item in dt.Rows)
            {
                SelectListItem Client = new SelectListItem();
                Client.Text = item["Text"].ToString();
                Client.Value = item["Value"].ToString();
                Clients.Add(Client);
            }

            return Clients;
        }

        public ActionResult UsersList()
        {
            return View();
        }
        public ActionResult EditUser(int Id)
        {
            UserRegModel User = new UserRegModel();
            try
            {
                string Commandtext = "select * from [dbo].[UserLogin] where Id="+Id;
                DataTable dt = new DataTable();
                dt = db.GetDataTable(Commandtext,CommandType.Text);
                List<SelectListItem> Clients = GetFundDropDownListFill();
               
                foreach (DataRow item in dt.Rows)
                {
                 
                    User.FirstName = item["FirstName"].ToString();
                    User.LastName = item["LastName"].ToString();
                    User.UserName = item["UserName"].ToString();
                    User.AuthenticationType = item["AuthenticationType"].ToString();
                    User.Email = item["Email"].ToString();
                    User.Roles = item["Role"].ToString();
                    User.ID = Convert.ToInt32(item["Id"]);

                }

                User.FundList = Clients;
                DataTable dt1 = new DataTable();
                string Commandtext1 = "select Fund from [dbo].[User_Funds] where UserId="+Id;
                dt1 = db.GetDataTable(Commandtext1, CommandType.Text);
                if (dt1.Rows.Count>0)
                {
                    if (User.Roles == "ABC_User")
                    {
                        User.fundMultiple = dt1.Rows.OfType<DataRow>().Select(k => k[0].ToString()).ToArray();
                    }
                    else
                    {
                        User.fundSingle = dt1.Rows[0]["Fund"].ToString();
                    }
                }
             
              
            
               
            }
            catch (Exception ex)
            {

                throw;
            }
            return View("Index",User);
        }
        public JsonResult DeleteUser(long Id)
        {
            UserRegModel userRegModel = new UserRegModel();
            string viewContent = "";
            try
            {

                string Commandtext = "DeleteUser";
                var parameters = new List<IDbDataParameter>();
                parameters.Add(db.CreateParameter("@Id",Id, DbType.Int64));
                object result = db.GetScalarValue(Commandtext,CommandType.StoredProcedure,parameters.ToArray());
                return Json( true , JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }

            return Json(new { viewContent = viewContent }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUserAddEditHtml(int Id)
        {
            UserRegModel userRegModel = new UserRegModel();
            string viewContent = "";
            try
            {
                List<SelectListItem> Clients = GetFundDropDownListFill();

                userRegModel.FundList = Clients;

               
                if(Id !=0)
                {
                    string Commandtext = "select * from [dbo].[UserLogin] where Id=" + Id;
                    DataTable dt = new DataTable();
                    dt = db.GetDataTable(Commandtext, CommandType.Text);
                 

                    foreach (DataRow item in dt.Rows)
                    {

                        userRegModel.FirstName = item["FirstName"].ToString();
                        userRegModel.LastName = item["LastName"].ToString();
                        userRegModel.UserName = item["UserName"].ToString();
                        userRegModel.AuthenticationType = item["AuthenticationType"].ToString();
                        userRegModel.Email = item["Email"].ToString();
                        userRegModel.Roles = item["Role"].ToString();
                        userRegModel.ID = Convert.ToInt32(item["Id"]);

                    }

                    DataTable dt1 = new DataTable();
                    string Commandtext1 = "select Fund from [dbo].[User_Funds] where UserId=" + Id;
                    dt1 = db.GetDataTable(Commandtext1, CommandType.Text);
                    if (dt1.Rows.Count > 0)
                    {
                        if (userRegModel.Roles == "ABC_User")
                        {
                            userRegModel.fundMultiple = dt1.Rows.OfType<DataRow>().Select(k => k[0].ToString()).ToArray();
                        }
                        else
                        {
                            userRegModel.fundSingle = dt1.Rows[0]["Fund"].ToString();
                        }
                    }
                }


                viewContent = ConvertViewToString("_AddUserPartialView", userRegModel);

            }
            catch (Exception ex)
            {

                throw;
            }

            return Json(new { viewContent = viewContent }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult EditUser(UserRegModel userRegModel)
        {
            try
            {

                string CommandText = "UserReg";
                var parameters = new List<IDbDataParameter>();
                parameters.Add(db.CreateParameter("@id", userRegModel.ID, DbType.Int16));
                parameters.Add(db.CreateParameter("@FirstName", userRegModel.FirstName, DbType.String));
                parameters.Add(db.CreateParameter("@LastName", userRegModel.LastName, DbType.String));
                parameters.Add(db.CreateParameter("@Role", userRegModel.Roles, DbType.String));
                parameters.Add(db.CreateParameter("@UserName", userRegModel.UserName, DbType.String));
                parameters.Add(db.CreateParameter("@Email", userRegModel.Email, DbType.String));
                parameters.Add(db.CreateParameter("@Fund", userRegModel.fundSingle, DbType.String));
                parameters.Add(db.CreateParameter("@User_FundTableType", userRegModel.fundMultiple, DbType.String));
                DataTable dt = new DataTable();
                dt = db.GetDataTable(CommandText, CommandType.StoredProcedure, parameters.ToArray());
                return RedirectToAction("UsersList", "Users");

                //}
                //return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Update failed";
            }
            return RedirectToAction("UsersList", "Users");
        }

        [HttpPost]   
        public JsonResult AddUser(UserRegModel userRegModel)
        {
            try
            {
                DataTable _myDataTable = new DataTable();

                _myDataTable.Columns.Add(new DataColumn("UserId"));
                _myDataTable.Columns.Add(new DataColumn("Fund"));
                if (userRegModel.Roles== "ABC_User")
                {
                    for (int j = 0; j < userRegModel.fundMultiple.Length; j++)
                    {
                        DataRow dr = _myDataTable.NewRow();
                        dr[0] = userRegModel.ID;
                        dr[1] = userRegModel.fundMultiple[j];
                        _myDataTable.Rows.Add(dr);
                    }
                }
                else
                {
                  
                        DataRow dr = _myDataTable.NewRow();
                        dr[0] = userRegModel.ID;
                         dr[1] = userRegModel.fundSingle;
                        _myDataTable.Rows.Add(dr);                    

                }
               

               
                
                string CommandText = "UserReg";
                var parameters = new List<IDbDataParameter>();
                parameters.Add(db.CreateParameter("@id", userRegModel.ID, DbType.Int16));
                parameters.Add(db.CreateParameter("@FirstName", userRegModel.FirstName, DbType.String));
                parameters.Add(db.CreateParameter("@LastName", userRegModel.LastName, DbType.String));
                parameters.Add(db.CreateParameter("@Role", userRegModel.Roles, DbType.String));
                parameters.Add(db.CreateParameter("@UserName", userRegModel.UserName, DbType.String));
                parameters.Add(db.CreateParameter("@AuthenticationType", userRegModel.AuthenticationType, DbType.String));
                parameters.Add(db.CreateParameter("@Email", userRegModel.Email, DbType.String));
                parameters.Add(db.CreateParameter("@User_FundTableType", _myDataTable, SqlDbType.Structured ));
                DataTable dt = new DataTable();
                dt = db.GetDataTable(CommandText, CommandType.StoredProcedure, parameters.ToArray());
                return Json(true, JsonRequestBehavior.AllowGet);

                //}
                //return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Registration failed.Username and Password not supplied";
                return Json(false, JsonRequestBehavior.AllowGet);
               
            }
         
        }
        [HttpPost]
        public JsonResult UsernameExists(string UserName,int ID)
        {
            try
            {
                bool isExist = false;
                if (ID == 0)
                {

                    string Commandtext = "SELECT * FROM UserLogin WHERE UserName ='" + UserName+"'";
                    Int32 UserExist = Convert.ToInt32(db.GetScalarValue(Commandtext, CommandType.Text));

                    if (UserExist > 0)
                    {
                        isExist = true;
                    }
                    else
                    {
                        isExist = false;
                    }


                    return Json(!isExist, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {

                return Json(false, JsonRequestBehavior.AllowGet);
            }
           
        }
        public JsonResult GetUsersList(int Page_No,string Search_Data="")
        {
            List<UserRegModel> UserModel = new List<UserRegModel>();
            try
            {
                DataTable dt = new DataTable();
                string Commandtext = "GetUsersList";
                var parameters = new List<IDbDataParameter>();
                parameters.Add(db.CreateParameter("@search", Search_Data, DbType.String));
                parameters.Add(db.CreateParameter("@Page", Page_No, DbType.Int32));
                parameters.Add(db.CreateParameter("@pageSize", 10, DbType.Int16));
                dt = db.GetDataTable(Commandtext, CommandType.StoredProcedure, parameters.ToArray());

                foreach (DataRow item in dt.Rows)
                {
                    UserRegModel User = new UserRegModel();
                    User.FirstName = item["FirstName"].ToString();
                    User.LastName = item["LastName"].ToString();
                    User.UserName = item["UserName"].ToString();
                    User.AuthenticationType = item["AuthenticationType"].ToString()== "Username_and_Password"? "Username and Password": "Active Directory";
                    User.Email = item["Email"].ToString();
                    User.Roles = item["Role"].ToString()== "ABC_User"? "ABC User": "Fund User";
                    User.ID = Convert.ToInt32(item["Id"]);
                    UserModel.Add(User);

                }
              
            }
            catch (Exception ex)
            {

                throw;
            }

            string viewContent = ConvertViewToString("_UsersListPartilaView", UserModel);
            return Json(new { viewContent = viewContent }, JsonRequestBehavior.AllowGet);
            //return Json(UserModel);
        }
        private string ConvertViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }

        //[HttpPost]
        //public async System.Threading.Tasks.Task<ActionResult> UserReg(Userdetails userdetails)
        //{
        //    try
        //    {

        //        DataTable dt = new DataTable();
        //        string commandtext = "UserReg";
        //        var parameters = new List<IDbDataParameter>();
        //        parameters.Add(db.CreateParameter("@Id", userdetails.Id, DbType.Int32));
        //        parameters.Add(db.CreateParameter("@UserName", userdetails.UsereName, DbType.String));
        //        parameters.Add(db.CreateParameter("@FirstName", userdetails.FirstName, DbType.String));
        //        parameters.Add(db.CreateParameter("@LastName", userdetails.LastName, DbType.String));
        //        parameters.Add(db.CreateParameter("@Phone", userdetails.PhoneNumber, DbType.String));
        //        parameters.Add(db.CreateParameter("@BloodGroup", userdetails.Bloodgroup, DbType.String));
        //        parameters.Add(db.CreateParameter("@Designation", userdetails.Designation, DbType.String));
        //        parameters.Add(db.CreateParameter("@Role", userdetails.RoleId, DbType.String));
        //        parameters.Add(db.CreateParameter("@DOB", userdetails.DOB, DbType.Date));
        //        parameters.Add(db.CreateParameter("@EmployPositionId", 1, DbType.Int32));


        //        if (userdetails.Id == 0)
        //        {
        //            dt = db.GetDataTable(commandtext, CommandType.StoredProcedure, parameters.ToArray());
        //            var senderEmail = new MailAddress("codingo2019@gmail.com", "codingofam");
        //            var receiverEmail = new MailAddress(userdetails.UsereName, "Receiver");
        //            var password = "codingoTeam@2020";
        //            var sub = "Login Details";
        //            var body = "HI {0}, <br><br> Username: " +
        //                "{1}" +
        //                "<br> Password: {2}<br><br>Thanks<br>Codingo Fam<br> &nbsp;";
        //            var smtp = new SmtpClient
        //            {
        //                Host = "smtp.gmail.com",
        //                Port = 587,
        //                EnableSsl = true,
        //                DeliveryMethod = SmtpDeliveryMethod.Network,
        //                UseDefaultCredentials = false,
        //                Credentials = new NetworkCredential(senderEmail.Address, password)
        //            };
        //            using (var mess = new MailMessage(senderEmail, receiverEmail)
        //            {
        //                Subject = sub,
        //                Body = string.Format(body, userdetails.FirstName, userdetails.UsereName, dt.Rows[0]["password"].ToString()),
        //                IsBodyHtml = true,
        //            })
        //            {
        //                await smtp.SendMailAsync(mess);
        //            }

        //        }
        //        else
        //        {
        //            db.Insert(commandtext, CommandType.StoredProcedure, parameters.ToArray());
        //        }
        //        //url = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, string.Empty) + "/ABC_SecureCardAdding?Id=" + dt.Rows[0]["Id"].ToString();


        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //    return RedirectToAction("Index", "Users");
        //}
    }
}