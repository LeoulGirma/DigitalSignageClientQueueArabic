using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Model
{
    public class Queue
    {
        [Key,Required]
        public long Id { get; set; }
        [Required]
        public string TokenNumber { get; set; }
        [Required]

        public string Service { get; set; }
        [Required]

        public int Row { get; set; }
        [Required]

        public string Status { get; set; }

        [Required,MaxLength(50)]
        public string EmployeeId { get; set; }

        [Required,MaxLength(500)]
        public string EmployeeName { get; set; }

        [Required]
        public DateTime QueueDate { get; set; }
        [Required]

        public Nullable<long> CounterId { get; set; }

        public bool HasPreference { get; set; }

        public string TransferReason { get; set; }



        public long ScreenId { get; set; }
        public Nullable<TimeSpan> CalledTime { get; set; } 
        public Nullable<TimeSpan> ClosedTime { get; set; }

    }
}
