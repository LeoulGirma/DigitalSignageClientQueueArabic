using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Repository
{
    public class ServiceRepository : Repository<Service>, IServiceRepository
    {
        public ServiceRepository(ClientDSDbContext context) : base(context) { }

        public Service Get(long id)
        {
            return _context.Services.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Service> GetAll(Func<Service, bool> func)
        {
            return _context.Services.Where(func);
        }
    }
}
