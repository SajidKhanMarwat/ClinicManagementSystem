using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicManagementSystem.Controllers
{
    public class NotFoundController : Controller
    {
        // GET: NotFound
        [HttpGet]
        public ActionResult MissingPage()
        {
            return View();
        }
    }
}