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
    [Authorize(Roles = "ABC_User,Admin")]
    public class UsersController : Controller
    {
        private DBManager db = new DBManager("CustomerServicePortal");       

       

        public ActionResult UsersList()
        {
            return View();
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
                        if (userRegModel.Roles == "ABC_User" || userRegModel.Roles == "Admin")
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
        public ActionResult ChangePassWord(ChangepassWord changepass)
        {
            try
            {
                if (Session["UserModel"] != null)
                {
                    UserModel userModel = new UserModel();
                    userModel = (UserModel)Session["UserModel"];
                    string CommanText = "  update [UserLogin] set PassWord='" + changepass.NewPassWord + "'   where Id=" + userModel.UserId;
                    var obj = db.GetScalarValue(CommanText, CommandType.Text);
                }

            }
            catch (Exception)
            {

                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]   
        public async System.Threading.Tasks.Task<JsonResult> AddUser(UserRegModel userRegModel)
        {
            try
            {
                DataTable _myDataTable = new DataTable();

                _myDataTable.Columns.Add(new DataColumn("UserId"));
                _myDataTable.Columns.Add(new DataColumn("Fund"));
                if (userRegModel.Roles== "ABC_User" || userRegModel.Roles=="Admin")
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
                if (userRegModel.ID==0)
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0]["Email"] != null && dt.Rows[0]["Email"].ToString() != "")
                        {
                           await  Common.StaticClass.SendmailAsync(dt);
                        }
                    }
                }
                

                return Json(true, JsonRequestBehavior.AllowGet);
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
                    User.Roles = item["Role"].ToString()== "ABC_User"? "ABC User": (item["Role"].ToString() == "Admin")? "Admin":"Fund User";
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

    
    }
}