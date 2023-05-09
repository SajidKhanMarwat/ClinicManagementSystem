using ClinicManagementSystem.Models;
using ClinicManagementSystem.Repository;
using ClinicManagementSystem.Repository.EntityModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
            if (Request.IsAuthenticated)
            {
                // All Users
                //var users = unitOfWork.UserRepository.GetAll().Where(u => u.IsDeleted == false).ToList();

                // Sending users to view using UserModel
                var users = (from u in unitOfWork.UserRepository.GetAll()
                             join d in unitOfWork.DoctorRepository.GetAll() on u.UserID equals d.UserID into doctorGroup
                             from doctor in doctorGroup.DefaultIfEmpty()
                             join p in unitOfWork.PatientRepository.GetAll() on u.UserID equals p.UserID into patientGroup
                             from patient in patientGroup.DefaultIfEmpty()
                             where u.IsDeleted == false && (doctor != null || patient != null)
                             select new UserModel()
                             {
                                 FirstName = u.FirstName,
                                 LastName = u.LastName,
                                 Email = u.Email,
                                 Phone = (int)u.Phone,
                                 Gender = u.Gender,
                                 Age = u.Age ?? DateTime.MinValue,
                                 CreatedOn = u.CreatedOn ?? DateTime.MinValue,
                                 UserID = u.UserID
                             }).ToList();


                //ViewBag.AllUsers = users;


                int doctorsCount = (from user in unitOfWork.UserRepository.GetAll().Where(u => u.IsDeleted == false && u.RoleID == 2)
                                    join doctors in unitOfWork.DoctorRepository.GetAll() on user.UserID equals doctors.UserID
                                    where doctors.UserID == user.UserID &&
                                    user.IsDeleted == false
                                    select new
                                    {
                                        user.UserID
                                    }
                                   ).Count();
                ViewBag.totalDoctors = doctorsCount;


                // All Patients
                int patientsCount = (from user in unitOfWork.UserRepository.GetAll().Where(u => u.IsDeleted == false && u.RoleID == 3)
                                     join patients in unitOfWork.PatientRepository.GetAll() on user.UserID equals patients.UserID
                                     where patients.UserID == user.UserID &&
                                     user.IsDeleted == false
                                     select new
                                     {
                                         user.UserID
                                     }
                                   ).Count();
                ViewBag.TotalPatients = patientsCount;


                // All Appointments
                int appointmentsCount = unitOfWork.AppointmentRepository.GetAll().Where(i => i.IsDeleted == false)
                    .Count();
                ViewBag.TotalAppointments = appointmentsCount;


                return View(users);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
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
            //Finding & Getting UserName by SessionID
            var userName = unitOfWork.UserRepository.GetAll().Where(u => u.UserID == int.Parse(Session["UserID"].ToString()));
            var fullName = new UserModel()
            {
                FirstName = userName.First().FirstName,
                LastName = userName.Last().LastName,
            };

            ViewBag.User = fullName;

            //var userDetails = unitOfWork.UserRepository.GetById(id);
            var userDetails = from userTable in unitOfWork.UserRepository.GetAll().Where(i => i.UserID == id)
                              select new UserModel()
                              {
                                  FirstName = userTable.FirstName,
                                  LastName = userTable.LastName,
                                  Email = userTable.Email,
                                  Address = userTable.Address,
                                  Phone = (int)userTable.Phone
                              };

            if (userDetails == null)
            {
                return RedirectToAction("MissingPage", "NotFound");
            }
            ViewBag.UserDetails = userDetails;
            return View(userDetails);
        }

        [HttpGet]
        public ActionResult AddDoctor()
        {
            //Finding & Getting UserName by SessionID
            var dbUser = unitOfWork.UserRepository.GetAll().Where(u => u.UserID == int.Parse(Session["UserID"].ToString()));

            var fullName = new UserModel()
            {
                FirstName = dbUser.First().FirstName,
                LastName = dbUser.Last().LastName,
            };

            ViewBag.User = fullName;
            return View();
        }


        public ActionResult AllPatients()
        {
            var allPatients = (from patient in unitOfWork.PatientRepository.GetAll()
                               join user in unitOfWork.UserRepository.GetAll().Where(u => u.IsDeleted == false && u.RoleID == 3) on patient.UserID equals user.UserID
                               where patient.UserID == user.UserID &&
                               user.IsDeleted == false
                               select new AllPatients()
                               {
                                   UserID = (int)patient.UserID,
                                   FirstName = user.FirstName,
                                   LastName = user.LastName,
                                   Email = user.Email,
                                   Phone = (int)user.Phone,
                                   Gender = user.Gender,
                                   Age = (DateTime)user.Age,
                                   Address = user.Address
                               }).ToList();
            //ViewBag.Patients = allPatients;
            return View(allPatients);
        }

        [HttpPost]
        public ActionResult DeletePatient(int id)
        {
            if (Request.IsAuthenticated)
            {
                var patientAsUser = unitOfWork.UserRepository.GetById(id);
                var patient = unitOfWork.PatientRepository.GetAll().FirstOrDefault(p => p.UserID == patientAsUser.UserID);
                if (patient.UserID == patientAsUser.UserID)
                {
                    try
                    {
                        var userIsDeleted = new User()
                        {
                            UserID = patientAsUser.UserID,
                            FirstName = patientAsUser.FirstName,
                            LastName = patientAsUser.LastName,
                            Email = patientAsUser.Email,
                            Password = patientAsUser.Password,
                            Phone = patientAsUser.Phone,
                            Gender = patientAsUser.Gender,
                            Age = patientAsUser.Age,
                            Address = patientAsUser.Address,
                            IsDeleted = true,
                            CreatedOn = patientAsUser.CreatedOn,
                            CreatedBy = patientAsUser.CreatedBy,
                            UpdatedOn = DateTime.Now,
                            UpdatedBy = patientAsUser.UpdatedBy,
                            RoleID = patientAsUser.RoleID,
                        };

                        var patientIsDeleted = new Patient()
                        {
                            PatientID = patient.PatientID,
                            UserID = patient.UserID,
                            IsDeleted = true,
                            CreatedOn = patient.CreatedOn,
                            CreatedBy = patient.CreatedBy,
                            UpdatedOn = DateTime.Now,
                            UpdatedBy = patient.UpdatedBy
                        };
                        unitOfWork.UserRepository.Update(userIsDeleted);
                        unitOfWork.PatientRepository.Update(patientIsDeleted);
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("AllPatients", "Admin");
                    }
                }
            }
            return RedirectToAction("AllPatients", "Admin");
        }

        public ActionResult AllDoctors()
        {
            var allDoctors = (from doctor in unitOfWork.DoctorRepository.GetAll().Where(d => d.IsDeleted == false)
                              join user in unitOfWork.UserRepository.GetAll().Where(u => u.IsDeleted == false && u.RoleID == 2) on doctor.UserID equals user.UserID
                              where doctor.UserID == user.UserID &&
                              user.IsDeleted == false
                              select new AllDoctors()
                              {
                                  FirstName = user.FirstName,
                                  LastName = user.LastName,
                                  Email = user.Email,
                                  Phone = (int)user.Phone,
                                  Education = doctor.Education,
                                  Experience = (int)doctor.Experience,
                                  Fees = doctor.Fees ?? 0,
                                  Specialization = doctor.Specialization,
                                  WorkingHours = doctor.WorkingHours ?? 0,
                                  DoctorID = (int)doctor.UserID,
                                  //Age = (DateTime)user.Age
                              }).ToList();
            //ViewBag.Doctors = allDoctors;

            return View(allDoctors);
        }

        [HttpPost]
        public ActionResult AddDoctor(UserModel model)
        {
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var user = new User()
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email.ToLower(),
                            Password = model.Password,
                            Phone = model.Phone,
                            Gender = model.Gender,
                            Age = model.Age,
                            Address = model.Address,
                            IsDeleted = false,
                            CreatedOn = DateTime.Now,
                            RoleID = 2,
                            CreatedBy = Session["FirstName"] + " " + Session["LastName"]
                        };
                        unitOfWork.UserRepository.AddNew(user);

                        var doctor = new Doctor()
                        {
                            UserID = user.UserID,
                            Specialization = model.Specialization,
                            Experience = model.Experience,
                            Education = model.Education,
                            IsDeleted = user.IsDeleted,
                            CreatedOn = user.CreatedOn,
                            CreatedBy = Session["FirstName"] + " " + Session["LastName"]
                        };
                        unitOfWork.DoctorRepository.AddNew(doctor);

                        return RedirectToAction("Index", "Admin");
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("AddDoctor", "Admin");
                    }
                }
                else 
                { 
                    return View(model); 
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult DeleteDoctor(int id)
        {
            if (Request.IsAuthenticated)
            {
                var doctorAsUser = unitOfWork.UserRepository.GetById(id);
                var doctor = unitOfWork.DoctorRepository.GetAll().FirstOrDefault(p => p.UserID == doctorAsUser.UserID);
                if (doctor.UserID == doctorAsUser.UserID)
                {
                    try
                    {
                        var userIsDeleted = new User()
                        {
                            UserID = doctorAsUser.UserID,
                            FirstName = doctorAsUser.FirstName,
                            LastName = doctorAsUser.LastName,
                            Email = doctorAsUser.Email,
                            Password = doctorAsUser.Password,
                            Phone = doctorAsUser.Phone,
                            Gender = doctorAsUser.Gender,
                            Age = doctorAsUser.Age,
                            Address = doctorAsUser.Address,
                            IsDeleted = true,
                            CreatedOn = doctorAsUser.CreatedOn,
                            CreatedBy = doctorAsUser.CreatedBy,
                            UpdatedOn = DateTime.Now,
                            UpdatedBy = doctorAsUser.UpdatedBy,
                            RoleID = doctorAsUser.RoleID,
                        };

                        var doctorIsDeleted = new Doctor()
                        {
                            DoctorID = doctor.DoctorID,
                            Education = doctor.Education,
                            Specialization = doctor.Specialization,
                            Experience = doctor.Experience,
                            ProfilePicture = doctor.ProfilePicture,
                            Fees = doctor.Fees,
                            WorkingHours = doctor.WorkingHours,
                            IsDeleted = true,
                            CreatedOn = doctor.CreatedOn,
                            CreatedBy = doctor.CreatedBy,
                            UpdatedOn = DateTime.Now,
                            UpdatedBy = doctor.UpdatedBy,
                            UserID = doctor.UserID
                        };
                        unitOfWork.UserRepository.Update(userIsDeleted);
                        unitOfWork.DoctorRepository.Update(doctorIsDeleted);
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("AllDoctors", "Admin");
                    }
                }
            }
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public ActionResult DoctorUpdate(int id)
        {
            if (Request.IsAuthenticated)
            {
                var doctorAsUser = unitOfWork.UserRepository.GetById(id);
                var doctor = unitOfWork.DoctorRepository.GetById(doctorAsUser.UserID);
                if (doctor.UserID == doctorAsUser.UserID)
                {
                    var userIsUpdated = new User()
                    {

                    };

                    var doctorIsUpdated = new Doctor()
                    {

                    };
                    unitOfWork.UserRepository.Update(userIsUpdated);
                    unitOfWork.DoctorRepository.Update(doctorIsUpdated);
                }
            }
            return RedirectToAction("AllDoctors", "Admin");
        }

        public ActionResult AllAppointments()
        {
            //var allAppointments = unitOfWork.AppointmentRepository.GetAll().ToList();

            var allAppointments = (from a in unitOfWork.AppointmentRepository.GetAll().Where(a => a.IsDeleted == false)
                                   select new AllAppointments()
                                   {
                                       Aid = a.AppointmentID,
                                       Title = a.Title,
                                       CreatedOn = a.CreatedOn ?? DateTime.MinValue,
                                       Status = a.Status,
                                       Appointment_DateTime = a.Appointment_DateTime ?? DateTime.MinValue
                                   }).ToList();

            //ViewBag.Appointments = allAppointments;
            return View(allAppointments);
        }

        [HttpPost]
        public ActionResult DeleteAppointment(int id)
        {
            if (Request.IsAuthenticated)
            {
                var appointment = unitOfWork.AppointmentRepository.GetById(id);
                if (appointment.AppointmentID == id)
                {
                    var isDeleted = new Appointment()
                    {
                        AppointmentID = appointment.AppointmentID,
                        DoctorID = appointment.DoctorID,
                        PatientID = appointment.PatientID,
                        FeesPaid = appointment.FeesPaid,
                        Appointment_DateTime = appointment.Appointment_DateTime,
                        Title = appointment.Title,
                        Description = appointment.Description,
                        PatientHistory = appointment.PatientHistory,
                        IsDeleted = true,
                        CreatedOn = appointment.CreatedOn,
                        CreatedBy = appointment.CreatedBy,
                        UpdatedOn = DateTime.Now,
                        UpdatedBy = appointment.UpdatedBy,
                        Status = appointment.Status,
                    };
                    unitOfWork.AppointmentRepository.Update(isDeleted);
                }
            }
            return RedirectToAction("AllAppointments", "Admin");
        }
    }
}