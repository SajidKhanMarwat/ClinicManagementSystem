using ClinicManagementSystem.Controllers;
using ClinicManagementSystem.Repository.EntityModel;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace ClinicManagementSystem.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        
        private readonly CMSEntitiesDB _cmsEntities;


        // Repository Constructor
        public Repository(CMSEntitiesDB entities)
        {
            _cmsEntities = entities;
        }


        //Adding New User to the Users Table in Databasse
        public void AddNew(T entity)
        {
            _cmsEntities.Set<T>().Add(entity);
            _cmsEntities.SaveChanges();
        }

        public void Delete(int id)
        {
            //_cmsEntities.Set<T>().Remove(GetById(id)); // This will delete the user completely which is not recommended.
                       
            _cmsEntities.Set<T>().AddOrUpdate(GetById(id)); // This will change the 'IsDeleted' to 1, which means the user is deleted. But in reality the user will be available but not active.            
            _cmsEntities.SaveChanges();
        }

        //Get All Users From Database
        public IEnumerable<T> GetAll()
        {
            return _cmsEntities.Set<T>().ToList();
        }

        public T GetById(int id)
        {
            var userByID = _cmsEntities.Set<T>().Find(id);

            if (userByID == null)
            {
                return null;
            }
            else
            {
                return userByID;
            }
        }

        public void Update(T entity)
        {
            _cmsEntities.Set<T>().AddOrUpdate(entity);
            _cmsEntities.SaveChanges();
        }
    }
}