using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Interface
{
    public interface IServiceGroupRepository : IRepository<ServiceGroup>
    {
        IEnumerable<ServiceGroup> GetAll(Func<ServiceGroup, bool> func);
        ServiceGroup Get(long id);
    }
}
