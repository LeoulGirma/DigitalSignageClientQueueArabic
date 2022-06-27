using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Repository
{
    public class CalendarViewRepository : Repository<CalendarView>, ICalendarViewRepository
    {
        public CalendarViewRepository(ClientDSDbContext context) : base(context)
        {

        }

        public CalendarView Get(long id)
        {
            return _context.CalendarViews.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<CalendarView> GetAll(Func<CalendarView, bool> func)
        {
            return _context.CalendarViews.Where(func);
        }

    }
}
