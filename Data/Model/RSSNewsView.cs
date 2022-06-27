using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Model
{
    public class RSSNewsView
    {
        public long Id { get; set; }
        public long RSSStyleId { get; set; }
        public string Title { get; set; }
        public string NewsContent { get; set; }
        public string Date { get; set; }

        public int RowNumber { get; set; }
    }
}
