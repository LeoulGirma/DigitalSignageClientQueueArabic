using System.Linq;
using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using DigitalSignageClient.Models;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSignageClient.Controllers
{
    [Produces("application/json")]
    [Route("api/Accounts")]
    public class AccountsController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICounterRepository _counterRepository;
        public AccountsController(IEmployeeRepository employeeRepository, ICounterRepository counterRepository)
        {
            _employeeRepository = employeeRepository;
            _counterRepository = counterRepository;
        }

        [HttpPost]
        [Route("Login")]
        public string Login([FromBody] Credential credential)
        {
            Employee employee = _employeeRepository.Where(x => x.UserName == credential.UserName && x.Password == credential.Password).FirstOrDefault();
            if(employee == null)
            {
                return "AUTHENTICATE";
            }
            //if (employee.IsActive)
            //{
            //    return "ACTIVE";
            //}
            employee.IsActive = true;
            employee.CounterId = credential.CounterId;
            _employeeRepository.Update(employee);
            return "SUCCESS";
        }

        [HttpGet]
        [Route("Logout/{counterId}")]
        public string Logout(long counterId)
        {
            Employee employee = _employeeRepository.Where(x => x.CounterId == counterId).FirstOrDefault();
            if(employee != null && employee.IsActive)
            {
                employee.IsActive = false;
                _employeeRepository.Update(employee);
            }
            return "SUCCESS";

        }

        [HttpGet]
        [Route("GetCounter")]
        public IActionResult GetCounter()
        {
            return Ok(_counterRepository.Where(x => x.Id != 0).Select(x => new { Text = x.CounterNumber, Value = x.Id }));
        }
    }
}