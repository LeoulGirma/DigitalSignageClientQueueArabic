using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Repository
{
    public class ServiceGroupRepository : Repository<ServiceGroup>, IServiceGroupRepository
    {
        public ServiceGroupRepository(ClientDSDbContext context) : base(context) { }

        public ServiceGroup Get(long id)
        {
            return _context.ServiceGroups.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<ServiceGroup> GetAll(Func<ServiceGroup, bool> func)
        {
            return _context.ServiceGroups.Where(func);
        }
    }
}
