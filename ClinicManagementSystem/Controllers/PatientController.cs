using ClinicManagementSystem.Models;
using ClinicManagementSystem.Repository;
using ClinicManagementSystem.Repository.EntityModel;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ClinicManagementSystem.Controllers
{    
    enum Status
    {
        Pending,
        Accepted,
        Declined
    }

    [Authorize]
    public class PatientController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();
        int currentUserID;

        // GET: Patient
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                // Getting all the Appointments of particular Patient.
                var currentAppointments = (from u in unitOfWork.PatientRepository.GetAll()
                                           join r in unitOfWork.AppointmentRepository.GetAll() on u.UserID equals r.PatientID
                                           select new
                                           {
                                               u.UserID,
                                               r.Title,
                                               r.DoctorID,
                                               r.CreatedOn,
                                               r.Appointment_DateTime
                                           }).ToList();


                ViewBag.CurrentAppointments = currentAppointments;

                currentUserID = currentAppointments.FirstOrDefault().UserID.Value;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(AppointmentsDetails model)
        {
            if (Request.IsAuthenticated)
            {
                var db = System.Web.HttpContext.Current.User.Identity.GetUserId();
                //Get Patient id as a User
                var uID = User.Identity.GetUserId();

                var newAppointment = new Appointment()
                {
                    Title = model.Title,
                    Description = model.Description,
                    FeesPaid = model.FeesPaid,
                    Appointment_DateTime = model.Appointment_DateTime,
                    PatientHistory = model.PatientHistory,
                    Status = Status.Pending.ToString(),
                    PatientID = unitOfWork.PatientRepository.GetAll().Select(i => i.UserID).First(),
                };

                unitOfWork.AppointmentRepository.AddNew(newAppointment);
            }
            return View();
        }

        public JsonResult MyAppointments()
        {
            var AllAppointments = (from appointment in unitOfWork.AppointmentRepository.GetAll()
                                join doctor in unitOfWork.DoctorRepository.GetAll() on appointment.DoctorID equals doctor.DoctorID
                                join user in unitOfWork.UserRepository.GetAll() on doctor.UserID equals user.UserID
                                select new
                                {
                                    appointment.Title,
                                    user.UserID,
                                    appointment.CreatedOn,
                                }).ToList();
            return Json(AllAppointments, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DoctorDDL()
        {
            var availableDoctors = (from doctor in unitOfWork.DoctorRepository.GetAll()
                                    join user in unitOfWork.UserRepository.GetAll() on doctor.UserID equals user.UserID
                                    select new
                                    {
                                        user.FirstName,
                                        user.LastName,
                                        doctor.Specialization,
                                        doctor.Education
                                    }
                                    ).ToList();

            return Json(availableDoctors, JsonRequestBehavior.AllowGet);
        }
    }
}