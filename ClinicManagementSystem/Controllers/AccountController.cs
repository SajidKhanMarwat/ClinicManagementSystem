using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services.Description;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Repository;
using ClinicManagementSystem.Repository.EntityModel;


namespace ClinicManagementSystem.Controllers
{

    public class AccountController : Controller
    {

        private UnitOfWork _unitOfWork = new UnitOfWork();

        enum userRoles
        {
            Admin,
            Doctor,
            Patient
        }

        // GET: Account
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var emailCheck = _unitOfWork.UserRepository.GetAll().Any(u => u.Email == viewModel.Email);

                if (emailCheck == false)
                {
                    var user = new User()
                    {
                        FirstName = viewModel.FirstName,
                        LastName = viewModel.LastName,
                        Email = viewModel.Email.ToLower(),
                        Password = viewModel.Password,
                        Age = viewModel.Age,
                        Gender = viewModel.Gender,
                        Address = viewModel.Address,
                        Phone = viewModel.Phone,
                        CreatedOn = DateTime.Today,
                        IsDeleted = false,
                        // Only patients can register, that's why we need to assing their role manually as we know only Patients can register.
                        RoleID = 3
                    };
                    _unitOfWork.UserRepository.AddNew(user);


                    var patient = new Patient()
                    {
                        UserID = user.UserID,
                        IsDeleted = user.IsDeleted,
                        CreatedOn = user.CreatedOn,
                    };
                    _unitOfWork.PatientRepository.AddNew(patient);

                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ViewBag.EmailCheck = "Email Already Exists. Please Login";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                bool auth = _unitOfWork.UserRepository.GetAll().Any(user => user.Email == viewModel.Email && user.Password == viewModel.Password && user.IsDeleted == false);

                if (auth == true)
                {
                    var currentUserRole = from u in _unitOfWork.UserRepository.GetAll()
                                          join r in _unitOfWork.RoleRepository.GetAll() on u.RoleID equals r.RoleID
                                          where u.Email == viewModel.Email
                                          && u.Password == viewModel.Password
                                          select new
                                          {
                                              u.UserID,
                                              u.FirstName,
                                              u.LastName,
                                              r.Name,
                                              u.IsDeleted
                                          };

                    var currentUserID = currentUserRole.FirstOrDefault()?.UserID;
                    if (currentUserID.HasValue)
                    {
                        Session["UserID"] = currentUserID.Value.ToString();
                        Session["FirstName"] = currentUserRole.FirstOrDefault()?.FirstName;
                        Session["LastName"] = currentUserRole.FirstOrDefault()?.LastName;
                    }

                    FormsAuthentication.SetAuthCookie(currentUserRole.Where(name => name.FirstName != string.Empty).ToString(), false);

                    if (currentUserRole.Any(r => r.Name == userRoles.Admin.ToString()))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (currentUserRole.Any(r => r.Name == userRoles.Patient.ToString()))
                    {
                        return RedirectToAction("Index", "Patient");
                    }
                    else if (currentUserRole.Any(role => role.Name == userRoles.Doctor.ToString()))
                    {
                        return RedirectToAction("Index", "Doctor");
                    }
                }
            }
            return View();
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}