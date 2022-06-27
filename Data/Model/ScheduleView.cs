using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Model
{
    public class ScheduleView
    {
        public long Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Template { get; set; }
        public string Branch { get; set; }
        public string BranchInAmharic { get; set; }
        public string CurrencyPosition { get; set; }
        public string RSS { get; set; }

        public int VideoLength { get; set; }
        public int WeatherDuration { get; set; }
        public int TimeDuration { get; set; }
        public int DifferenceDuration { get; set; }
        public int ScheduleId { get; set; }
        
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public int NewsDuration { get; set; }
        public int ContentDuration { get; set; }
        public DateTime LastUpdate { get; set; }
        public long TemplateId { get; set; }

        public string Orientation { get; set; }
    }
}
