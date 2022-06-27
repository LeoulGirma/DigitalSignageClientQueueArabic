using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Model
{
    public class VideoView
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public int RowNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsFullScreen { get; set; }
        public long ScheduleId { get; set; }
        public int Duration { get; set; }
        public bool IsVideo { get; set; }
        public bool IsStream { get; set; }
    }
}
