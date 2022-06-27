using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Interface
{
    public interface ICounterServiceRepository : IRepository<CounterService>
    {
        IEnumerable<CounterService> GetAll(Func<CounterService, bool> func);
        CounterService Get(long id);
    }
}
