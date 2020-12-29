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
                ((LayoutModel)Session["LayoutDetails"]).Client = Client;
                Session["Client"] = Client;
            }

            if (((LayoutModel)Session["LayoutDetails"]).Client != null || ((LayoutModel)Session["LayoutDetails"]).Client !="")
            {
                LayoutModel layoutModel = new LayoutModel();
                layoutModel = GetLayoutSessionClass.GetLayoutModel(((LayoutModel)Session["LayoutDetails"]).Client);
                Session["LayoutDetails"] = layoutModel;

            }
            else
            {
                ((LayoutModel)Session["LayoutDetails"]).Client = "ABC"; 
                Session["Client"] = "ABC";
                LayoutModel layoutModel = new LayoutModel();
                layoutModel = GetLayoutSessionClass.GetLayoutModel(((LayoutModel)Session["LayoutDetails"]).Client);
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
            try
            {
                LayoutModel layoutModel = new LayoutModel();

                layoutModel = GetLayoutSessionClass.GetLayoutModel(((LayoutModel)Session["LayoutDetails"]).Client);
                Session["LayoutDetails"] = layoutModel;

            }
            catch (Exception)
            {

                LayoutModel layoutModel = new LayoutModel();

                layoutModel = GetLayoutSessionClass.GetLayoutModel(null);
                Session["LayoutDetails"] = layoutModel;

            }




        }
       
    }
}