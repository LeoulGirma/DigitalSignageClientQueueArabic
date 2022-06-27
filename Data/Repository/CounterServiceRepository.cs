using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Repository
{
    public class CounterServiceRepository : Repository<CounterService>, ICounterServiceRepository
    {
        public CounterServiceRepository(ClientDSDbContext context) : base(context) { }

        public CounterService Get(long id)
        {
            return _context.CounterServices.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<CounterService> GetAll(Func<CounterService, bool> func)
        {
            return _context.CounterServices.Where(func);
        }
    }
}
