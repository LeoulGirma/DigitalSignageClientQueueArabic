using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DigitalSignageClient.Data.Hubs;
using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using DigitalSignageClient.Models;
using MediaToolkit;
using MediaToolkit.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CognitiveServices.Speech;
using NetCoreAudio;

namespace DigitalSignageClient.Controllers
{
    [Produces("application/json")]
    [Route("api/Queues")]
    public class QueuesController : Controller
    {
        private readonly ICounterRepository _counterRepository;
        private readonly ICounterServiceRepository _counterServiceRepository;
        private readonly IQueueDisplayRepository _queueDisplayRepository;
        private readonly IQueueRepository _queueRepository;
        private readonly IServiceGroupRepository _serviceGroupRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHubContext<QueueHub> _queueHub;
        private readonly IHubContext<MissingServiceHub> _missingServiceHub;
        private readonly IHubContext<AllServiceHub> _allServiceHub;
        public static readonly object LockObject = new object();
        public static SemaphoreSlim semaphoreOutOfOrder = new SemaphoreSlim(1, 1);
        public static SemaphoreSlim semaphoreNext = new SemaphoreSlim(1, 1);
        public static SemaphoreSlim semaphoreClose = new SemaphoreSlim(1, 1);
        public static SemaphoreSlim semaphoreMissing = new SemaphoreSlim(1, 1);

        public QueuesController(ICounterRepository counterRepository, ICounterServiceRepository counterServiceRepository, IQueueDisplayRepository queueDisplayRepository,
            IQueueRepository queueRepository, IServiceGroupRepository serviceGroupRepository, IServiceRepository serviceRepository, ITokenRepository tokenRepository,
            IHubContext<QueueHub> queueHub, IHubContext<MissingServiceHub> missingServiceHub, IHubContext<AllServiceHub> allServiceHub,IEmployeeRepository employeeRepository)
        {
            _counterRepository = counterRepository;
            _counterServiceRepository = counterServiceRepository;
            _queueDisplayRepository = queueDisplayRepository;
            _queueRepository = queueRepository;
            _serviceGroupRepository = serviceGroupRepository;
            _serviceRepository = serviceRepository;
            _tokenRepository = tokenRepository;
            _queueHub = queueHub;
            _missingServiceHub = missingServiceHub;
            _allServiceHub = allServiceHub;
            _employeeRepository = employeeRepository;
        }

        [HttpGet]
        [Route("GetQueryDisplay")]
        public QueueDisplay GetQueueDisplay()
        {
            CheckDate();
            return _queueDisplayRepository.GetAll().FirstOrDefault();
        }

        [HttpGet]
        [Route("GetTellerInfo/{counterId}")]
        public TellerLoginInfo GetTellerInfo(long counterId)
        {
            TellerLoginInfo tellerLoginInfo = new TellerLoginInfo();

            Counter counter = _counterRepository.Where(x => x.Id == counterId).FirstOrDefault();
            tellerLoginInfo.Counter = counter.CounterNumber;

            Employee employee = _employeeRepository.Where(x => x.CounterId == counter.Id).FirstOrDefault();
            tellerLoginInfo.EmployeeName = employee.EmployeeName;

            List<long> serviceIds = _counterServiceRepository.Where(x => x.CounterId == counterId).Select(x => x.ServiceId).ToList();
            if (serviceIds.Count() > 0)
            {
                tellerLoginInfo.Services = _serviceRepository.Where(x => serviceIds.Contains(x.Id)).ToList();
                List<string> serviceNames = tellerLoginInfo.Services.Select(x => x.Name).ToList();
                tellerLoginInfo.AllServiceCount = _queueRepository.Where(x => x.Status == "Pending" && serviceNames.Contains(x.Service) && DateTime.Now.Day == x.QueueDate.Day && DateTime.Now.Month == x.QueueDate.Month && DateTime.Now.Year == x.QueueDate.Year).Count();
                tellerLoginInfo.MissingServiceCount = _queueRepository.Where(x => x.Status == "Missing" && serviceNames.Contains(x.Service) && DateTime.Now.Day == x.QueueDate.Day && DateTime.Now.Month == x.QueueDate.Month && DateTime.Now.Year == x.QueueDate.Year).Count();
            }
            else
                tellerLoginInfo.Services = new List<Service>();

            return tellerLoginInfo;
        }

