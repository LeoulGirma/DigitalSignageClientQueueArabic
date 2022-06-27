using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Interface
{
    public interface ICalendarViewRepository : IRepository<CalendarView>
    {
        IEnumerable<CalendarView> GetAll(Func<CalendarView, bool> func);
        CalendarView Get(long id);
    }
}
