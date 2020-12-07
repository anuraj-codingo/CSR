using CustomerServicePortal.Models;
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
        public ActionResult MemebersList(string SearchMember)
        {
            List<MemeberDetailsModel> memeberDetailsModels = new List<MemeberDetailsModel>();
            try
            {
                GetmemeberListModel(SearchMember, memeberDetailsModels, 1);
                ViewBag.TotalmemeberCount = (int)Db2Connnect.GetDataTable(GetSqlQuery.TotalMemeberCount(SearchMember), CommandType.Text).Rows[0]["Total"];
                ViewBag.SearchMember = SearchMember;
            }
            catch (Exception)
            {
                throw;
            }
            return View(memeberDetailsModels);
        }

        public JsonResult GetMember(int page, string SearchMember)
        {
            List<MemeberDetailsModel> memeberDetailsModels = new List<MemeberDetailsModel>();
            string viewContent = "";
            int TotalCount = 0;
            try
            {
                GetmemeberListModel(SearchMember, memeberDetailsModels, page);

                viewContent = ConvertViewToString("_MemberListPartialView", memeberDetailsModels);
                TotalCount = (int)Db2Connnect.GetDataTable(GetSqlQuery.TotalMemeberCount(SearchMember), CommandType.Text).Rows[0]["Total"];
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

        private static void GetmemeberListModel(string SearchMember, List<MemeberDetailsModel> memeberDetailsModels, int page)
        {
            DataTable dt = new DataTable();

            dt = Db2Connnect.GetDataTable(GetSqlQuery.GetEMployDetails(SearchMember, page, 10), CommandType.Text);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    MemeberDetailsModel memeberDetailsModel = new MemeberDetailsModel();
                    memeberDetailsModel.SSN = item["SSN"].ToString();
                    memeberDetailsModel.Member = (item["Member"].ToString().Split('*')[1] + "*" + item["Member"].ToString().Split('*')[0]).Replace("*","");
                    memeberDetailsModel.Year = item["Year"].ToString();
                    memeberDetailsModel.Month = item["Month"].ToString();
                    memeberDetailsModel.Day = item["Day"].ToString();
                    memeberDetailsModel.City = item["City"].ToString();
                    memeberDetailsModel.State = item["State"].ToString();
                    memeberDetailsModel.ID = item["ID"].ToString();
                    memeberDetailsModels.Add(memeberDetailsModel);
                }
            }
        }
    }
}