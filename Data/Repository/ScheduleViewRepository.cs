using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Repository
{
    public class SchduleViewRepository : Repository<ScheduleView>, IScheduleViewRepository
    {
        public SchduleViewRepository(ClientDSDbContext context) : base(context)
        {

        }

        public ScheduleView Get(long id)
        {
            return _context.ScheduleViews.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<ScheduleView> GetAll(Func<ScheduleView, bool> func)
        {
            return _context.ScheduleViews.Where(func);
        }

    }


}
