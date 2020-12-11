using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomerServicePortal.Controllers
{
    [Authorize]
    public class ABCDashBoardController : Controller
    {
        // GET: ABC_UserDashBoard
        public ActionResult Index()
        {
            return View();
        }
    }
}