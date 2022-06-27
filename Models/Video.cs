using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Models
{
    public class Video
    {
        public int Step { get; set; }
        public string Name { get; set; }
        public double Length { get; set; }
        public bool IsVideo { get; set; }
        public bool IsStream { get; set; }
    }
}
