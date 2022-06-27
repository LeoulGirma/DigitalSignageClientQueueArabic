using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Interface
{
    public interface IServiceRepository : IRepository<Service>
    {
        IEnumerable<Service> GetAll(Func<Service, bool> func);
        Service Get(long id);
    }
}
