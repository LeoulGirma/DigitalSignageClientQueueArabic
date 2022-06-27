using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Repository
{
    public class CounterRepository : Repository<Counter>, ICounterRepository
    {
        public CounterRepository(ClientDSDbContext context) : base(context) { }

        public Counter Get(long id)
        {
            return _context.Counters.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Counter> GetAll(Func<Counter, bool> func)
        {
            return _context.Counters.Where(func);
        }
    }
}
