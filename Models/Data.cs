using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Models
{
    public class Data
    {
        public List<CurrencyView> CurrencyViews { get; set; }
        public List<ScheduleView> ScheduleViews { get; set; }
        public CalendarView CalendarView { get; set; }
        public NavStyleView NavStyle { get; set; }
        public BodyStyleView BodyStyle { get; set; }
        public int Step { get; set; }
        public string AmharicCalendar { get; set; }

        public string GregorianCalendar { get; set; }
    }

    public class ImportSchedule
    {
        public List<ScheduleView> ScheduleViews { get; set; }
        public List<VideoView> ScheduleVideoViews { get; set; }
    }

    public class ImportCurrency
    {
        public List<CurrencyView> CurrencyViews { get; set; }
    }

    public class ImportRSS
    {
        public List<RSSStyleView> RSSStyleViews { get; set; }
        public List<RSSNewsView> RSSNewsViews { get; set; }

    }

    public class ImportWeather
    {
        public List<CalendarView> CalendarViews { get; set; }
    }


    public class ImportStyle
    {
        public List<NavStyleView> NavStyleView { get; set; }
        public List<BodyStyleView> BodyStyleView { get; set; }
    }

    public class ScheduleVideoView
    {
        public string FileName { get; set; }
        public long ScheduleId { get; set; }
        public bool ForAdd { get; set; }
        public int RowNumber { get; set; }
        public int Duration { get; set; }
        public bool IsVideo { get; set; }
    }

    public class RSS
    {
        public RSSNewsView RSSNewsView { get; set; }
        public RSSStyleView RSSStyleView { get; set; }
        public int RSSLength { get; set; }
    }

    public class RemoveVideo
    {
        public string FileName { get; set; }
    }

    public class CheckIfClientContains
    {
        public DateTime LastUpdate { get; set; }
        public Dictionary<string, bool> Files { get; set; }
        public bool Exists { get; set; }
        public string Type { get; set; }
        public bool HasLicense { get; set; }
        public string ServerUID { get; set; }
    }

    public class SendQMSData
    {
        public List<Service> Services { get; set; }
        public List<Counter> Counters { get; set; }
        public List<Employee> Employees { get; set; }
        public List<CounterService> CounterServices { get; set; }
        public List<ServiceGroup> ServiceGroups { get; set; }
    }
}
