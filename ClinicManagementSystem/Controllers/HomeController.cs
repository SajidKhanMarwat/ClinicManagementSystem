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
                           select new
                           {
                               user.FirstName,
                               user.LastName,
                               doctor.Specialization,
                               doctor.Education,
                               doctor.Experience,
                               doctor.Fees
                           }).ToList();

            //AvailableDoctorsModel doctorsModel = new AvailableDoctorsModel()
            //{
            //    FirstName = doctors.First().FirstName,
            //    LastName = doctors.First().LastName,
            //    Specialization = doctors.First().Specialization,
            //    Education = doctors.First().Education,
            //    Experience = (int)doctors.First().Experience,
            //    Fees = (int)doctors.First().Fees
            //};

            //ViewBag.Doctors = doctors;

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