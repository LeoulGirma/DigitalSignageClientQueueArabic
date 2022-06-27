using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Models
{
    public class ServerStatus
    {
        public List<LicensedPc> LicensedPc { get; set; }
        public int TotalDSLicense { get; set; }
        public int TotalQMLicense { get; set; }

        public int DSLicenseLeft { get; set; }
        public int QMLicenseLeft { get; set; }

        public string ServerUID { get; set; }

        public DateTime ExpireDate { get; set; }

    }

    public class LicenseInfo
    {
        public int Fetch { get; set; }
        public int Playlist { get; set; }
        public int Push { get; set; }
        public int License { get; set; }
    }


    public class LicensedPc
    {
        public string DeviceId { get; set; }
        public string LicenseType { get; set; }
    }

    public class ActivatedScreen
    {
        public string Id { get; set; }
        public string UID { get; set; }
    }

    public class Screen
    {
        public Screen() { ActivatedScreen = new List<ActivatedScreen>(); }
        public List<ActivatedScreen> ActivatedScreen { get; set; }
    }

    public class ActivateLicense
    {
        public ActivateLicense() { }
        public string License { get; set; }
    }

    public class ScreenLicense
    {
        public string DeviceId { get; set; }
        public string ExpireDate { get; set; }
    }
}
