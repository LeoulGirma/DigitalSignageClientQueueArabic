using DigitalSignageClient.Data.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ClientDSDbContext _context;
        public Repository(ClientDSDbContext context)
        {
            _context = context;
        }
        protected void Save() => _context.SaveChanges();

        public void Add(T entity)
        {
            _context.DetachAllEntities();
            _context.Entry(entity).State = EntityState.Added;
            _context.Add(entity);

            Save();
        }

        public virtual void Remove(T entity)
        {
            _context.DetachAllEntities();
            _context.Entry(entity).State = EntityState.Deleted;

            _context.Remove(entity);

            Save();
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public T GetById(long id)
        {
            return _context.Set<T>().Find(id);
        }

        public void Update(T entity)
        {
            _context.DetachAllEntities();

            _context.Entry(entity).State = EntityState.Modified;

            Save();
        }

        public IEnumerable<T> Where(Func<T, bool> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }

        public int Count(Func<T, Boolean> predicate)
        {
            return _context.Set<T>().Where(predicate).Count();
        }

        public bool Any(Func<T, bool> predicate)
        {
            return _context.Set<T>().Any(predicate);
        }

        public bool Any()
        {
            return _context.Set<T>().Any();
        }

        public void AddRange(List<T> entity)
        {
            _context.DetachAllEntities();
            foreach (var item in entity)
            {
                _context.Entry(item).State = EntityState.Added;

            }
            _context.Set<T>().AddRange(entity);
            Save();
        }

        public void RemoveRange(List<T> entity)
        {
            _context.DetachAllEntities();

            foreach (var item in entity)
            {
                _context.Entry(item).State = EntityState.Deleted;

            }
            _context.Set<T>().RemoveRange(entity);
            Save();
        }

        public void UpdateRange(List<T> entities)
        {
            _context.DetachAllEntities();

            foreach (var item in entities)
            {
                _context.Entry(item).State = EntityState.Modified;

                _context.Set<T>().Update(item);
                Save();
            }
        }



    }
}
