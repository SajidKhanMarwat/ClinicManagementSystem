using ClinicManagementSystem.Repository.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicManagementSystem.Models
{

    public class PatientViewModel
    {

    }

    public class DoctorDDL
    {
        private SelectList _DoctorName;
        private SelectList _DoctorSpecialization;

        public SelectList Name
        {
            get
            {
                return _DoctorName;
            }
        }

        public SelectList Specialization
        {
            get
            {
                return _DoctorSpecialization;
            }
        }
    }

    public class AppointmentsDetails
    {
        private string _appointmentStatus = "Pending";

        public int FeesPaid { get; set; }
        public DateTime Appointment_DateTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PatientHistory { get; set; }
        public string Status
        {
            get { return _appointmentStatus; }
            set
            {
                _appointmentStatus = value;
            }
        }
        public int DoctorID { get; set; }
    }
}