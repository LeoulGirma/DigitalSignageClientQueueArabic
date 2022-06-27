using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Model
{
    public class Token
    {
        [Key,Required]
        public long Id { get; set; }
        [Required]
        public int TokenNumber { get; set; }
        [Required]
        public DateTime TokenDate { get; set; }
    }
}
