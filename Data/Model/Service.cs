using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Model
{
    public class Service
    {
        [Key,Required]
        public long Id { get; set; }
        [Required,MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public long GroupId { get; set; }

        [Required]
        public bool HasPreference { get; set; }

        [Required,MaxLength(50)]
        public string DisplayName { get; set; }
    }
}
