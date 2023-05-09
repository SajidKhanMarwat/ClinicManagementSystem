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
                    var todayAppointments = (from a in unitOfWork.AppointmentRepository.GetAll()
                                               join p in unitOfWork.PatientRepository.GetAll() on a.PatientID equals p.PatientID
                                               join d in unitOfWork.DoctorRepository.GetAll() on a.DoctorID equals d.DoctorID
                                               where p.UserID == int.Parse(Session["UserID"].ToString()) &&
                                               a.Status == AppointmentStatus.Accepted.ToString() &&
                                               a.Appointment_DateTime >= DateTime.Now
                                               select new GetCurrentAppointments
                                               {
                                                   Title = a.Title,
                                                   DoctorID = (int)d.UserID,
                                                   CreatedOn = a.CreatedOn.ToString(),
                                                   Appointment_DateTime = a.Appointment_DateTime.ToString(),
                                                   Appointment_Status = a.Status
                                               }).ToList();

                    //ViewBag.TodayAppointments = todayAppointments;

                    return View(todayAppointments);
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

        public ActionResult AllAppointments()
        {
            //Finding & Getting UserName by SessionID
            var userName = unitOfWork.UserRepository.GetAll().Where(u => u.UserID == int.Parse(Session["UserID"].ToString()));
            var fullName = new UserModel()
            {
                FirstName = userName.First().FirstName,
                LastName = userName.Last().LastName,
            };

            ViewBag.User = fullName;


            if (Request.IsAuthenticated)
            {
                //Getting all appointments, including accepted & declined
                var allAppointments = (from appointmentTable in unitOfWork.AppointmentRepository.GetAll()
                                       join p in unitOfWork.PatientRepository.GetAll() on appointmentTable.PatientID equals p.PatientID
                                       join d in unitOfWork.DoctorRepository.GetAll() on appointmentTable.DoctorID equals d.DoctorID
                                       where p.UserID == int.Parse(Session["UserID"].ToString())
                                       select new GetCurrentAppointments
                                       {
                                           Title = appointmentTable.Title,
                                           DoctorID = (int)d.UserID,
                                           CreatedOn = appointmentTable.CreatedOn.ToString(),
                                           Appointment_DateTime = appointmentTable.Appointment_DateTime.ToString(),
                                           Appointment_Status = appointmentTable.Status
                                       }).ToList();

                //ViewBag.AllAppointments = allAppointments;
                
                return View(allAppointments);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult AllPrescriptions()
        {
            //Finding & Getting UserName by SessionID
            var userName = unitOfWork.UserRepository.GetAll().Where(u => u.UserID == int.Parse(Session["UserID"].ToString()));
            var fullName = new UserModel()
            {
                FirstName = userName.First().FirstName,
                LastName = userName.Last().LastName,
            };

            ViewBag.User = fullName;


            if (Request.IsAuthenticated)
            {
                var completedAppointmentsPrescriptions = (from appointment in unitOfWork.AppointmentRepository.GetAll()
                                             join prescription in unitOfWork.PrescriptionRepository.GetAll() on appointment.AppointmentID equals prescription.AppointmentID
                                             join patient in unitOfWork.PatientRepository.GetAll() on appointment.PatientID equals patient.PatientID
                                             where patient.UserID == int.Parse(Session["UserID"].ToString())
                                             select new PrescriptionsCompleted
                                             {
                                                 Title = appointment.Title,
                                                 Medicines = prescription.Medicines,
                                                 Usage = prescription.Usage,
                                                 EndDate = (DateTime)prescription.EndDate,
                                             }).ToList();

                //ViewBag.CompletedAppointmentsPrescriptions = completedAppointmentsPrescriptions;


                return View(completedAppointmentsPrescriptions);
            }
            else
            {
                return RedirectToAction("Login","Account");
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
                                    where user.IsDeleted == false &&
                                    doctor.UserID == user.UserID &&
                                    user.RoleID == 2
                                    select new
                                    {
                                        doctor.UserID,
                                        doctor.Fees,
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
            try
            {
                //Getting DoctorID by comparing the UserID of particular Doctor in Doctor Table
                var doctor = unitOfWork.DoctorRepository.GetAll().Where(d => d.UserID == model.DoctorID).FirstOrDefault();
                //Get Patient by session
                var patient = unitOfWork.PatientRepository.GetAll().Where(u => u.UserID == int.Parse(Session["UserID"].ToString())).FirstOrDefault();

                if (doctor.Fees == model.FeesPaid)
                {
                    var newAppointment = new Appointment()
                    {
                        Title = model.Title,
                        Description = model.Description,
                        FeesPaid = model.FeesPaid,
                        Appointment_DateTime = model.Appointment_DateTime,
                        Status = AppointmentStatus.Pending.ToString(),
                        DoctorID = doctor.DoctorID,
                        PatientID = patient.PatientID,
                        CreatedOn = DateTime.Today,
                        CreatedBy = string.Empty ?? Session["FirstName"].ToString() +" "+ Session["LastName"].ToString() //Null Coalesing
                    };

                    unitOfWork.AppointmentRepository.AddNew(newAppointment);

                    var newPayment = new Payment()
                    {
                        AppointmentID = newAppointment.AppointmentID,
                        CardNumber = (int?)model.CardNumber,
                        Amount = newAppointment.FeesPaid,
                        Date = newAppointment.CreatedOn,
                        IsDeleted = false,
                        CreatedBy = string.Empty ?? Session["FirstName"].ToString() + " " + Session["LastName"].ToString() //Null Coalesing
                    };

                    unitOfWork.PaymentRepository.AddNew(newPayment);
                    return Json(JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null);
                }
            }
            catch (Exception)
            {
                return Json(null);
            }            
        }


        public JsonResult Payments(PaymentsModel payments)
        {

            return Json(JsonRequestBehavior.AllowGet);
        }

    }
}