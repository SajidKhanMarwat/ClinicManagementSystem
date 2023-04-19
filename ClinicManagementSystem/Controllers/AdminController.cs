using ClinicManagementSystem.Models;
using ClinicManagementSystem.Repository;
using ClinicManagementSystem.Repository.EntityModel;
using System;
using System.Linq;
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
                var totalUsers = unitOfWork.UserRepository.GetAll().Where(u => u.IsDeleted == false).ToList();
                ViewBag.AllUsers = totalUsers;

                int doctorsCount = (from user in unitOfWork.UserRepository.GetAll()
                                    join doctors in unitOfWork.DoctorRepository.GetAll() on user.UserID equals doctors.UserID
                                    where user.UserID == doctors.UserID &&
                                    user.IsDeleted == false
                                    select new
                                    {
                                        user.UserID
                                    }
                                   ).Count();
                ViewBag.totalDoctors = doctorsCount;


                // All Patients
                int patientsCount = (from user in unitOfWork.UserRepository.GetAll()
                                     join patients in unitOfWork.DoctorRepository.GetAll() on user.UserID equals patients.UserID
                                     where user.UserID == patients.UserID &&
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


                //Finding & Getting UserName by SessionID
                //var user = unitOfWork.UserRepository.GetAll().Where(u => u.UserID == int.Parse(Session["UserID"].ToString()));

                //var fullName = new UserModel()
                //{
                //    FirstName = user.First().FirstName,
                //    LastName = user.Last().LastName,
                //};

                //ViewBag.User = fullName;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult Index(int id)
        {
            var dbUser = unitOfWork.UserRepository.GetById(id);

            var user = new User()
            {
                UserID = dbUser.UserID,
                FirstName = dbUser.FirstName,
                LastName = dbUser.LastName,
                Email = dbUser.Email,
                Password = dbUser.Password,
                Phone = dbUser.Phone,
                Gender = dbUser.Gender,
                Age = dbUser.Age,
                Address = dbUser.Address,
                IsDeleted = true,
                CreatedOn = dbUser.CreatedOn,
                CreatedBy = dbUser.CreatedBy,
                UpdatedOn = DateTime.Now,
                UpdatedBy = dbUser.UpdatedBy,
                RoleID = dbUser.RoleID,
            };

            if (user.UserID == id)
            {
                unitOfWork.UserRepository.Update(user);
            }
            return RedirectToAction("Index", "Admin");        
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

            if(userDetails == null)
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

        [HttpPost]
        public ActionResult AddDoctor(UserModel model)
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
                };
                unitOfWork.DoctorRepository.AddNew(doctor);

               return RedirectToAction("Index", "Admin");
            }
            catch (Exception)
            {
                RedirectToAction("Index", "Admin");
            }
            return View("Index");
        }

        public ActionResult AllPatients()
        {
            var allPatients = (from patient in unitOfWork.PatientRepository.GetAll().Where(p => p.IsDeleted == false)
                              join user in unitOfWork.UserRepository.GetAll() on patient.UserID equals user.UserID
                               where patient.UserID == user.UserID
                              select new AllDoctors()
                              {
                                  FirstName = user.FirstName,
                                  LastName = user.LastName,
                                  Email = user.Email,
                                  Phone = (int)user.Phone,
                                  Gender = user.Gender,
                                  Age = (DateTime)user.Age,
                                  Address = user.Address
                              }).ToList();
            ViewBag.Patients = allPatients;
            return View();
        }

        public ActionResult AllDoctors()
        {
            var allDoctors = (from doctor in unitOfWork.DoctorRepository.GetAll().Where(d => d.IsDeleted == false)
                              join user in unitOfWork.UserRepository.GetAll() on doctor.UserID equals user.UserID
                              where doctor.UserID == user.UserID
                              select new AllDoctors()
                              {
                                  FirstName = user.FirstName,
                                  LastName = user.LastName,
                                  Email = user.Email,
                                  Phone = (int)user.Phone,
                                  Education = doctor.Education,
                                  Experience = (int)doctor.Experience,
                                  Fees = (int)doctor.Fees,
                                  Specialization = doctor.Specialization,
                                  WorkingHours = (int)doctor.WorkingHours,
                                  DoctorID = (int)doctor.UserID
                              }).ToList();
            ViewBag.Doctors = allDoctors;
            return View();
        }

        public ActionResult AllAppointments()
        {
            var allAppointments = unitOfWork.AppointmentRepository.GetAll();
            ViewBag.Appointments = allAppointments;
            return View();
        }
    }
}