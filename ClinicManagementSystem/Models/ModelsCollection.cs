using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicManagementSystem.Models
{
    public class ModelsCollection
    {
        public List<AllPatients> AllPatientsModel { get; set; }
        public List<AllDoctors> AllDoctorsModel { get; set; }
        public List<TodayAppointments> TodayAppointmentsModel { get; set; }
        public List<CurrentAppointments> CurrentAppointmentsModel { get; set; }
        public List<AvailableDoctorsModel> AvailableDoctorsModel { get; set; }
        public List<DoctorDDL> DoctorDDLModel { get; set; }
        public List<AppointmentsDetails> AppointmentsDetailsModel { get; set; }
        public List<GetCurrentAppointments> GetGetCurrentAppointmentsModel { get; set; }
        public List<UserModel> UserModel { get; set; }
    }
}