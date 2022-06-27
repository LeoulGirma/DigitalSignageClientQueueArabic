using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Model
{
    public class Counter
    {
        [Key,Required]
        public long Id { get; set; }

        [Required,MaxLength(50)]
        public string CounterNumber { get; set; }

        public long ScreenId { get; set; }
    }
}
