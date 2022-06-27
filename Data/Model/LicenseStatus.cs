using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Model
{
    public class LicenseStatus
    {
        public long Id { get; set; }
        public string ExpireDate { get; set; }
        public string ServerUID { get; set; }
    }
}
