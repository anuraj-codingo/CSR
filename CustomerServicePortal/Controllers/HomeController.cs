using CustomerServicePortal.Auth;
using CustomerServicePortal.DAL;
using CustomerServicePortal.Models;
using System;
using System.Web.Mvc;
using System.Web.UI;

//using IBM.Data.DB2.iSeries;

namespace CustomerServicePortal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
     
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult LoadFooterHtml()
        {
            try
            {
                if (Session["LayoutDetails"] == null)
                {
                    LayoutModel layoutModel = new LayoutModel();
                    layoutModel = GetLayoutSessionClass.GetLayoutModel();
                    Session["LayoutDetails"] = layoutModel;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return PartialView("_LayoutFooterPartialView", (LayoutModel)Session["LayoutDetails"]);
        }

        public ActionResult LoadHeaderHtml()
        {
            try
            {
                if (Session["LayoutDetails"] == null)
                {
                    LayoutModel layoutModel = new LayoutModel();
                    layoutModel = GetLayoutSessionClass.GetLayoutModel();
                    Session["LayoutDetails"] = layoutModel;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_layoutHeaderPartialView", (LayoutModel)Session["LayoutDetails"]);
        }
    }
}