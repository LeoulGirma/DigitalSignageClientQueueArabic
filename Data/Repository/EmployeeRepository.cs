using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Repository
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ClientDSDbContext context) : base(context) { }

        public Employee Get(long id)
        {
            return _context.Employees.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Employee> GetAll(Func<Employee, bool> func)
        {
            return _context.Employees.Where(func);
        }
    }
}
