using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Interface
{
    public interface IScheduleViewRepository : IRepository<ScheduleView>
    {
        IEnumerable<ScheduleView> GetAll(Func<ScheduleView, bool> func);
        ScheduleView Get(long id);
    }
}