        [HttpGet]
        [Route("AddToMissing/{counterId}")]
        public async Task<string> AddToMissing(long counterId)
        {
            Counter counter = _counterRepository.Where(x => x.Id == counterId).FirstOrDefault();
            QueueDisplay queueDisplay = _queueDisplayRepository.GetAll().FirstOrDefault();
            string checkQueueResponse = CheckQueue(counter.CounterNumber, queueDisplay);
            string missingToken = "";
            if (checkQueueResponse == "First")
                missingToken = queueDisplay.FirstToken;

            else if (checkQueueResponse == "Second")
                missingToken = queueDisplay.SecondToken;

            else if (checkQueueResponse == "Third")
                missingToken = queueDisplay.ThirdToken;

            else if (checkQueueResponse == "Fourth")
                missingToken = queueDisplay.FourthToken;

            else if (checkQueueResponse == "Fifth")
                missingToken = queueDisplay.FifthToken;

            else if (checkQueueResponse == "Sixth")
                missingToken = queueDisplay.SixthToken;

            else if (checkQueueResponse == "Seventh")
                missingToken = queueDisplay.SeventhToken;


            Queue queue = _queueRepository.Where(x => x.TokenNumber == missingToken && DateTime.Now.Day == x.QueueDate.Day && DateTime.Now.Month == x.QueueDate.Month && DateTime.Now.Year == x.QueueDate.Year).FirstOrDefault();
            if(queue != null)
            {
                queue.Status = "Missing";
                _queueRepository.Update(queue);
                await _allServiceHub.Clients.All.SendAsync("NewMissingService", queue.Service);
                return "SUCCESS";
            }
            else
            {
                return "NOT FOUND";
            }
        }

        [HttpGet]
        [Route("GetMissing/{counterId}/{id}")]
        public IActionResult GetMissing(long counterId,int id)
        {
            int page = id;
            long length = 0;

            List<Queue> queues = new List<Queue>();
            Counter counter = _counterRepository.Where(x => x.Id == counterId).FirstOrDefault();
            List<long> counterServices = _counterServiceRepository.Where(x => x.CounterId == counterId).Select(x => x.ServiceId).ToList();
            if (counterServices.Count() > 0)
            {
                List<string> services = _serviceRepository.Where(x => counterServices.Contains(x.Id)).Select(x => x.Name).ToList();
                if (services.Count() > 0)
                {
                    queues = _queueRepository.Where(x => x.Status == "Missing" && services.Contains(x.Service) && DateTime.Now.Day == x.QueueDate.Day && DateTime.Now.Month == x.QueueDate.Month && DateTime.Now.Year == x.QueueDate.Year).OrderBy(x => x.Row).Skip((page - 1) * 10).Take(10).ToList();
                    length = _queueRepository.Where(x => x.Status == "Missing" && services.Contains(x.Service) && DateTime.Now.Day == x.QueueDate.Day && DateTime.Now.Month == x.QueueDate.Month && DateTime.Now.Year == x.QueueDate.Year).OrderBy(x => x.Row).Count();
                }
            }
            return Ok(new { Data = queues, Length = length });
        }

        [HttpGet]
        [Route("AllServices")]
        public List<Service> AllServices()
        {
            return _serviceRepository.Where(x => x.Id != 0).ToList();
        }

        [HttpPost]
        [Route("TransferCustomer")]
        public async Task<string> TransferCustomer([FromBody] TransferCustomer transferCustomer)
        {
            Queue queue = _queueRepository.Where(x => x.TokenNumber == transferCustomer.Token && DateTime.Now.Day == x.QueueDate.Day && DateTime.Now.Month == x.QueueDate.Month && DateTime.Now.Year == x.QueueDate.Year).FirstOrDefault();
            if(queue != null)
            {
                queue.Status = "Transfered";
                queue.TransferReason = transferCustomer.TransferReason;
                _queueRepository.Update(queue);

                Queue newQueue = new Queue()
                {
                    EmployeeId = "-",
                    EmployeeName = "-",
                    QueueDate = DateTime.Now,
                    Row = queue.Row,
                    Service = transferCustomer.Service,
                    Status = "Pending",
                    TokenNumber = queue.TokenNumber,
                    HasPreference = true
                };
                await _allServiceHub.Clients.All.SendAsync("NewService", transferCustomer.Service);
                _queueRepository.Add(newQueue);
                return queue.TokenNumber;

            }
            return "ERROR";
        }

