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
    [Authorize]
    public class IdCardListController : Controller
    {
        private DBManager db = new DBManager("CustomerServicePortal");
        // GET: IdCardList
        public ActionResult Index()
        {
            List<IdCardListModel> idCardListModels = new List<IdCardListModel>();
            try
            {
                DataSet ds = IdcardrequestListDataset(1);

                ViewBag.TotalmemeberCount = (int)ds.Tables[0].Rows[0]["Count"];

                IdCardRequestDatatableToList(idCardListModels, ds);




                //string CommandText = "select * from [IDCardRequestDetails] where Requester = '"+User.Identity.Name+"'";
                //dt = db.GetDataTable(CommandText,CommandType.Text);


            }
            catch (Exception ex)
            {

                throw;
            }
            return View(idCardListModels);
        }

        private static void IdCardRequestDatatableToList(List<IdCardListModel> idCardListModels, DataSet ds)
        {
            foreach (DataRow item in ds.Tables[1].Rows)
            {
                IdCardListModel idCardListModel = new IdCardListModel();
                idCardListModel.Id = (long)item["Id"];
                idCardListModel.EMSSN = item["EMSSN"].ToString();
                idCardListModel.Name = item["Name"].ToString().Replace("*", "");
                idCardListModel.Gender = item["Gender"].ToString();
                idCardListModel.Completestatus = (bool)item["Complete"];
                DataTable dt1 = new DataTable();
                dt1 = Db2Connnect.GetDataTable(GetSqlQuery.GetMemberDetailsWIthSSN(idCardListModel.EMSSN), CommandType.Text);
                if (dt1.Rows.Count > 0)
                {
                    idCardListModel.Addr1 = dt1.Rows[0]["Addr1"].ToString().TrimEnd().TrimStart();
                    idCardListModel.Addr2 = dt1.Rows[0]["Addr2"].ToString().TrimEnd().TrimStart();
                    idCardListModel.City = dt1.Rows[0]["City"].ToString().TrimEnd().TrimStart();
                    idCardListModel.MemberId = dt1.Rows[0]["Id"].ToString();
                    idCardListModel.State = dt1.Rows[0]["State"].ToString().TrimEnd().TrimStart();
                    idCardListModel.Zip1 = dt1.Rows[0]["Zip1"].ToString().TrimEnd().TrimStart();
                }


                idCardListModels.Add(idCardListModel);

            }
        }

        private DataSet IdcardrequestListDataset(int page)
        {
            DataSet ds = new DataSet();
            string commandtext = "GetIdCardRequestList";
            var parameters = new List<IDbDataParameter>();
            parameters.Add(db.CreateParameter("@Requester", User.Identity.Name, DbType.String));
            parameters.Add(db.CreateParameter("@page", page, DbType.Int16));
            parameters.Add(db.CreateParameter("@size", 10, DbType.Int16));
            ds = db.GetDataSet(commandtext, CommandType.StoredProcedure, parameters.ToArray());
            return ds;
        }

        public JsonResult GetIdCardRequestPartialViewHtml(int page)
        {
            List<IdCardListModel> idCardListModels = new List<IdCardListModel>();
            string viewContent = "";
            int TotalCount = 0;
            try
            {

                DataSet ds = IdcardrequestListDataset(page);

                IdCardRequestDatatableToList(idCardListModels, ds);

                viewContent = ConvertViewToString("_IdCardTableListPartilaView", idCardListModels);
                TotalCount = (int)ds.Tables[0].Rows[0]["Count"];
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
        public JsonResult IdCardComplete(long Id, string Notes)
        {
            try
            {
                DBManager db = new DBManager("CustomerServicePortal");
                string Commandtext = "Fill_IDCardRequestDetails";
                var parameters = new List<IDbDataParameter>();
                parameters.Add(db.CreateParameter("@Id", Id, DbType.String));
                parameters.Add(db.CreateParameter("@CompleteNotes", Notes, DbType.String));
             
                db.Insert(Commandtext, CommandType.StoredProcedure, parameters.ToArray());

            }
            catch (Exception EX)
            {
                return Json(false, JsonRequestBehavior.AllowGet);

            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}