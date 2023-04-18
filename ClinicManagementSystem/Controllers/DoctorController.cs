using ClinicManagementSystem.Models;
using ClinicManagementSystem.Repository;
using ClinicManagementSystem.Repository.EntityModel;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace ClinicManagementSystem.Controllers
{

    enum DoctorAppointmentStatus
    {
        Pending,
        Accepted,
        Completed,
        Declined
    }
    [Authorize]
    public class DoctorController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();
        
        // GET: Doctor
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                //Finding & Getting UserName by SessionID
                var userName = unitOfWork.UserRepository.GetAll().Where(u => u.UserID == int.Parse(Session["UserID"].ToString()));
                var fullName = new UserModel()
                {
                    FirstName = userName.First().FirstName,
                    LastName = userName.Last().LastName,
                };

                ViewBag.User = fullName;

                //Appointments Count
                var totalAppointments = (from appointments in unitOfWork.AppointmentRepository.GetAll()
                                         join doctor in unitOfWork.DoctorRepository.GetAll() on appointments.DoctorID equals doctor.DoctorID
                                         where doctor.UserID == int.Parse(Session["UserID"].ToString()) &&
                                         appointments.Appointment_DateTime > DateTime.Today &&
                                         appointments.Status == DoctorAppointmentStatus.Pending.ToString() ||
                                         appointments.Status == DoctorAppointmentStatus.Accepted.ToString()
                                         select new
                                         {
                                             appointments.Title
                                         }).Count();

                ViewBag.TotalAppointments = totalAppointments;
                try
                {

                    // New Appointment Request
                    var newAppointments = (from appointments in unitOfWork.AppointmentRepository.GetAll()
                                           join doctor in unitOfWork.DoctorRepository.GetAll() on appointments.DoctorID equals doctor.DoctorID
                                           join patient in unitOfWork.PatientRepository.GetAll() on appointments.PatientID equals patient.PatientID
                                           join user in unitOfWork.UserRepository.GetAll() on patient.UserID equals user.UserID //Used for getting user Name
                                           where appointments.DoctorID == doctor.DoctorID &&
                                           doctor.UserID == int.Parse(Session["UserID"].ToString()) &&
                                           patient.PatientID == appointments.PatientID &&
                                           (
                                           appointments.Status == AppointmentStatus.Pending.ToString()
                                           )
                                           select new CurrentAppointmentsDoc
                                           {
                                               ApID = appointments.AppointmentID,
                                               FirstName = user.FirstName,
                                               LastName = user.LastName,
                                               Title = appointments.Title,
                                               Description = appointments.Description,
                                               PatientHistory = appointments.PatientHistory,
                                               Appointment_DateTime = appointments.Appointment_DateTime.ToString(),
                                           }).ToList();
                    ViewBag.NewAppointments = newAppointments; // New Appointments that are 'Pending'.


                    // Current appointments
                    var todayAppointments = (from appointments in unitOfWork.AppointmentRepository.GetAll()
                                               join doctor in unitOfWork.DoctorRepository.GetAll() on appointments.DoctorID equals doctor.DoctorID
                                               join patient in unitOfWork.PatientRepository.GetAll() on appointments.PatientID equals patient.PatientID
                                               join user in unitOfWork.UserRepository.GetAll() on patient.UserID equals user.UserID //Used for getting user Name
                                               where appointments.DoctorID == doctor.DoctorID &&
                                               doctor.UserID == int.Parse(Session["UserID"].ToString()) &&
                                               appointments.Appointment_DateTime == DateTime.Today &&
                                               patient.PatientID == appointments.PatientID &&
                                               (
                                               appointments.Status == AppointmentStatus.Accepted.ToString()
                                               )
                                               select new CurrentAppointmentsDoc
                                               {                                                   
                                                   ApStatus = appointments.Status,
                                                   FirstName = user.FirstName,
                                                   LastName = user.LastName,
                                                   Title = appointments.Title,
                                                   Description = appointments.Description,
                                                   PatientHistory = appointments.PatientHistory,
                                                   PatientID = (int)patient.UserID,
                                                   Appointment_DateTime = appointments.Appointment_DateTime.ToString()
                                               }).ToList();


                    ViewBag.TodayAppointments = todayAppointments; //Sending today's Appointments to View

                    return View();
                }
                catch (Exception)
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        //Status Update Here
        public void UpdateAppointment(Appointment appointment)
        {
            try
            {
                Appointment statusUpdate = new Appointment()
                {
                    Status = appointment.Status,
                };
                unitOfWork.AppointmentRepository.Update(statusUpdate);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public JsonResult Accepted(int id)
        {
            var appointment = unitOfWork.AppointmentRepository.GetById(id);

            if (appointment.AppointmentID != 0)
            {
                Appointment appointmentObj = new Appointment()
                {
                    AppointmentID = appointment.AppointmentID,
                    PatientID = appointment.PatientID,
                    DoctorID = appointment.DoctorID,
                    FeesPaid = appointment.FeesPaid,
                    Appointment_DateTime = appointment.Appointment_DateTime,
                    Title = appointment.Title,
                    Description = appointment.Description,
                    PatientHistory = appointment.PatientHistory,
                    IsDeleted = appointment.IsDeleted,
                    CreatedOn = appointment.CreatedOn,
                    CreatedBy = appointment.CreatedBy,
                    UpdatedOn = DateTime.Now,
                    UpdatedBy = appointment.UpdatedBy,
                    Status = DoctorAppointmentStatus.Accepted.ToString()
                };
                unitOfWork.AppointmentRepository.Update(appointmentObj);
                return Json(JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(JsonRequestBehavior.AllowGet);
            }
            
        }

        public JsonResult Declined(int id)
        {
            var appointment = unitOfWork.AppointmentRepository.GetById(id);

            if (appointment.AppointmentID != 0)
            {
                Appointment appointmentObj = new Appointment()
                {
                    AppointmentID = appointment.AppointmentID,
                    PatientID = appointment.PatientID,
                    DoctorID = appointment.DoctorID,
                    FeesPaid = appointment.FeesPaid,
                    Appointment_DateTime = appointment.Appointment_DateTime,
                    Title = appointment.Title,
                    Description = appointment.Description,
                    PatientHistory = appointment.PatientHistory,
                    IsDeleted = appointment.IsDeleted,
                    CreatedOn = appointment.CreatedOn,
                    CreatedBy = appointment.CreatedBy,
                    UpdatedOn = DateTime.Now,
                    UpdatedBy = appointment.UpdatedBy,
                    Status = DoctorAppointmentStatus.Declined.ToString()
                };
                unitOfWork.AppointmentRepository.Update(appointmentObj);
                return Json(JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(JsonRequestBehavior.AllowGet);
            }
        }

        //Appointment Completed
        public JsonResult Completed(int id)
        {
            var appointment = unitOfWork.AppointmentRepository.GetById(id);

            if (appointment.AppointmentID != 0)
            {
                Appointment appointmentObj = new Appointment()
                {
                    AppointmentID = appointment.AppointmentID,
                    PatientID = appointment.PatientID,
                    DoctorID = appointment.DoctorID,
                    FeesPaid = appointment.FeesPaid,
                    Appointment_DateTime = appointment.Appointment_DateTime,
                    Title = appointment.Title,
                    Description = appointment.Description,
                    PatientHistory = appointment.PatientHistory,
                    IsDeleted = appointment.IsDeleted,
                    CreatedOn = appointment.CreatedOn,
                    CreatedBy = appointment.CreatedBy,
                    UpdatedOn = DateTime.Now,
                    UpdatedBy = appointment.UpdatedBy,
                    Status = DoctorAppointmentStatus.Completed.ToString()
                };
                unitOfWork.AppointmentRepository.Update(appointmentObj);
                return Json(JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Prescription(CreatePrescription prescription)
        {

            Prescription prescriptionEntry = new Prescription()
            {
                AppointmentID = prescription.AppointmentID,
                Medicines = prescription.Medicines,
                Usage = prescription.Usage,
                StartDate = prescription.StartDate,
                EndDate = prescription.EndDate,
            };

            unitOfWork.PrescriptionRepository.AddNew(prescriptionEntry);

            return Json(JsonRequestBehavior.AllowGet);
        }
    }
}