        [HttpGet]
        [Route("NextTurn/{counterId}")]
        public async Task<string> NextTurn(long counterId)
        {

            await semaphoreNext.WaitAsync();
            try
            {
                Queue openedQueue = _queueRepository.Where(x => x.Status == "Processed" && x.CounterId == counterId && x.ClosedTime == null && DateTime.Now.Day == x.QueueDate.Day && DateTime.Now.Month == x.QueueDate.Month && DateTime.Now.Year == x.QueueDate.Year && x.CalledTime != null).FirstOrDefault();
                if (openedQueue != null)
                {
                    DateTime calledDate = openedQueue.QueueDate.AddMilliseconds(openedQueue.CalledTime.Value.TotalMilliseconds);
                    openedQueue.ClosedTime = DateTime.Now - calledDate;
                    _queueRepository.Update(openedQueue);
                }

                string response = "COMPLETE";
                Counter counter = _counterRepository.Where(x => x.Id == counterId).FirstOrDefault();
                QueueDisplay queueDisplay = _queueDisplayRepository.GetAll().FirstOrDefault();
                string checkQueueResponse = CheckQueue(counter.CounterNumber, queueDisplay);
                if (checkQueueResponse == "First")
                    queueDisplay = SwitchFirst(queueDisplay);
                else if (checkQueueResponse == "Second")
                    queueDisplay = SwitchSecond(queueDisplay);
                else if (checkQueueResponse == "Third")
                    queueDisplay = SwitchThird(queueDisplay);
                else if (checkQueueResponse == "Fourth")
                    queueDisplay = SwitchFourth(queueDisplay);
                else if (checkQueueResponse == "Fifth")
                    queueDisplay = SwitchFifth(queueDisplay);
                else if (checkQueueResponse == "Sixth")
                    queueDisplay = SwitchSixth(queueDisplay);
                else if (checkQueueResponse == "Seventh")
                    queueDisplay = SwitchSeventh(queueDisplay);

                List<long> counterServices = _counterServiceRepository.Where(x => x.CounterId == counterId).Select(x => x.ServiceId).ToList();
                if (counterServices.Count() > 0)
                {
                    List<string> services = _serviceRepository.Where(x => counterServices.Contains(x.Id)).Select(x => x.Name).ToList();
                    List<string> preferenceServices = _serviceRepository.Where(x => counterServices.Contains(x.Id) && x.HasPreference).Select(x => x.Name).ToList();
                    if (services.Count() > 0)
                    {
                        Queue queue;
                        if (preferenceServices.Count() > 0)
                            queue = _queueRepository.Where(x => x.Status == "Pending" && preferenceServices.Contains(x.Service) && DateTime.Now.Day == x.QueueDate.Day && DateTime.Now.Month == x.QueueDate.Month && DateTime.Now.Year == x.QueueDate.Year).OrderBy(x => x.Row).FirstOrDefault();
                        else
                            queue = _queueRepository.Where(x => x.Status == "Pending" && services.Contains(x.Service) && DateTime.Now.Day == x.QueueDate.Day && DateTime.Now.Month == x.QueueDate.Month && DateTime.Now.Year == x.QueueDate.Year).OrderBy(x => x.Row).FirstOrDefault();

                        if (queue is null)
                            queue = _queueRepository.Where(x => x.Status == "Pending" && x.HasPreference && services.Contains(x.Service) && DateTime.Now.Day == x.QueueDate.Day && DateTime.Now.Month == x.QueueDate.Month && DateTime.Now.Year == x.QueueDate.Year).OrderBy(x => x.Row).FirstOrDefault();

                        if (queue is null)
                            queue = _queueRepository.Where(x => x.Status == "Pending" && services.Contains(x.Service) && DateTime.Now.Day == x.QueueDate.Day && DateTime.Now.Month == x.QueueDate.Month && DateTime.Now.Year == x.QueueDate.Year).OrderBy(x => x.Row).FirstOrDefault();

                        if (queue != null)
                        {


                            response = queue.TokenNumber;
                            queueDisplay = SetQueue(counter.CounterNumber, queue.TokenNumber, queueDisplay);
                            queue.Status = "Processed";
                            queue.CounterId = counter.Id;

                            queue.CalledTime = DateTime.Now - queue.QueueDate;
                            var employee = _employeeRepository.Where(e => e.CounterId == counter.Id).First();
                            queue.EmployeeId = employee.EmployeeId;
                            queue.EmployeeName = employee.EmployeeName;

                            _queueRepository.Update(queue);


                            _queueDisplayRepository.Update(queueDisplay);
                            await _queueHub.Clients.All.SendAsync("Queue", "fade");
                            await _allServiceHub.Clients.All.SendAsync("ServedService", queue.Service);

                            int tokenNumber = int.Parse(queue.TokenNumber);
                            int counterNumber = int.Parse(counter.CounterNumber);
                            PlayAudio(tokenNumber, counterNumber);

                            return response;
                        }
                    }
                    return response;
                }
                else
                {
                    _queueDisplayRepository.Update(queueDisplay);
                    await _queueHub.Clients.All.SendAsync("Queue", response);
                    return response;
                }

            }
            finally
            {
                semaphoreNext.Release();
            }
            
        }

