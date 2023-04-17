using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicManagementSystem.Models
{
    public class HomeViewModel
    {
    }

    public class AvailableDoctorsModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Education { get; set; }
        public string Specialization { get; set; }
        public int Experience { get; set; }
        public int Fees { get; set; }
    }
}