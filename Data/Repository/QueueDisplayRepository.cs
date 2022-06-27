using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Repository
{
    public class QueueDisplayRepository : Repository<QueueDisplay>, IQueueDisplayRepository
    {
        public QueueDisplayRepository(ClientDSDbContext context) : base(context) { }

        public QueueDisplay Get(long id)
        {
            return _context.QueueDisplays.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<QueueDisplay> GetAll(Func<QueueDisplay, bool> func)
        {
            return _context.QueueDisplays.Where(func);
        }
    }
}