        [HttpGet]
        [Route("Close/{counterId}")]
        public async Task<string> Close(long counterId)
        {
            await semaphoreClose.WaitAsync();
            try
            {
                string response = "COMPLETE";
                Counter counter = _counterRepository.Where(x => x.Id == counterId).FirstOrDefault();
                QueueDisplay queueDisplay = _queueDisplayRepository.GetAll().FirstOrDefault();
                Queue openedQueue = _queueRepository.Where(x => x.Status == "Processed" && x.CounterId == counterId && x.ClosedTime == null && DateTime.Now.Day == x.QueueDate.Day && DateTime.Now.Month == x.QueueDate.Month && DateTime.Now.Year == x.QueueDate.Year && x.CalledTime != null).FirstOrDefault();
                if(openedQueue != null)
                {
                    DateTime calledDate = openedQueue.QueueDate.AddMilliseconds(openedQueue.CalledTime.Value.TotalMilliseconds);
                    openedQueue.ClosedTime = DateTime.Now - calledDate;
                    _queueRepository.Update(openedQueue);
                }
                string checkQueueResponse = CheckQueue(counter.CounterNumber, queueDisplay);
                if (checkQueueResponse == "First")
                    queueDisplay = SwitchFirst(queueDisplay);
                else if (checkQueueResponse == "Second")
                    queueDisplay = SwitchSecond(queueDisplay);
                else if (checkQueueResponse == "Third")
                    queueDisplay = SwitchThird(queueDisplay);
                else if (checkQueueResponse == "Fourth")
                    queueDisplay = SwitchFourth(queueDisplay);
                else if (checkQueueResponse == "Fifth")
                    queueDisplay = SwitchFifth(queueDisplay);
                else if (checkQueueResponse == "Sixth")
                    queueDisplay = SwitchSixth(queueDisplay);
                else if (checkQueueResponse == "Seventh")
                    queueDisplay = SwitchSeventh(queueDisplay);

                _queueDisplayRepository.Update(queueDisplay);
                await _queueHub.Clients.All.SendAsync("Queue", response);
                return response;
            }
            finally
            {
                semaphoreClose.Release();
            }

        }

