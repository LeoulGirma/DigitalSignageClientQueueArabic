using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Interface
{
    public interface IQueueDisplayRepository : IRepository<QueueDisplay>
    {
        IEnumerable<QueueDisplay> GetAll(Func<QueueDisplay, bool> func);
        QueueDisplay Get(long id);
    }
}
