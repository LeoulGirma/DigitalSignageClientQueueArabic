using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Model
{
    public class Employee
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Status { get; set; }

        public long ScreenId { get; set; }
        public bool IsActive { get; set; }

        public long CounterId { get; set; }
    }
}