        [HttpGet]
        [Route("MissingTurn/{counterId}")]
        public async Task<string> MissingTurn(long counterId)
        {
            await semaphoreMissing.WaitAsync();
            try
            {
                Counter counter = _counterRepository.Where(x => x.Id == counterId).FirstOrDefault();
                Queue queue = _queueRepository.Where(x => x.CounterId == counter.Id).OrderByDescending(x => x.Id).FirstOrDefault();

                QueueDisplay queueDisplay = _queueDisplayRepository.GetAll().FirstOrDefault();
                string checkQueueResponse = CheckQueue(counter.CounterNumber, queueDisplay);
                if (checkQueueResponse == "First")
                {
                    await _queueHub.Clients.All.SendAsync("Queue", "first-missing");
                    int tokenNumber = int.Parse(queueDisplay.FirstToken);
                    int counterNumber = int.Parse(counter.CounterNumber);

                    
                    PlayAudio(tokenNumber, counterNumber);
                }
                else if (checkQueueResponse == "Second")
                {
                    await _queueHub.Clients.All.SendAsync("Queue", "second-missing");
                    int tokenNumber = int.Parse(queueDisplay.SecondToken);
                    int counterNumber = int.Parse(counter.CounterNumber);
                    PlayAudio(tokenNumber, counterNumber);
                }
                else if (checkQueueResponse == "Third")
                {
                    await _queueHub.Clients.All.SendAsync("Queue", "third-missing");
                    int tokenNumber = int.Parse(queueDisplay.ThirdToken);
                    int counterNumber = int.Parse(counter.CounterNumber);
                    PlayAudio(tokenNumber, counterNumber);
                }
                else if (checkQueueResponse == "Fourth")
                {
                    await _queueHub.Clients.All.SendAsync("Queue", "fourth-missing");
                    int tokenNumber = int.Parse(queueDisplay.FourthToken);
                    int counterNumber = int.Parse(counter.CounterNumber);
                    PlayAudio(tokenNumber, counterNumber);
                }
                else if (checkQueueResponse == "Fifth")
                {
                    await _queueHub.Clients.All.SendAsync("Queue", "fifth-missing");
                    int tokenNumber = int.Parse(queueDisplay.FifthToken);
                    int counterNumber = int.Parse(counter.CounterNumber);
                    PlayAudio(tokenNumber, counterNumber);
                }
                else if (checkQueueResponse == "Sixth")
                {
                    await _queueHub.Clients.All.SendAsync("Queue", "sixth-missing");
                    int tokenNumber = int.Parse(queueDisplay.SixthToken);
                    int counterNumber = int.Parse(counter.CounterNumber);
                    PlayAudio(tokenNumber, counterNumber);
                }
                else if (checkQueueResponse == "Seventh")
                {
                    await _queueHub.Clients.All.SendAsync("Queue", "seventh-missing");
                    int tokenNumber = int.Parse(queueDisplay.SeventhToken);
                    int counterNumber = int.Parse(counter.CounterNumber);
                    PlayAudio(tokenNumber, counterNumber);
                }
                else
                {
                    int tokenNumber = int.Parse(queue.TokenNumber);
                    int counterNumber = int.Parse(counter.CounterNumber);
                    PlayAudio(tokenNumber, counterNumber);
                    
                }
                return "CALLED";
            }
            finally
            {
                semaphoreMissing.Release();
            }

        }

        [HttpGet]
        [Route("GetQueueForCounter/{counterId}/{id}")]
        public IActionResult GetQueueForCounter(long counterId,int id)
        {
            int page = id;
            long length = 0;

            List<Queue> queues = new List<Queue>();
            Counter counter = _counterRepository.Where(x => x.Id == counterId).FirstOrDefault();
            List<long> counterServices = _counterServiceRepository.Where(x => x.CounterId == counterId).Select(x => x.ServiceId).ToList();
            if (counterServices.Count() > 0)
            {
                List<string> services = _serviceRepository.Where(x => counterServices.Contains(x.Id)).Select(x => x.Name).ToList();
                if (services.Count() > 0)
                {
                    queues = _queueRepository.Where(x => x.Status == "Pending" && services.Contains(x.Service) && DateTime.Now.Day == x.QueueDate.Day && DateTime.Now.Month == x.QueueDate.Month && DateTime.Now.Year == x.QueueDate.Year).OrderBy(x => x.Row).Skip((page - 1) * 10).Take(10).ToList();
                    length = _queueRepository.Where(x => x.Status == "Pending" && services.Contains(x.Service) && DateTime.Now.Day == x.QueueDate.Day && DateTime.Now.Month == x.QueueDate.Month && DateTime.Now.Year == x.QueueDate.Year).OrderBy(x => x.Row).Count();
                }
            }
            return Ok(new { Data = queues, Length = length });
        }

