using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Model
{
    public class NavStyleView
    {
        public long Id { get; set; }
        public string Background { get; set; }
        public string TopText { get; set; }
        public string BottomText { get; set; }
        public string HeaderLogoText { get; set; }
        public string HeaderLogoIcon { get; set; }
        public string FooterLogo { get; set; }
        public string DateFont { get; set; }
        public string BranchFont { get; set; }
        public DateTime LastUpdate { get; set; }

        public bool LogoLeft { get; set; }
        public string LeftLogoWidth { get; set; }
        public string RightLogoWidth { get; set; }

        public long TemplateId { get; set; }

        public string HeaderPadding { get; set; }
        public string HeaderBackgroundImage { get; set; }
    }
}
