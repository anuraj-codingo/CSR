using CustomerServicePortal.Common;
using CustomerServicePortal.DAL;
using CustomerServicePortal.Models;
using DB2SQlClass.DB2;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Mvc;

namespace CustomerServicePortal.Controllers
{
    [Authorize]
    public class ClaimsController : Controller
    {
        //private DBManager db = new DBManager("CustomerServicePortal");
        // GET: Claims
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MemebersList(string SearchMember,bool GlobalSearch)
        {
            List<MemeberDetailsModel> memeberDetailsModels = new List<MemeberDetailsModel>();
            try
            {
                TempData["GlobalSearch"] = GlobalSearch;
                ViewBag.GlobalSearch = GlobalSearch;
                if (GlobalSearch)
                {
                   
                    DataTable dt = new DataTable();
                    DBManager db = new DBManager("CustomerServicePortal");
                    UserModel userModel = (UserModel)Session["UserModel"];
                    dt = db.GetDataTable(GetSqlQuery.GetEMployDetails_GlobalSearch(SearchMember,"EMNAME", "ASC",userModel.UserId, 1, 10), CommandType.Text);
                    GlobalAndLocalSearch_Datatable_TOList(memeberDetailsModels, dt);
                    ViewBag.TotalmemeberCount = db.GetScalarValue(GetSqlQuery.GlobalSearchTotalCount(SearchMember, userModel.UserId), CommandType.Text);
                }
                else
                {
                    GetmemeberListModel(SearchMember, memeberDetailsModels, 1, "EMNAME","ASC");
                    ViewBag.TotalmemeberCount = (int)Db2Connnect.GetDataTable(GetSqlQuery.TotalMemeberCount(SearchMember), CommandType.Text).Rows[0]["Total"];

                }
                ViewBag.SearchMember = SearchMember;
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(memeberDetailsModels);
        }

        public JsonResult GetMember(int page, string SearchMember,bool GlobalSearch,string SortingColumn = "Member",string Orderby="ASC")
        {
            ViewBag.GlobalSearch = GlobalSearch;
            //bool GlobalSearch = (bool)TempData.Peek("GlobalSearch");
            List<MemeberDetailsModel> memeberDetailsModels = new List<MemeberDetailsModel>();
            string viewContent = "";
            int TotalCount = 0;
            try
            {
                if (GlobalSearch)
                {
                    DataTable dt = new DataTable();
                    DBManager db = new DBManager("CustomerServicePortal");
                    UserModel userModel = (UserModel)Session["UserModel"];
                    dt = db.GetDataTable(GetSqlQuery.GetEMployDetails_GlobalSearch(SearchMember,SortingColumn, Orderby, userModel.UserId,page, 10), CommandType.Text);
                    GlobalAndLocalSearch_Datatable_TOList(memeberDetailsModels, dt);
                    viewContent = ConvertViewToString("_MemberListPartialView", memeberDetailsModels);
                    TotalCount = (int)db.GetScalarValue(GetSqlQuery.GlobalSearchTotalCount(SearchMember, userModel.UserId), CommandType.Text);

                }
                else
                {
                    GetmemeberListModel(SearchMember, memeberDetailsModels, page, SortingColumn, Orderby);
                    viewContent = ConvertViewToString("_MemberListPartialView", memeberDetailsModels);
                    TotalCount = (int)Db2Connnect.GetDataTable(GetSqlQuery.TotalMemeberCount(SearchMember), CommandType.Text).Rows[0]["Total"];

                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(new { Page = page, TotalCount = TotalCount, viewContent = viewContent }, JsonRequestBehavior.AllowGet);
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

        private static void GetmemeberListModel(string SearchMember, List<MemeberDetailsModel> memeberDetailsModels, int page,string SortingColumn,string Orderby)
        {
            DataTable dt = new DataTable();

            dt = Db2Connnect.GetDataTable(GetSqlQuery.GetEMployDetails(SearchMember, SortingColumn, Orderby, page, 10), CommandType.Text);

            GlobalAndLocalSearch_Datatable_TOList(memeberDetailsModels, dt);
        }

        private static void GlobalAndLocalSearch_Datatable_TOList(List<MemeberDetailsModel> memeberDetailsModels, DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    MemeberDetailsModel memeberDetailsModel = new MemeberDetailsModel();
                    memeberDetailsModel.SSN = item["SSN"].ToString();
                    if (item["Member"].ToString().Contains("*"))
                    {
                        memeberDetailsModel.Member = item["Member"].ToString().Replace("*", ",");

                    }
                    else
                    {
                        memeberDetailsModel.Member = item["Member"].ToString();
                    }
                    memeberDetailsModel.Year = item["Year"].ToString();
                    memeberDetailsModel.Month = item["Month"].ToString();
                    memeberDetailsModel.Day = item["Day"].ToString();
                    memeberDetailsModel.City = item["City"].ToString();
                    memeberDetailsModel.State = item["State"].ToString();
                    memeberDetailsModel.ID = item["ID"].ToString();
                    if (dt.Columns.Contains("Client"))
                    {
                        memeberDetailsModel.Client = item["Client"].ToString();
                    }
                    memeberDetailsModels.Add(memeberDetailsModel);
                }
            }
        }
    }
}