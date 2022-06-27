using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Models
{
    public class TransferCustomer
    {
        public string Token { get; set; }
        public string Service { get; set; }
        public string TransferReason { get; set; }
    }
}