        [HttpGet]
        [Route("OutOfOrder/{counterId}/{queueId}")]
        public async Task<string> OutOfOrder(long counterId,long queueId)
        {
            await semaphoreOutOfOrder.WaitAsync();
            try
            {
                Queue openedQueue = _queueRepository.Where(x => x.Status == "Processed" && x.CounterId == counterId && x.ClosedTime == null && DateTime.Now.Day == x.QueueDate.Day && DateTime.Now.Month == x.QueueDate.Month && DateTime.Now.Year == x.QueueDate.Year && x.CalledTime != null).FirstOrDefault();
                if (openedQueue != null)
                {
                    DateTime calledDate = openedQueue.QueueDate.AddMilliseconds(openedQueue.CalledTime.Value.TotalMilliseconds);
                    openedQueue.ClosedTime = DateTime.Now - calledDate;
                    _queueRepository.Update(openedQueue);
                }

                string response = "COMPLETE";
                string signalResponse = "COMPLETE";
                Counter counter = _counterRepository.Where(x => x.Id == counterId).FirstOrDefault();
                QueueDisplay queueDisplay = _queueDisplayRepository.GetAll().FirstOrDefault();
                string checkQueueResponse = CheckQueue(counter.CounterNumber, queueDisplay);
                if (checkQueueResponse == "First")
                    queueDisplay = SwitchFirst(queueDisplay);
                else if (checkQueueResponse == "Second")
                    queueDisplay = SwitchSecond(queueDisplay);
                else if (checkQueueResponse == "Third")
                    queueDisplay = SwitchThird(queueDisplay);
                else if (checkQueueResponse == "Fourth")
                    queueDisplay = SwitchFourth(queueDisplay);
                else if (checkQueueResponse == "Fifth")
                    queueDisplay = SwitchFifth(queueDisplay);
                else if (checkQueueResponse == "Sixth")
                    queueDisplay = SwitchSixth(queueDisplay);
                else if (checkQueueResponse == "Seventh")
                    queueDisplay = SwitchSeventh(queueDisplay);

                List<long> counterServices = _counterServiceRepository.Where(x => x.CounterId == counterId).Select(x => x.ServiceId).ToList();
                if (counterServices.Count() > 0)
                {
                    List<string> services = _serviceRepository.Where(x => counterServices.Contains(x.Id)).Select(x => x.Name).ToList();
                    if (services.Count() > 0)
                    {
                        Queue queue = _queueRepository.Where(x => x.Id == queueId && (x.Status == "Pending" || x.Status == "Missing") && services.Contains(x.Service)).OrderBy(x => x.Row).FirstOrDefault();
                        if (queue != null)
                        {


                            signalResponse = "first-missing";
                            response = queue.TokenNumber;
                            queueDisplay = SetQueue(counter.CounterNumber, queue.TokenNumber, queueDisplay);
                            Employee employee = _employeeRepository.Where(x => x.CounterId == counter.Id).FirstOrDefault();
                            if (employee != null)
                            {
                                queue.EmployeeId = employee.EmployeeId;
                                queue.EmployeeName = employee.EmployeeName;
                            }
                            queue.Status = "Processed";
                            queue.CounterId = counter.Id;
                            queue.CalledTime = DateTime.Now - queue.QueueDate;
                            _queueRepository.Update(queue);


                            int tokenNumber = int.Parse(queue.TokenNumber);
                            int counterNumber = int.Parse(counter.CounterNumber);

                            queue.TokenNumber = 
                            _queueDisplayRepository.Update(queueDisplay);
                            await _queueHub.Clients.All.SendAsync("Queue", signalResponse);
                            if(queue.Status == "Pending")
                                await _allServiceHub.Clients.All.SendAsync("ServedService", queue.Service);
                            else
                                await _allServiceHub.Clients.All.SendAsync("ServedMissingService", queue.Service);

                            PlayAudio(tokenNumber, counterNumber);
                        }
                    }
                }

                return response;
            }
            finally
            {
                semaphoreOutOfOrder.Release();
            }

        }

        [HttpGet]
        [Route("GetServices")]
        public List<Service> GetServices()
        {
            return _serviceRepository.GetAll().ToList();
        }

        [HttpGet]
        [Route("GetServiceForCounter/{counterId}")]
        public List<Service> GetServiceForCounter(long counterId)
        {
            List<long> serviceIds = _counterServiceRepository.Where(x => x.CounterId == counterId).Select(x => x.ServiceId).ToList();
            if (serviceIds.Count() > 0)
                return _serviceRepository.Where(x => serviceIds.Contains(x.Id)).ToList();
            else
                return new List<Service>();
        }

        [HttpGet]
        [Route("GetGroups")]
        public List<ServiceGroup> GetGroups()
        {
            return _serviceGroupRepository.GetAll().ToList();
        }

        [HttpGet]
        [Route("GenerateToken/{serviceId}")]
        public async Task<TokenResponse> GenerateToken(long serviceId)
        {
            Service service = _serviceRepository.Where(x => x.Id == serviceId).FirstOrDefault();
            Token token = _tokenRepository.GetAll().FirstOrDefault();
            int maxRow = 1;
            if(token != null)
            {
                maxRow = _queueRepository.Where(x => x.Id != 0).Max(x =>(int?) x.Row) ?? +1;

                string response = CheckToken(token);
                token.TokenDate = DateTime.Now;

                if (response == "RESET")
                    token.TokenNumber = 1;
                else
                    token.TokenNumber += 1;

                if (token.TokenNumber > 999)
                    token.TokenNumber = 1;
            }


            Queue queue = new Queue()
            {
                QueueDate = DateTime.Now,
                Row = maxRow,
                Service = service.Name,
                Status = "Pending",
                EmployeeId = "-",
                EmployeeName = "-",
                TokenNumber = token.TokenNumber.ToString("000")
            };
            await _allServiceHub.Clients.All.SendAsync("NewService", service.Name);
            _tokenRepository.Update(token);
            _queueRepository.Add(queue);
            TokenResponse tokenResponse = new TokenResponse();
            tokenResponse.tokens.Add(new Ticket() { token = queue.TokenNumber });
            return tokenResponse;
        }

