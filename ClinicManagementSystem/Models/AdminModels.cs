using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicManagementSystem.Models
{
    public class AdminModels
    {
    }

    public class AllPatients
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get;set; }
        public int Phone { get; set; }
        public string Gender { get; set; }
        public DateTime Age { get; set; }
        public string Address { get; set; }
    }

    public class AllDoctors
    {
        public int DoctorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Phone { get; set; }
        public string Gender { get; set; }
        public DateTime Age { get; set; }
        public string Address { get; set; }
        public string Education { get; set; }
        public int Experience { get; set; }
        public int? Fees { get; set; }
        public string Specialization { get; set; }
        public int? WorkingHours { get; set; }
    }
}