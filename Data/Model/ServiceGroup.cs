using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Model
{
    public class ServiceGroup
    {
        [Key,Required]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