        private string CheckToken(Token token)
        {
            DateTime tokenDate = new DateTime(token.TokenDate.Year, token.TokenDate.Month, token.TokenDate.Day);
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            if (tokenDate != currentDate)
            {
                return "RESET";
            }
            else
                 return "NO RESET";
        }

        private void CheckDate()
        {
            Token token = _tokenRepository.Where(x => x.Id != 0).FirstOrDefault();
            DateTime tokenDate = new DateTime(token.TokenDate.Year, token.TokenDate.Month, token.TokenDate.Day);
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            if (tokenDate != currentDate)
            {
                QueueDisplay queueDisplay = _queueDisplayRepository.Where(x => x.Id != 0).FirstOrDefault();
                queueDisplay.FirstCounter = "";
                queueDisplay.FirstToken = "";
                queueDisplay.SecondToken = "";
                queueDisplay.SecondCounter = "";
                queueDisplay.ThirdCounter = "";
                queueDisplay.ThirdToken = "";
                queueDisplay.FourthCounter = "";
                queueDisplay.FourthToken = "";
                queueDisplay.FifthCounter = "";
                queueDisplay.FifthToken = "";
                queueDisplay.SixthCounter = "";
                queueDisplay.SixthToken = "";
                queueDisplay.SeventhCounter = "";
                queueDisplay.SeventhToken = "";
                _queueDisplayRepository.Update(queueDisplay);
            }
        }

        private void PlayAudio(int token = 0,int counter = 0)
        {
            lock (LockObject)
            {
                string ftpPath = @"C:\ftp\VOICES\";
                var player = new Player();
                var tokenNumber = new Player();
                var proceedToCounter = new Player();
                var counterNumber = new Player();

                int proceedToTokenDuration = GetDuration("TokenNumber.mp3") + 500;
                int tokenNumberDuration = GetDuration(token + ".mp3") + 500;
                int proceedToCounterDuration = GetDuration("ProceedToCounter.mp3") + 500;
                int counterNumberDuration = GetDuration(counter + ".mp3") + 500;

                try
                {
                    player.Play(ftpPath + "TokenNumber.mp3").Wait();
                    Thread.Sleep(proceedToTokenDuration);
                    player.Stop();

                    player.Play(ftpPath + token + ".mp3").Wait();
                    Thread.Sleep(tokenNumberDuration);
                    player.Stop();

                    player.Play(ftpPath + "ProceedToCounter.mp3").Wait();
                    Thread.Sleep(proceedToCounterDuration);
                    player.Stop();

                    player.Play(ftpPath + counter + ".mp3").Wait();
                    Thread.Sleep(counterNumberDuration);
                    player.Stop();
                }
                catch (Exception ex)
                {

                }
            }
        }

