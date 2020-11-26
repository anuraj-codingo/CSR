using CustomerServicePortal.DAL;
using CustomerServicePortal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomerServicePortal.Controllers
{
    public class UsersController : Controller
    {
        private DBManager db = new DBManager("CustomerServicePortal");
        // GET: Users
        public ActionResult Index()
        {
            try
            {
                DataTable dt = new DataTable();
                string commandText = "select Client as Value,Fundds as Text from [BICC_REPORTING].[dbo].[CLIENTS_ABC] order by FUNDDS asc";
                dt = db.GetDataTable(commandText, CommandType.Text);

                List<SelectListItem> Clients = new List<SelectListItem>();
                Clients.Add(new SelectListItem() { Value = "", Text = "Select Client" });
                foreach (DataRow item in dt.Rows)
                {
                    SelectListItem Client = new SelectListItem();
                    Client.Text = item["Text"].ToString();
                    Client.Value = item["Value"].ToString();
                    Clients.Add(Client);
                }
                ViewBag.Fund = new SelectList(Clients, "Value", "Text");
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(new UserRegModel());
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
                dt = db.GetDataTable(Commandtext,CommandType.StoredProcedure);
           
                foreach (DataRow item in dt.Rows)
                {
                 
                    User.FirstName = item["FirstName"].ToString();
                    User.LastName = item["LastName"].ToString();
                    User.UserName = item["UserName"].ToString();
                    User.fund = item["Fund"].ToString();
                    User.Email = item["Email"].ToString();
                    User.Roles = item["Role"].ToString();
                    User.ID = Convert.ToInt32(item["Id"]);

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(User);
        }
        [HttpPost]   
        public ActionResult AddUser(UserRegModel userRegModel)
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
                parameters.Add(db.CreateParameter("@Fund", userRegModel.fund, DbType.String));
                DataTable dt = new DataTable();
                dt = db.GetDataTable(CommandText, CommandType.StoredProcedure, parameters.ToArray());
                return RedirectToAction("Index", "Users");

                //}
                //return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Registration failed.Username and Password not supplied";
            }
            return RedirectToAction("index");
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
                    User.fund = item["Fund"].ToString();
                    User.Email = item["Email"].ToString();
                    User.Roles = item["Role"].ToString();
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