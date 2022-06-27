using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Interface
{
    public interface IQueueRepository : IRepository<Queue>
    {
        IEnumerable<Queue> GetAll(Func<Queue, bool> func);
        Queue Get(long id);
    }
}
