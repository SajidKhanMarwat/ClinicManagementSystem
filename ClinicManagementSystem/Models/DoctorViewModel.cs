﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicManagementSystem.Models
{
    public class DoctorViewModel
    {

    }

    public class CurrentAppointmentsDoc
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int ApID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreatedOn { get; set; }
        public string PatientHistory { get; set; }
        public string Appointment_DateTime { get; set; }
        public int PatientID { get; set; }
        public string ApStatus { get; set; }
    }

    //public class StatusUpdate
    //{
    //    public string Accepted { get; set; }
    //    public string Declined { get; set; }
    //}
}