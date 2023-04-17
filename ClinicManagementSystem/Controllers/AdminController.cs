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
                var totalUsers = unitOfWork.UserRepository.GetAll().Where(i => i.IsDeleted == false && i.RoleID != 1).ToList();
                ViewBag.All = totalUsers;

                int doctorsCount = unitOfWork.DoctorRepository.GetAll()
                    .Where(d => totalUsers
                    .Any(u => u.UserID == d.UserID))
                    .Count();
                ViewBag.totalDoctors = doctorsCount;


                // All Patients
                int patientsCount = unitOfWork.PatientRepository.GetAll().Where(i => i.IsDeleted == false)
                    .Where(p => totalUsers
                    .Any(u => u.UserID == p.UserID))
                    .Count();
                ViewBag.TotalPatients = patientsCount;


                // All Appointments
                int appointmentsCount = unitOfWork.AppointmentRepository.GetAll().Where(i => i.IsDeleted == false)
                    .Count();
                ViewBag.TotalAppointments = appointmentsCount;


                //Finding & Getting UserName by SessionID
                var user = unitOfWork.UserRepository.GetAll().Where(u => u.UserID == int.Parse(Session["UserID"].ToString()));

                var fullName = new UserModel()
                {
                    FirstName = user.First().FirstName,
                    LastName = user.Last().LastName,
                };

                ViewBag.User = fullName;

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

            var userDetails = unitOfWork.UserRepository.GetById(id);
            if(userDetails == null)
            {
                return RedirectToAction("MissingPage", "NotFound");
            }
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
                    Email = model.Email,
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
                    IsDeleted = user.IsDeleted,
                    CreatedOn = user.CreatedOn,
                };
                unitOfWork.DoctorRepository.AddNew(doctor);

                RedirectToAction("Index", "Admin");
            }
            catch (Exception)
            {
                RedirectToAction("Index", "Admin");
            }
            return View();
        }
    }
}