using ClinicManagementSystem.Repository.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Repository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(int id);


        void Delete(int id);
        void Update(T entity);


        void AddNew(T entity);
    }
}
