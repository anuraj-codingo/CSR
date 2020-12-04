using CustomerServicePortal.DAL;
using CustomerServicePortal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomerServicePortal.Controllers
{
    public class IdCardListController : Controller
    {
        private DBManager db = new DBManager("CustomerServicePortal");
        // GET: IdCardList
        public ActionResult Index()
        {
            List<IdCardListModel> idCardListModels = new List<IdCardListModel>();
            try
            {
                DataTable dt = new DataTable();
                string CommandText = "select * from [IDCardRequestDetails] where Requester = '"+User.Identity.Name+"'";
                dt = db.GetDataTable(CommandText,CommandType.Text);

            

                foreach (DataRow item in dt.Rows)
                {
                    IdCardListModel idCardListModel = new IdCardListModel();
                    idCardListModel.EMSSN = (decimal)item["EMSSN"];
                    idCardListModel.Name = item["Name"].ToString();
                    idCardListModel.Gender = item["Gender"].ToString();
                    idCardListModels.Add(idCardListModel);

                }

            }
            catch (Exception)
            {

                throw;
            }
            return View(idCardListModels);
        }
    }
}