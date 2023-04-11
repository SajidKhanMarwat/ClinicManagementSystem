using ClinicManagementSystem.Models;
using ClinicManagementSystem.Repository;
using ClinicManagementSystem.Repository.EntityModel;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using System;
using System.IdentityModel.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

namespace ClinicManagementSystem.Controllers
{    
    enum Status
    {
        Pending,
        Accepted,
        Declined
    }

    
    public class PatientController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();
        int uID;



        // GET: Patient
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                uID = int.Parse(Session["UserID"].ToString());

                ////Getting all the Appointments of particular Patient.
                var currentAppointments = (from a in unitOfWork.AppointmentRepository.GetAll()
                                           join p in unitOfWork.PatientRepository.GetAll() on a.PatientID equals p.PatientID
                                           where p.UserID == uID
                                           select new
                                           {
                                               a.Title,
                                               a.DoctorID,
                                               a.CreatedOn,
                                               a.Appointment_DateTime
                                           }).ToList();

                ViewBag.CurrentAppointments = currentAppointments;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }


        //View Appointmnts Json Format
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



        //Doctor Dropdown List (ajax)
        public JsonResult DoctorDDL()
        {
            var availableDoctors = (from doctor in unitOfWork.DoctorRepository.GetAll()
                                    join user in unitOfWork.UserRepository.GetAll() on doctor.UserID equals user.UserID
                                    select new
                                    {
                                        doctor.UserID,
                                        user.FirstName,
                                        user.LastName,
                                        doctor.Specialization,
                                        doctor.Education
                                    }
                                    ).ToList();

            return Json(availableDoctors, JsonRequestBehavior.AllowGet);
        }


        //Create Appointment
        public JsonResult CreateAppointment(AppointmentsDetails model)
        {

            //Get Patient id as a User
            var newAppointment = new Appointment()
            {
                Title = model.Title,
                Description = model.Description,
                FeesPaid = model.FeesPaid,
                Appointment_DateTime = model.Appointment_DateTime,
                PatientHistory = model.PatientHistory,
                Status = Status.Pending.ToString(),
                DoctorID = model.DoctorID,
                PatientID = unitOfWork.PatientRepository.GetAll().Select(i => i.UserID).First(),
            };

            unitOfWork.AppointmentRepository.AddNew(newAppointment);

            return Json(JsonRequestBehavior.AllowGet);
        }


    }
}