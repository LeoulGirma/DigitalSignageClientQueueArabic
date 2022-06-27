using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Models
{
    public class TellerLoginInfo
    {
        public string EmployeeName { get; set; }
        public string Counter { get; set; }
        public List<Service> Services { get; set; }
        public int MissingServiceCount { get; set; }
        public int AllServiceCount { get; set; }
    }
}
