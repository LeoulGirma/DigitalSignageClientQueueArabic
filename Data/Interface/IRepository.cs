using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Interface
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> Where(Func<T, bool> func);
        T GetById(long id);
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        void AddRange(List<T> entity);
        void RemoveRange(List<T> entity);
        void UpdateRange(List<T> entity);
        int Count(Func<T, bool> func);
    }
}
