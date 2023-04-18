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

                //Finding & Getting UserName by SessionID
                var userName = unitOfWork.UserRepository.GetAll().Where(u => u.UserID == int.Parse(Session["UserID"].ToString()));
                var fullName = new UserModel()
                {
                    FirstName = userName.First().FirstName,
                    LastName = userName.Last().LastName,
                };

                ViewBag.User = fullName;

                ////Getting all the Appointments of particular Patient.
                try
                {
                    var currentAppointments = (from a in unitOfWork.AppointmentRepository.GetAll()
                                               join p in unitOfWork.PatientRepository.GetAll() on a.PatientID equals p.PatientID
                                               join d in unitOfWork.DoctorRepository.GetAll() on a.DoctorID equals d.DoctorID
                                               where p.UserID == int.Parse(Session["UserID"].ToString()) &&
                                               a.Status == AppointmentStatus.Accepted.ToString() &&
                                               a.Appointment_DateTime == DateTime.Today
                                               select new GetCurrentAppointments
                                               {
                                                   Title = a.Title,
                                                   DoctorID = (int)d.UserID,
                                                   CreatedOn = a.CreatedOn.ToString(),
                                                   Appointment_DateTime = a.Appointment_DateTime.ToString()
                                               }).ToList();

                    ViewBag.CurrentAppointments = currentAppointments;

                    //Getting all appointments, including accepted & declined
                    var allAppointments = (from appointmentTable in unitOfWork.AppointmentRepository.GetAll()
                                           join p in unitOfWork.PatientRepository.GetAll() on appointmentTable.PatientID equals p.PatientID
                                           join d in unitOfWork.DoctorRepository.GetAll() on appointmentTable.DoctorID equals d.DoctorID
                                           where p.UserID == int.Parse(Session["UserID"].ToString()) &&
                                           appointmentTable.Appointment_DateTime >= DateTime.Today
                                           select new GetCurrentAppointments
                                           {
                                               Title = appointmentTable.Title,
                                               DoctorID = (int)d.UserID,
                                               CreatedOn = appointmentTable.CreatedOn.ToString(),
                                               Appointment_DateTime = appointmentTable.Appointment_DateTime.ToString()
                                           });

                    ViewBag.AllAppointments = allAppointments;


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


            // Show only completed appointments
            var completedAppointments = (from appointment in unitOfWork.AppointmentRepository.GetAll()
                                         join prescription in unitOfWork.PrescriptionRepository.GetAll() on appointment.AppointmentID equals prescription.AppointmentID
                                         join patient in unitOfWork.PatientRepository.GetAll() on appointment.PatientID equals patient.PatientID
                                         where patient.UserID == int.Parse(Session["UserID"].ToString())
                                         select new
                                         {
                                             appointment.Title,
                                             prescription.Medicines,
                                             prescription.Usage,
                                             prescription.EndDate
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


        public JsonResult Payments(PaymentsModel payments)
        {

            return Json(JsonRequestBehavior.AllowGet);
        }

    }
}