using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClinicManagementSystem.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        [Required]
        [Display(Name = "First Name Required")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        [Display (Name = "Email Required")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Password Required")]
        public string Password { get; set; }
        public int Phone { get; set; }
        [Required]
        [Display(Name = "Gender Required")]
        public string Gender { get; set; }
        public DateTime Age { get; set; }
        public string Address { get; set; }
        public int Experience { get; set; }
        public string Education { get; set; }
        public string Specialization { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public int RoleID { get; set; }
    }
}