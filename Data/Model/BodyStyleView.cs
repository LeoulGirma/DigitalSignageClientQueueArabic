using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Model
{
    public class BodyStyleView
    {
        public long Id { get; set; }
        public string FeatureText { get; set; }
        public string RSSText { get; set; }
        public string RSSFont { get; set; }
        public string RSSBackground { get; set; }
        public string CurrencyHeaderText { get; set; }
        public string CurrencyHeaderFont { get; set; }
        public string CurrencyBodyText { get; set; }
        public string CurrencyBodyFont { get; set; }
        public string WeatherLogoPath { get; set; }

        public long TemplateId { get; set; }
    }
}