        private int GetDuration(string fileName)
        {
            var inputFile = new MediaFile { Filename = @"C:\ftp\VOICES\" +  fileName };

            using (var engine = new Engine(@"wwwroot\FFMPEG.exe"))
            {
                engine.GetMetadata(inputFile);
            }
            return (int)(inputFile.Metadata.Duration.TotalSeconds * 1000);
        }

        private QueueDisplay SetQueue(string counter, string tokenNumber,QueueDisplay queueDisplay)
        {
            if (!string.IsNullOrEmpty(queueDisplay.SixthCounter))
            {
                queueDisplay.SeventhCounter = queueDisplay.SixthCounter;
                queueDisplay.SeventhToken = queueDisplay.SixthToken;
            }
            if (!string.IsNullOrEmpty(queueDisplay.FifthCounter))
            {
                queueDisplay.SixthCounter = queueDisplay.FifthCounter;
                queueDisplay.SixthCounter = queueDisplay.FifthToken;
            }
            if (!string.IsNullOrEmpty(queueDisplay.FourthCounter))
            {
                queueDisplay.FifthCounter = queueDisplay.FourthCounter;
                queueDisplay.FifthToken = queueDisplay.FourthToken;
            }
            if (!string.IsNullOrEmpty(queueDisplay.ThirdCounter))
            {
                queueDisplay.FourthCounter = queueDisplay.ThirdCounter;
                queueDisplay.FourthToken = queueDisplay.ThirdToken;
            }
            if (!string.IsNullOrEmpty(queueDisplay.SecondCounter))
            {
                queueDisplay.ThirdCounter = queueDisplay.SecondCounter;
                queueDisplay.ThirdToken = queueDisplay.SecondToken;
            }
            if (!string.IsNullOrEmpty(queueDisplay.FirstCounter))
            {
                queueDisplay.SecondCounter = queueDisplay.FirstCounter;
                queueDisplay.SecondToken = queueDisplay.FirstToken;
            }
            queueDisplay.FirstCounter = counter;
            queueDisplay.FirstToken = tokenNumber;
            return queueDisplay;
        }

        private string CheckQueue(string counter,QueueDisplay queueDisplay)
        {
            
            if (!string.IsNullOrEmpty(queueDisplay.FirstCounter) && queueDisplay.FirstCounter == counter)
            {
                return "First";
            }
            if (!string.IsNullOrEmpty(queueDisplay.SecondCounter) && queueDisplay.SecondCounter == counter)
            {
                return "Second";
            }
            if (!string.IsNullOrEmpty(queueDisplay.ThirdCounter) && queueDisplay.ThirdCounter == counter)
            {
                return "Third";
            }
            if (!string.IsNullOrEmpty(queueDisplay.FourthCounter) && queueDisplay.FourthCounter == counter)
            {
                return "Fourth";
            }
            if (!string.IsNullOrEmpty(queueDisplay.FifthCounter) && queueDisplay.FifthCounter == counter)
            {
                return "Fifth";
            }
            if (!string.IsNullOrEmpty(queueDisplay.SixthCounter) && queueDisplay.SixthCounter == counter)
            {
                return "Sixth";
            }
            if (!string.IsNullOrEmpty(queueDisplay.SeventhCounter) && queueDisplay.SeventhCounter == counter)
            {
                return "Seventh";
            }
            return "NOT EXIST";
        }

        private QueueDisplay SwitchFirst(QueueDisplay queueDisplay)
        {
            queueDisplay.FirstCounter = queueDisplay.SecondCounter;
            queueDisplay.FirstToken = queueDisplay.SecondToken;
            queueDisplay = SwitchSecond(queueDisplay);
            return queueDisplay;

        }

        private QueueDisplay SwitchSecond(QueueDisplay queueDisplay)
        {
            queueDisplay.SecondCounter = queueDisplay.ThirdCounter;
            queueDisplay.SecondToken = queueDisplay.ThirdToken;
            queueDisplay = SwitchThird(queueDisplay);
            return queueDisplay;

        }

        private QueueDisplay SwitchThird(QueueDisplay queueDisplay)
        {
            queueDisplay.ThirdCounter = queueDisplay.FourthCounter;
            queueDisplay.ThirdToken = queueDisplay.FourthToken;
            queueDisplay = SwitchFourth(queueDisplay);
            return queueDisplay;

        }

        private QueueDisplay SwitchFourth(QueueDisplay queueDisplay)
        {
            queueDisplay.FourthCounter = queueDisplay.FifthCounter;
            queueDisplay.FourthToken = queueDisplay.FifthToken;
            queueDisplay = SwitchFifth(queueDisplay);
            return queueDisplay;

        }

        private QueueDisplay SwitchFifth(QueueDisplay queueDisplay)
        {
            queueDisplay.FifthCounter = queueDisplay.SixthCounter;
            queueDisplay.FifthToken = queueDisplay.SixthToken;
            queueDisplay = SwitchSixth(queueDisplay);
            return queueDisplay;

        }

        private QueueDisplay SwitchSixth(QueueDisplay queueDisplay)
        {
            queueDisplay.SixthCounter = queueDisplay.SeventhCounter;
            queueDisplay.SixthToken = queueDisplay.SeventhToken;
            queueDisplay = SwitchSeventh(queueDisplay);
            return queueDisplay;

        }

        private QueueDisplay SwitchSeventh(QueueDisplay queueDisplay)
        {
            queueDisplay.SeventhCounter = "";
            queueDisplay.SeventhToken = "";
            return queueDisplay;
        }


        
    }
}