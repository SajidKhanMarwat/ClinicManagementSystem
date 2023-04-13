using ClinicManagementSystem.Models;
using ClinicManagementSystem.Repository;
using ClinicManagementSystem.Repository.EntityModel;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ClinicManagementSystem.Controllers
{    
    public enum AppointmentStatus
    {
        Pending,
        Accepted,
        Completed,
        Declined
    }

    [Authorize]
    public class PatientController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();


        // GET: Patient
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {

                ////Getting all the Appointments of particular Patient.
                try
                {
                    var currentAppointments = (from a in unitOfWork.AppointmentRepository.GetAll()
                                               join p in unitOfWork.PatientRepository.GetAll() on a.PatientID equals p.PatientID
                                               where p.UserID == int.Parse(Session["UserID"].ToString()) && a.Status == AppointmentStatus.Accepted.ToString()
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
            //// Showing all the appointments except the 'Declined Appointments'
            //var AllAppointments = (from appointment in unitOfWork.AppointmentRepository.GetAll()
            //                    join doctor in unitOfWork.DoctorRepository.GetAll() on appointment.DoctorID equals doctor.DoctorID
            //                    join patient in unitOfWork.PatientRepository.GetAll() on appointment.PatientID equals patient.PatientID
            //                    where appointment.PatientID == patient.PatientID &&
            //                    patient.UserID == int.Parse(Session["UserID"].ToString())
            //                       select new
            //                       {
            //                           appointment.Title,
            //                           doctor.UserID,
            //                           appointment.CreatedOn
            //                       }).ToList();



            var completedAppointments = (from appointment in unitOfWork.AppointmentRepository.GetAll()
                                         join prescription in unitOfWork.PrescriptionRepository.GetAll() on appointment.AppointmentID equals prescription.AppointmentID
                                         join patient in unitOfWork.PatientRepository.GetAll() on appointment.PatientID equals patient.PatientID
                                         join user in unitOfWork.UserRepository.GetAll() on patient.UserID equals user.UserID
                                         where appointment.PatientID == patient.PatientID
                                         select new
                                         {
                                             appointment.Title,
                                             prescription.Medicines,
                                             prescription.Usage
                                         }).ToList();

            return Json(completedAppointments, JsonRequestBehavior.AllowGet);
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
                Status = AppointmentStatus.Pending.ToString(),
                DoctorID = doctor.DoctorID,
                PatientID = patient.PatientID,
            };

            unitOfWork.AppointmentRepository.AddNew(newAppointment);

            return Json(JsonRequestBehavior.AllowGet);
        }


    }
}