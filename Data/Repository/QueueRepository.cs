using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Repository
{
    public class QueueRepository : Repository<Queue>, IQueueRepository
    {
        public QueueRepository(ClientDSDbContext context) : base(context) { }

        public Queue Get(long id)
        {
            return _context.Queues.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Queue> GetAll(Func<Queue, bool> func)
        {
            return _context.Queues.Where(func);
        }
    }
}
