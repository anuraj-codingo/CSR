using CustomerServicePortal.Auth;
using CustomerServicePortal.DAL;
using CustomerServicePortal.Models;
using System;
using System.Web.Mvc;

//using IBM.Data.DB2.iSeries;

namespace CustomerServicePortal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
     
        public ActionResult Index(string Client)
        {
            if (Client !=null)
            {
                Session["LayoutDetails"]=new LayoutModel();
                Session["Client"] = Client;
            }

            if (Session["Client"]!=null)
            {
                LayoutModel layoutModel = new LayoutModel();
                layoutModel = GetLayoutSessionClass.GetLayoutModel((string)Session["Client"]);
                Session["LayoutDetails"] = layoutModel;

            }
            else
            {
                Session["Client"] = "ABC";
                LayoutModel layoutModel = new LayoutModel();
                layoutModel = GetLayoutSessionClass.GetLayoutModel((string)Session["Client"]);
                Session["LayoutDetails"] = layoutModel;
            }

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
                LoadlayoutModel();
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
                LoadlayoutModel();
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_layoutHeaderPartialView", (LayoutModel)Session["LayoutDetails"]);
        }

        private void LoadlayoutModel()
        {
          
                LayoutModel layoutModel = new LayoutModel();
          
                    layoutModel = GetLayoutSessionClass.GetLayoutModel((string)Session["Client"]);
                    Session["LayoutDetails"] = layoutModel;
          


            
        }
    }
}