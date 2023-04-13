using ClinicManagementSystem.Models;
using ClinicManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace ClinicManagementSystem.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET: Admin
        public ActionResult Index()
        {
            var totalUsers = unitOfWork.UserRepository.GetAll();
            ViewBag.All = totalUsers;

            int doctorsCount = unitOfWork.DoctorRepository.GetAll()
                .Where(d => totalUsers
                .Any(u => u.UserID == d.UserID))
                .Count();
            ViewBag.totalDoctors = doctorsCount;


            int patientsCount = unitOfWork.PatientRepository.GetAll()
                .Where(p => totalUsers
                .Any(u => u.UserID == p.UserID))
                .Count();
            ViewBag.TotalPatients = patientsCount;


            int appointmentsCount = unitOfWork.AppointmentRepository.GetAll()
                .Where(p => totalUsers
                .Any(u => u.UserID == p.AppointmentID))
                .Count();
            ViewBag.TotalPatients = appointmentsCount;


            return View();
        }

        [HttpPost]
        public ActionResult Index(int id)
        {
            unitOfWork.UserRepository.Delete(id);            
            return Index();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult UserProfile()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult UserProfile(int id)
        {

            var userDetails = unitOfWork.UserRepository.GetById(id);
            if(userDetails == null)
            {
                return RedirectToAction("MissingPage", "NotFound");
            }
            return View(userDetails);
        }
    }
}