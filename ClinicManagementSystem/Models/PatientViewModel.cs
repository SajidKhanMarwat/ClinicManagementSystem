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
        public double CardNumber { get; set; }
        public DateTime Appointment_DateTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //public string PatientHistory { get; set; }
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

    public class AllAppointments
    {
        public int Aid { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime Appointment_DateTime { get; set; }
    }

    public class GetCurrentAppointments
    {
        public string Title { get; set; }
        public int DoctorID { get; set; }
        public string CreatedOn { get; set; }
        public string Appointment_DateTime { get; set; }
        public string Appointment_Status { get; set; }
    }

    public class PaymentsModel
    {
        public double CardNumber { get; set; }
        public decimal Amount { get; set; }
    }

    public class PrescriptionsCompleted
    {
        public string Title { get; set; }
        public string Medicines { get; set; }
        public string Usage { get; set; }
        public DateTime EndDate { get; set; }
    }
}