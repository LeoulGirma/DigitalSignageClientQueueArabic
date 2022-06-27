using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DigitalSignageClient.Models;

namespace DigitalSignageClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ARM()
        {
            return View();
        }

        public IActionResult Currency()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public LicenseInfo GetInfo(ServerStatus licenseStatus)
        {
            LicenseInfo licenseInfo = new LicenseInfo();
            licenseInfo.Fetch = 2;
            licenseInfo.License = 1;
            licenseInfo.Playlist = 3;
            licenseInfo.Push = 4;
            return licenseInfo;
        }
    }
}
