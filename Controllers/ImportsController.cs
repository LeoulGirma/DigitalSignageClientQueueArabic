using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DeviceId;
using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using DigitalSignageClient.Models;
using DSLicense;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSignageClient.Controllers
{
    [Produces("application/json")]
    [Route("api/Imports")]
    public class ImportsController : Controller
    {
        private readonly ICurrencyViewRepository _currencyViewRepository;
        private readonly IScheduleViewRepository _scheduleViewRepository;
        private readonly IVideoViewRepository _videoViewRepository;
        private readonly ICalendarViewRepository _calendarViewRepository;
        private readonly INavStyleViewRepository _navStyleViewRepository;
        private readonly IBodyStyleViewRepository _bodyStyleViewRepository;
        private readonly IRSSStyleViewRepository _rssStyleViewRepository;
        private readonly IRSSNewsViewRepository _rssNewsViewRepository;
        private readonly ILicenseStatusRepository _licenseStatusRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceGroupRepository _groupServiceRepository;
        private readonly ICounterRepository _counterRepository;
        private readonly ICounterServiceRepository _counterServiceRepository;
        private readonly IQueueRepository _queueRepository;
        public ImportsController(ICurrencyViewRepository currencyViewRepository, IScheduleViewRepository scheduleViewRepository, IVideoViewRepository videoViewRepository,
            ICalendarViewRepository calendarViewRepository, INavStyleViewRepository navStyleViewRepository, IBodyStyleViewRepository bodyStyleViewRepository,
            IRSSNewsViewRepository rSSNewsViewRepository, IRSSStyleViewRepository rSSStyleViewRepository, ILicenseStatusRepository licenseStatusRepository,
            IEmployeeRepository employeeRepository, IServiceRepository serviceRepository, IServiceGroupRepository serviceGroupRepository, 
            ICounterRepository counterRepository, ICounterServiceRepository counterServiceRepository, IQueueRepository queueRepository)
        {
            _currencyViewRepository = currencyViewRepository;
            _scheduleViewRepository = scheduleViewRepository;
            _videoViewRepository = videoViewRepository;
            _calendarViewRepository = calendarViewRepository;
            _navStyleViewRepository = navStyleViewRepository;
            _bodyStyleViewRepository = bodyStyleViewRepository;
            _rssNewsViewRepository = rSSNewsViewRepository;
            _rssStyleViewRepository = rSSStyleViewRepository;
            _licenseStatusRepository = licenseStatusRepository;
            _employeeRepository = employeeRepository;
            _serviceRepository = serviceRepository;
            _counterRepository = counterRepository;
            _counterServiceRepository = counterServiceRepository;
            _queueRepository = queueRepository;
            _groupServiceRepository = serviceGroupRepository;
        }

        [HttpGet]
        [Route("ServicesForAndroid")]
        public ServiceForAndroid ServicesForAndroid()
        {
            ServiceForAndroid service = new ServiceForAndroid();
            List<Service> services = _serviceRepository.Where(x => x.Id != 0).ToList();
            foreach (var item in services)
            {
                service.service.Add(new ServiceClient() { Id = item.Id, Name = item.DisplayName, GroupId = item.GroupId });
            }
            return service;
        }

        [HttpGet]
        [Route("ServicesForAndroidByGroupId{id}")]
        public ServiceForAndroid ServicesForAndroidByGroupId(long id)
        {
            ServiceForAndroid service = new ServiceForAndroid();
            List<Service> services = _serviceRepository.Where(x => x.GroupId != id).ToList();
            foreach (var item in services)
            {
                service.service.Add(new ServiceClient() { Id = item.Id, Name = item.DisplayName, GroupId = item.GroupId });
            }
            return service;
        }

        [HttpGet]
        [Route("ServicesForAndroidByGroupId{id}")]
        public ServiceForAndroid ServicesForWindowsByGroupId(long id)
        {
            ServiceForAndroid service = new ServiceForAndroid();
            List<Service> services = _serviceRepository.Where(x => x.GroupId != id).ToList();
            foreach (var item in services)
            {
                service.service.Add(new ServiceClient() { Id = item.Id, Name = item.DisplayName, GroupId = item.GroupId });
            }
            return service;
        }

        [HttpGet]
        [Route("GroupsForAndroid")]
        public GroupForAndroid GroupsForAndroid()
        {
            GroupForAndroid group = new GroupForAndroid();
            List<ServiceGroup> groups = _groupServiceRepository.Where(x => x.Id != 0).ToList();
            if(groups.Count() > 1)
            {
                foreach (var item in groups)
                {
                    group.group.Add(new GroupClient() { Id = item.Id, Name = item.Name });
                }
            }

            return group;
        }

        //start
        [HttpPost]
        [Route("ImportStyle")]
        public bool ImportStyle([FromBody] ImportStyle import)
        {
           // if (new LicensesController().CheckExpireDate() == false)
           //     return false;

            List<NavStyleView> navStyle = import.NavStyleView;
            List<BodyStyleView> bodyStyle = import.BodyStyleView;

            List<NavStyleView> oldNavStyle = _navStyleViewRepository.GetAll(x => x.Id != 0).ToList();
            List<BodyStyleView> oldBodyStyle = _bodyStyleViewRepository.GetAll(x => x.Id != 0).ToList();

            List<VideoView> newVideo = new List<VideoView>();
            if (bodyStyle.Count() == 0 || navStyle.Count() == 0 )
            {
                return false;
            }
            foreach (var item in navStyle)
            {
                item.Id = 0;
                bool ans = Move(item.HeaderLogoIcon, "Logo");
                if (!ans)
                    return false;
                ans = Move(item.HeaderLogoText, "Logo");
                if (!ans)
                    return false;
                ans = Move(item.FooterLogo, "Logo");
                if (!ans)
                    return false;
            }
            foreach (var item in bodyStyle)
            {
                item.Id = 0;
            }


            _bodyStyleViewRepository.AddRange(bodyStyle);
            _navStyleViewRepository.AddRange(navStyle);

            _navStyleViewRepository.RemoveRange(oldNavStyle);
            _bodyStyleViewRepository.RemoveRange(oldBodyStyle);

            List<RSSStyleView> views = _rssStyleViewRepository.GetAll().ToList();
            string[] files = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\lib\Project\Logo\"));
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                if (navStyle.Count(x => x.HeaderLogoText == fileName || x.HeaderLogoIcon == fileName || x.FooterLogo == fileName || x.HeaderBackgroundImage == fileName) > 0 || views.Count(x => x.LogoPath == fileName) > 0)
                {
                    continue;
                }
                System.IO.File.Delete(file);
            }

            return true;
        }
        
        [HttpPost]
        [Route("ImportRSS")]
        public string ImportRSS([FromBody] ImportRSS rss)
        {
            //if (new LicensesController().CheckExpireDate() == false)
            //    return "No License";

            if (rss.RSSStyleViews.Count() == 0 || rss.RSSNewsViews.Count() == 0)
            {
                return "Empty";
            }
            List<RSSNewsView> rssNews = _rssNewsViewRepository.GetAll().ToList();
            List<RSSStyleView> rssStyle = _rssStyleViewRepository.GetAll().ToList();
            try
            {
                foreach (var item in rss.RSSStyleViews)
                {
                    item.Id = 0;
                    bool ans = Move(item.LogoPath, "Logo");
                    if (!ans)
                        return "Logo Not Found";
                }
                List<RSSNewsView> newRssNews = new List<RSSNewsView>();
                foreach (var item in rss.RSSNewsViews)
                {
                    if (rss.RSSStyleViews.Count(x => x.StyleId == item.RSSStyleId) > 0)
                    {
                        item.Id = 0;
                        newRssNews.Add(item);
                    }
                }
                if (newRssNews.Count() == 0)
                {
                    return "Empty";
                }
                _rssStyleViewRepository.AddRange(rss.RSSStyleViews.OrderBy(x => x.RowNumber).ToList());
                _rssNewsViewRepository.AddRange(newRssNews.OrderBy(x => x.RowNumber).ToList());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            _rssNewsViewRepository.RemoveRange(rssNews);
            _rssStyleViewRepository.RemoveRange(rssStyle);
            List<string> rssFiles = rss.RSSStyleViews.Select(x => x.LogoPath).ToList();
            //List<RSSStyleView> views = _rssStyleViewRepository.GetAll().ToList();
            string[] files = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\lib\Project\Logo\"));
            List<NavStyleView> navStyle = _navStyleViewRepository.GetAll().ToList();
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                if (navStyle.Count(x => x.HeaderLogoText == fileName || x.HeaderLogoIcon == fileName || x.FooterLogo == fileName || x.HeaderBackgroundImage == fileName) > 0 || rssFiles.Count(x => x == fileName) > 0)
                {
                    continue;
                }
                System.IO.File.Delete(file);
            }

            return "SUCCESS";
        }
        
        [HttpPost]
        [Route("RemoveVideo")]
        public string RemoveVideo([FromBody]RemoveVideo video)
        {
            string localPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\lib\Project\Video\", video.FileName);
            string ftpPath =  @"C:\ftp\Video\" + video.FileName;
            List<VideoView> schedules = _videoViewRepository.GetAll(x => x.Id != 0).ToList();
            if (String.IsNullOrEmpty(video.FileName))
                return "FILE NAME EMPTY";
            if(schedules.Count() > 0 && schedules.Count(x => x.Name == video.FileName) > 0)
            {
                return "EXIST ON SCHEDULE";
            }
            if (System.IO.File.Exists(localPath))
            {
                try
                {
                    System.IO.File.Delete(localPath);
                }
                catch (Exception ex)
                {

                }
            }
            if (System.IO.File.Exists(ftpPath))
            {
                try
                {
                    System.IO.File.Delete(ftpPath);
                }
                catch (Exception ex)
                {
                    
                }
            }
            return "SUCCESS";
        }

        [HttpPost]
        [Route("ImportQMSData")]
        public bool ImportQMSData([FromBody] SendQMSData import)
        {

            if (import == null || import.Counters.Count() == 0 || import.Services.Count() == 0 || import.Employees.Count() == 0)
            {
                return false;
            }

            List<Counter> counters = _counterRepository.Where(x => x.Id != 0).ToList();
            List<Service> services = _serviceRepository.Where(x => x.Id != 0).ToList();
            List<CounterService> counterServices = _counterServiceRepository.Where(x => x.Id != 0).ToList();
            List<ServiceGroup> serviceGroups = _groupServiceRepository.Where(x => x.Id != 0).ToList();
            List<Employee> employees = _employeeRepository.Where(x => x.Id != 0).ToList();



            _counterRepository.RemoveRange(counters);
            _serviceRepository.RemoveRange(services);
            _counterServiceRepository.RemoveRange(counterServices);
            _groupServiceRepository.RemoveRange(serviceGroups);
            _employeeRepository.RemoveRange(employees);

            _employeeRepository.AddRange(import.Employees);
            _counterRepository.AddRange(import.Counters);
            _serviceRepository.AddRange(import.Services);
            _groupServiceRepository.AddRange(import.ServiceGroups);
            _counterServiceRepository.AddRange(import.CounterServices);

            return true;

        }

        [HttpPost]
        [Route("ImportWeather")]
        public bool ImportWeather([FromBody] ImportWeather import)
        {
            //if (new LicensesController().CheckExpireDate() == false)
            //    return false;


            List<CalendarView> newCalendars = import.CalendarViews.ToList();

            List<CalendarView> oldCalendars = _calendarViewRepository.GetAll(x => x.Id != 0).ToList();

            List<VideoView> newVideo = new List<VideoView>();
            if (newCalendars == null || newCalendars.Count() == 0)
            {
                return false;
            }

            foreach (var item in newCalendars)
            {
                item.Id = 0;
            }


            _calendarViewRepository.AddRange(newCalendars);

            _calendarViewRepository.RemoveRange(oldCalendars);
            return true;
        }

        [HttpPost]
        [Route("ImportCurrency")]
        public bool ImportFromServer([FromBody] ImportCurrency import)
        {
            //if (new LicensesController().CheckExpireDate() == false)
            //    return false;


            List<CurrencyView> newCurrencies = import.CurrencyViews.ToList();
            List<CurrencyView> orderedCurrencies = new List<CurrencyView>();
            List<CurrencyView> oldCurrencies = _currencyViewRepository.GetAll(x => x.Id != 0).ToList();

            List<VideoView> newVideo = new List<VideoView>();
            if (newCurrencies == null || newCurrencies.Count() == 0)
            {
                return false;
            }

            foreach (var item in newCurrencies)
            {
                item.Id = 0;
            }
            // good for Not having duplicate currencies
            //_currencyViewRepository.RemoveRange(oldCurrencies);

            foreach (var item in import.CurrencyViews)
            {
                // What is the purpose of looping trough the original list, search from the copied list then adding to db  
                //just adding the copied will work the same it seems like
                _currencyViewRepository.Add(newCurrencies.FirstOrDefault(x => x.Abbreviation == item.Abbreviation));
                //_currencyViewRepository.Add(newCurrencies.FirstOrDefault(x => x.Flag == item.Flag));
                //orderedCurrencies.Add();
            }

            //_currencyViewRepository.AddRange(orderedCurrencies);

            // good for not deleting the data before adding the new one
            _currencyViewRepository.RemoveRange(oldCurrencies);

            return true;
        }

        [HttpPost]
        [Route("ImportSchedule")]
        public string ImportSchedule([FromBody] ImportSchedule import)
        {

            // if (new LicensesController().CheckExpireDate() == false)
            //     return "No License";



            List<ScheduleView> newSchedule = import.ScheduleViews.ToList();
            List<VideoView> newScheduleVideoView = import.ScheduleVideoViews.ToList();
            //List<VideoView> newVideo = import.ScheduleVideoViews;

            if (newSchedule == null || newScheduleVideoView == null || newScheduleVideoView.Count(x => !x.IsFullScreen) == 0 || newSchedule.Count() == 0)
            {
                return "Schedule is empty";
            }

            List<ScheduleView> oldSchedules = _scheduleViewRepository.GetAll().ToList();
            List<VideoView> oldVideos = _videoViewRepository.GetAll(x => x.Id != 0).ToList();

            foreach (var item in newSchedule)
            {
                item.Id = 0;
            }

            if (newScheduleVideoView.Count() > 0)
            {
                //Decompress();
                foreach (var item in newScheduleVideoView.Where(X => !X.IsStream))
                {
                    bool ans = Move(item.Name, "Video");
                    if (!ans)
                        return "Video Not Found";
                    item.Id = 0;
                }
                foreach (var item in newScheduleVideoView.Where(X => X.IsStream))
                {
                    item.Id = 0;
                }
                CleanUpFTP();
            }


            _scheduleViewRepository.AddRange(newSchedule);
            _videoViewRepository.AddRange(newScheduleVideoView);

            _scheduleViewRepository.RemoveRange(oldSchedules);
            _videoViewRepository.RemoveRange(oldVideos);

            string[] files = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\lib\Project\Video\"));
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                if(newScheduleVideoView.Count(x => x.Name == fileName) == 0)
                {
                    System.IO.File.Delete(file);
                }
            }

            return "SUCCESS";
        }

        private void Decompress()
        {
            string uri = @"C:\ftp\Video";
            DirectoryInfo directorySelected = new DirectoryInfo(uri);
            Compression.DecompressAllFilesInDirectory(directorySelected);
        }

        private void CleanUpFTP()
        {
            string uri = @"C:\ftp\Video";
            DirectoryInfo directorySelected = new DirectoryInfo(uri);

            foreach (FileInfo fileToCompress in directorySelected.GetFiles())
            {
                System.IO.File.Delete(fileToCompress.FullName);
            }
        }

        private bool Move(string ftpFile, string toDownload)
        {
            string sourcePath = @"C:\ftp\" + toDownload + @"\" + ftpFile;
            string destinationPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\lib\Project\" + toDownload + @"\", ftpFile);

            if (!System.IO.File.Exists(destinationPath))
            {
                if (System.IO.File.Exists(sourcePath))
                {
                    try
                    {
                        System.IO.File.Move(sourcePath, destinationPath);
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }


            return true;
        }

        [HttpGet]
        [Route("GetData/{id}")]
        public Models.Data GetData(int id)
        {
            Calendar ec = new Calendar();
            string amharicCalendar = new EthiopicDateTime(DateTime.Now).ToShortDate();
            List<string> dates = amharicCalendar.Split(" ").ToList();
            amharicCalendar = "";
            for (int i = 0; i < dates.Count(); i++)
            {
                if (i + 1 != dates.Count())
                    amharicCalendar += dates[i] + "/";
                else
                    amharicCalendar += dates[i];
            }



            List<CurrencyView> currencies = _currencyViewRepository.GetAll(x => x.Id != 0).OrderBy(x => x.Id).ToList();
            List<ScheduleView> schedules = _scheduleViewRepository.GetAll(x => x.Id != 0).ToList();


            Models.Data data = new Models.Data();
            data.GregorianCalendar = DateTime.Now.ToString("dd/MM/yyyy");

            int staticCurrency = currencies.Count(x => x.Static);
            double notStaticCount = currencies.Count(x => !x.Static);
            int notStaticCurrency = 8 - staticCurrency;
            double check = double.Parse(id.ToString());
            check++;
            double length = double.Parse(notStaticCount.ToString()) / double.Parse(notStaticCurrency.ToString());
            length = Math.Ceiling(length); 
            if(staticCurrency < 8 && notStaticCurrency > 0 && check > length)
            {
                id = 0;
            }
            if(staticCurrency >= 8)
            {
                data.CurrencyViews = currencies.Where(x => x.Static).OrderBy(x => x.Static).Take(6).ToList();
            }
            else 
            {
                List<CurrencyView> c = currencies.Where(x => x.Static).OrderBy(x => x.Static).Take(staticCurrency).ToList();
                c.AddRange(currencies.Where(x => !x.Static).OrderBy(x => !x.Static).Skip(id * notStaticCurrency).Take(notStaticCurrency).ToList());
                data.CurrencyViews = c;
            }
            TimeSpan time = DateTime.Now.TimeOfDay;
            ScheduleView schedule = schedules.Where(x => x.StartTime <= time && x.EndTime > time && x.StartDate <= DateTime.Now.Date && x.EndDate >= DateTime.Now.Date).FirstOrDefault();
            if (schedule == null)
                schedule = schedules.FirstOrDefault(x => x.StartDate == null && x.StartTime <= time && x.EndTime > time);
            if (schedule == null)
                schedule = schedules.FirstOrDefault(x => x.StartTime == new TimeSpan(23,23,23));
            schedules = new List<ScheduleView>();
            schedules.Add(schedule);

            NavStyleView navStyle = _navStyleViewRepository.Where(x => x.TemplateId == schedule.TemplateId).FirstOrDefault();
            BodyStyleView bodyStyle = _bodyStyleViewRepository.GetAll(x => x.TemplateId == schedule.TemplateId).FirstOrDefault();

            data.NavStyle = navStyle;
            data.BodyStyle = bodyStyle;
            data.AmharicCalendar = amharicCalendar;
            data.ScheduleViews = schedules;
            data.Step = id + 1;
            CalendarView calendar = _calendarViewRepository.Where(x => x.Id != 0).FirstOrDefault();
            data.CalendarView = calendar;

            if (CheckExpireDate() == false)
            {
                schedule.Template = "License";
                foreach (var item in data.ScheduleViews)
                {
                    item.Id = 0;
                }
            }

            return data;
        }

        [HttpGet]
        [Route("GetVideoLength/{id}/{full}")]
        public Video GetVideoLength(int id, int full)
        {
            Video video = new Video();
            TimeSpan time = DateTime.Now.TimeOfDay;

            try
            {
                
                ScheduleView schedule = _scheduleViewRepository.Where(x => x.StartTime <= time && x.EndTime > time && x.StartDate <= DateTime.Now.Date && x.EndDate >= DateTime.Now.Date).FirstOrDefault();
                if (schedule == null)
                    schedule = _scheduleViewRepository.GetAll(x => x.Id != 0 && x.StartDate == null && x.StartTime <= time && x.EndTime > time).FirstOrDefault();
                if (schedule == null)
                    schedule = _scheduleViewRepository.GetAll(x => x.Id != 0 && x.StartTime == new TimeSpan(23,23,23)).FirstOrDefault();


                bool isFull = full == 1 ? true : false;
                int length = _videoViewRepository.GetAll(x => x.Id != 0 && x.IsFullScreen == isFull && x.ScheduleId == schedule.ScheduleId).Count();
                if (length == 0)
                {
                    return null;
                }
                if (id >= length)
                {
                    id = 0;
                }
                VideoView view = _videoViewRepository.Where(x => x.Id != 0 && x.IsFullScreen == isFull && x.ScheduleId == schedule.ScheduleId).OrderBy(x => x.RowNumber).Skip(id).FirstOrDefault();
                if (view == null)
                    view = _videoViewRepository.Where(x => x.Id != 0).FirstOrDefault();
                video.Length = view.Duration;
                video.Step = id + 1;
                video.Name = view.Name;
                video.IsVideo = view.IsVideo;
                video.IsStream = view.IsStream;
                return video;
            }
            catch(Exception ex) { }


            video.Length = 2;
            video.Step = 0;
            video.Name = "";
            video.IsVideo = false;
            video.IsStream = false;
            return video;
        }

        [HttpGet]
        [Route("GetRSSNews/{id}")]
        public RSS GetRSSNews(int id)
        {
            int count = _rssNewsViewRepository.GetAll().Count();
            if(count <= id)
            {
                id = 0;
            }
            RSSNewsView rssNews = _rssNewsViewRepository.Where(x => x.Id != 0).OrderBy(x => x.RowNumber).Skip(id).FirstOrDefault();
            RSSStyleView style = _rssStyleViewRepository.Where(x => x.StyleId == rssNews.RSSStyleId).FirstOrDefault();
            RSS rss = new RSS()
            {
                RSSNewsView = rssNews,
                RSSStyleView = style,
                RSSLength = id + 1
            };
            return rss;
        }

        [HttpPost]
        [Route("GetQMSData")]
        public QueueAudit SendAudit([FromBody] QueueAudit queueAudit)
        {
            if(queueAudit != null && queueAudit.ScreenId > 0)
            {
                queueAudit.Queues = _queueRepository.Where(x => x.Id != 0).ToList();
                _queueRepository.RemoveRange(queueAudit.Queues.Where(x => x.Status == "Processed" || x.Status == "Transfered" || ((x.Status == "Missing" || x.Status == "Pending") && !(DateTime.Now.Day == x.QueueDate.Day && DateTime.Now.Month == x.QueueDate.Month && DateTime.Now.Year == x.QueueDate.Year))).ToList());
                queueAudit.Queues.ForEach(x => { x.ScreenId = queueAudit.ScreenId; x.Id = 0; });
            }
            return queueAudit;
        }


        [HttpPost]
        [Route("CheckFile")]
        public CheckIfClientContains CheckFile([FromBody] CheckIfClientContains data)
        {
            data.HasLicense = CheckExpireDate();
            data.ServerUID = GetUID();
            if (data == null)
                return new CheckIfClientContains();
            DateTime lastUpdate = new DateTime();

            data.Exists = false;
            Dictionary<string, bool> files = new Dictionary<string, bool>();
            if(data.Files != null && data.Files.Count() > 0)
            {
                foreach (var item in data.Files)
                {
                    string videoPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\lib\Project\Video\", item.Key);
                    string logoPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\lib\Project\Logo\", item.Key);

                    if (!(System.IO.File.Exists(videoPath) || System.IO.File.Exists(logoPath)))
                    {
                        files.Add(item.Key, false);
                    }
                    else
                    {
                        files.Add(item.Key, true);
                    }
                }
                data.Files = files;
            }

            return data;
        }

        [HttpGet]
        [Route("GetProcessorId")]
        public string GetDeviceId()
        {
            string deviceId = "";
            try
            {
                deviceId = new DeviceIdBuilder()
                    .AddMotherboardSerialNumber()
                    .ToString();

                if(!String.IsNullOrEmpty(deviceId))
                    return deviceId.Substring(1,10);

            }
            catch (Exception ex)
            {
                //return ex.Message;
            }

            try
            {
                deviceId = new DeviceIdBuilder()
                    .AddMachineName()
                    .ToString();
                return deviceId.Substring(1, 10);

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpGet]
        [Route("GetId")]
        public string GetSystemDriveSerialNumber()
        {
            try
            {
                string deviceId = new DeviceIdBuilder()
                    .AddSystemDriveSerialNumber()
                    .ToString();
                return deviceId.Substring(1, 10);

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        [HttpPost]
        [Route("AddLicense")]
        public string AddLicense([FromBody]Data.Model.LicenseStatus screen)
        {
            //// Define a byte array.
            //byte[] bytes = { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 };
            //Console.WriteLine("The byte array: ");
            //Console.WriteLine("   {0}\n", BitConverter.ToString(bytes));

            //// Convert the array to a base 64 string.
            //string s = Convert.ToBase64String(bytes);
            //Console.WriteLine("The base 64 string:\n   {0}\n", s);

            //// Restore the byte array.
            //byte[] newBytes = Convert.FromBase64String(s);
            //Console.WriteLine("The restored byte array: ");
            //Console.WriteLine("   {0}\n", BitConverter.ToString(newBytes));
            try
            {

                string deviceId = GetDeviceId();
                
                screen.ExpireDate = StringCipher.Decrypt(screen.ExpireDate, "L4Y6S1N6S6");
                screen.ServerUID = StringCipher.Decrypt(screen.ServerUID, "L4Y6S1N6S6");
                
                string uid = GetUID();
                
                if (screen.ServerUID != uid)
                    return "ERROR";
                
                DateTime checkParse = DateTime.Parse(screen.ExpireDate);
                
                Data.Model.LicenseStatus licenseStatus = new Data.Model.LicenseStatus();
                licenseStatus.ExpireDate = StringCipher.Encrypt(screen.ExpireDate, deviceId);
                licenseStatus.ServerUID = StringCipher.Encrypt(screen.ServerUID, deviceId);
                
                List<Data.Model.LicenseStatus> licenseStatuses = _licenseStatusRepository.GetAll().ToList();
                _licenseStatusRepository.RemoveRange(licenseStatuses);
                _licenseStatusRepository.Add(licenseStatus);
                
                return "SUCCESS";
            }
            catch(Exception ex)
            {
                return "ERROR";
            }

        }

        public bool CheckExpireDate()
        {
            try
            {
                string deviceId = GetDeviceId();

                Data.Model.LicenseStatus screen = _licenseStatusRepository.Where(x => x.Id != 0).FirstOrDefault();
                if (screen == null)
                    return false;

                screen.ExpireDate = StringCipher.Decrypt(screen.ExpireDate, deviceId);
                screen.ServerUID = StringCipher.Decrypt(screen.ServerUID, deviceId);

                DateTime dateTime = new DateTime();
                DateTime.TryParse(screen.ExpireDate, out dateTime);
                if (dateTime < new DateTime(2000, 01, 01))
                    return false;
                if (dateTime > DateTime.Now)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                return false;
            }

        }

        [HttpPost]
        [Route("RemoveLicense")]
        public string RemoveLicense()
        {
            Data.Model.LicenseStatus screen = _licenseStatusRepository.GetAll().FirstOrDefault();
            if(screen != null)
            {
                _licenseStatusRepository.Remove(screen);
                return "SUCCESS";
            }
            else
            {
                return "NOT ACTIVATED";
            }
        }

        public string GetLicense()
        {
            Data.Model.LicenseStatus screen = _licenseStatusRepository.GetAll().FirstOrDefault();
            if (screen != null)
                return screen.ExpireDate;
            else
                return "NOT ACTIVATED";
        }

        //public bool CheckLicense()
        //{
        //    try
        //    {
        //        string expireDate = GetLicense();
        //        DateTime expire = DateTime.Parse(expireDate);
        //        if (expire > DateTime.Now)
        //        {
        //            return true;
        //        }
        //        else
        //            return false;
        //    }
        //    catch(Exception ex)
        //    {
        //        return false;
        //    }

        //}

        [HttpGet]
        [Route("GetUID")]
        public string GetUID()
        {
            //Display the device unique ID
            return LicenseHandler.GenerateUID("DigitalSignageSystem");
        }
    }
}