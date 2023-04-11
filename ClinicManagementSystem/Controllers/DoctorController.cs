using ClinicManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace ClinicManagementSystem.Controllers
{
    public class DoctorController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();
        int uID;
        // GET: Doctor
        public ActionResult Index()
        {
            uID = int.Parse(Session["UserID"].ToString());
            return View();
        }
    }
}