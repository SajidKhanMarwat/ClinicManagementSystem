using ClinicManagementSystem.Repository.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicManagementSystem.Repository
{
    public class UnitOfWork
    {
        private CMSEntitiesDB _cMSEntities = new CMSEntitiesDB();
        private IRepository<User> _usersRepository;
        private IRepository<Patient> _patientRepository;
        private IRepository<Doctor> _doctorRepository;
        private IRepository<Appointment> _appointmentsRepository;
        private IRepository<Role> _roleRepository;
        private IRepository<Prescription> _prescriptionRepository;
        private IRepository<Payment> _paymentRepository;
        private IRepository<AppointmentComment> _appointmentCommentRepository;
        private IRepository<Permission> _permissionRepository;
        private IRepository<User_Permissions> _userPermissionsRepository;


        // Users
        public IRepository<User> UserRepository
        {
            get
            {
                if (_usersRepository == null)
                {
                    _usersRepository = new Repository<User>(_cMSEntities);
                }
                return _usersRepository;
            }
        }

        // Doctors
        public IRepository<Doctor> DoctorRepository
        {
            get
            {
                if(_doctorRepository == null)
                {
                    _doctorRepository = new Repository<Doctor>(_cMSEntities);
                }
                return _doctorRepository;
            }
        }

        // Patients
        public IRepository<Patient> PatientRepository
        {
            get
            {
                if(_patientRepository == null)
                {
                    _patientRepository = new Repository<Patient>(_cMSEntities);
                }
                return _patientRepository;
            }
        }

        // Appointments
        public IRepository<Appointment> AppointmentRepository
        {
            get
            {
                if(_appointmentsRepository == null)
                {
                    _appointmentsRepository = new Repository<Appointment>(_cMSEntities);
                }
                return _appointmentsRepository;
            }
        }

        public IRepository<Role> RoleRepository
        {
            get {

                if (_roleRepository == null)
                {
                    _roleRepository = new Repository<Role>(_cMSEntities);
                }
                
                return _roleRepository; }
        }

        public IRepository<Prescription> PrescriptionRepository
        {
            get
            {
                if(_prescriptionRepository == null)
                {
                    _prescriptionRepository = new Repository<Prescription>(_cMSEntities);
                }
                return _prescriptionRepository;
            }
        }

        public IRepository<Payment> PaymentRepository
        {
            get
            {
                if (_paymentRepository == null)
                {
                    _paymentRepository = new Repository<Payment>(_cMSEntities);
                }
                return _paymentRepository;
            }
        }

        public IRepository<AppointmentComment> AppointmentCommentRepository
        {
            get
            {
                if (_appointmentCommentRepository == null)
                {
                    _appointmentCommentRepository = new Repository<AppointmentComment>(_cMSEntities);
                }
                return _appointmentCommentRepository;
            }
        }

        public IRepository<Permission> PermissionRepository
        {
            get
            {
                if(_permissionRepository == null)
                {
                    _permissionRepository = new Repository<Permission>(_cMSEntities);
                }
                return _permissionRepository;
            }
        }

        public IRepository<User_Permissions> UserPermissionsRepository
        {
            get
            {
                if(_userPermissionsRepository == null)
                {
                    _userPermissionsRepository = new Repository<User_Permissions>(_cMSEntities);
                }
                return (_userPermissionsRepository);
            }
        }
    }
}