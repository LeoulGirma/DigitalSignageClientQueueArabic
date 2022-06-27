using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Interface
{
    public interface ICounterRepository : IRepository<Counter>
    {
        IEnumerable<Counter> GetAll(Func<Counter, bool> func);
        Counter Get(long id);
    }
}
