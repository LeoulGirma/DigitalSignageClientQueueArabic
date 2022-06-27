using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Models
{
    public class QueueAudit
    {
        public List<Queue> Queues { get; set; }
        public long ScreenId { get; set; }
    }
}
