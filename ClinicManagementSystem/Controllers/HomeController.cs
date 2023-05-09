using ClinicManagementSystem.Models;
using ClinicManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private UnitOfWork _UnitOfWork = new UnitOfWork();

        public ActionResult Index()
        {
            var doctors = (from doctor in _UnitOfWork.DoctorRepository.GetAll()
                           join user in _UnitOfWork.UserRepository.GetAll() on doctor.UserID equals user.UserID
                           where doctor.IsDeleted == false
                           select new AvailableDoctorsModel()
                           {
                               FirstName = user.FirstName,
                               LastName = user.LastName,
                               Specialization = doctor.Specialization,
                               Education = doctor.Education,
                               Experience = doctor.Experience ?? 0,
                               Fees = doctor.Fees ?? 0
                           }).ToList();

            ViewBag.Doctors = doctors;

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}