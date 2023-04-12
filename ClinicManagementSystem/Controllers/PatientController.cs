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
        Completed,
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
                try
                {
                    var currentAppointments = (from a in unitOfWork.AppointmentRepository.GetAll()
                                               join p in unitOfWork.PatientRepository.GetAll() on a.PatientID equals p.PatientID
                                               where p.UserID == uID && a.Status == Status.Accepted.ToString()
                                               select new GetCurrentAppointments
                                               {
                                                   Title = a.Title,
                                                   DoctorID = (int)a.DoctorID,
                                                   CreatedOn = a.CreatedOn.ToString(),
                                                   Appointment_DateTime = a.Appointment_DateTime.ToString()
                                               }).ToList();

                    ViewBag.CurrentAppointments = currentAppointments;
                    return View();
                }
                catch (Exception)
                {
                    return View();
                }

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }


        //View Appointmnts Json Format
        public JsonResult MyAppointments()
        {
            // Showing all the appointments except the 'Declined Appointments'
            var AllAppointments = (from appointment in unitOfWork.AppointmentRepository.GetAll()
                                join doctor in unitOfWork.DoctorRepository.GetAll() on appointment.DoctorID equals doctor.DoctorID
                                join user in unitOfWork.UserRepository.GetAll() on doctor.UserID equals user.UserID
                                where user.UserID == uID && 
                                appointment.Status == Status.Accepted.ToString() ||
                                appointment.Status == Status.Completed.ToString() ||
                                appointment.Status == Status.Pending.ToString()
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
            //Getting DoctorID by comparing the UserID of particular Doctor in Doctor Table
            var doctor = unitOfWork.DoctorRepository.GetAll().Where(u => u.UserID == model.DoctorID).FirstOrDefault();
            //Get Patient id as a User
            var patient = unitOfWork.PatientRepository.GetAll().Where(u => u.UserID == int.Parse(Session["UserID"].ToString())).FirstOrDefault();


            var newAppointment = new Appointment()
            {
                Title = model.Title,
                Description = model.Description,
                FeesPaid = model.FeesPaid,
                Appointment_DateTime = model.Appointment_DateTime,
                PatientHistory = model.PatientHistory,
                Status = Status.Pending.ToString(),
                DoctorID = doctor.DoctorID,
                PatientID = patient.PatientID,
            };

            unitOfWork.AppointmentRepository.AddNew(newAppointment);

            return Json(JsonRequestBehavior.AllowGet);
        }


    }
}