using CustomerServicePortal.Auth;
using CustomerServicePortal.DAL;
using CustomerServicePortal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DB2SQlClass.DB2;

namespace CustomerServicePortal.Controllers
{
    //[Authorize(Roles = "ABC_User")]
    [Authorize]
    public class ABCDashBoardController : Controller
    {
        private DBManager db = new DBManager("CustomerServicePortal");
        // GET: ABC_UserDashBoard
        public ActionResult Index()
        {
            List<ABC_DashBoardModel> aBC_DashBoardModels = new List<ABC_DashBoardModel>();
            try
            {
                if (Session["UserModel"] != null)
                {
                    UserModel userModel = (UserModel)Session["UserModel"];
                    DataTable dt = new DataTable();
                    dt = db.GetDataTable(GetSqlQuery.GetABC_UserFundList(userModel.UserId), CommandType.Text);

                    foreach (DataRow item in dt.Rows)
                    {
                        ABC_DashBoardModel aBC_DashBoard = new ABC_DashBoardModel();

                        aBC_DashBoard.Fund = item["FUNDDS"].ToString();
                        aBC_DashBoard.Client = item["CLIENT"].ToString();
                        aBC_DashBoard.logo= item["Headerlogo"].ToString();
                        aBC_DashBoardModels.Add(aBC_DashBoard);

                    }

                }
                else
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            catch (Exception)
            {

                throw;
            }


           
            return View(aBC_DashBoardModels);
        }
    }
}