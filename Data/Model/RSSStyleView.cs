using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Model
{
    public class RSSStyleView
    {
        public long Id { get; set; }
        public string LogoPath { get; set; }
        public string TextColor { get; set; }
        public string BackgroundColor { get; set; }
        public string Category { get; set; }
        public long StyleId { get; set; }

        public int RowNumber { get; set; }
    }
}
