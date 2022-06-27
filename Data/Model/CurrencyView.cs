using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Model
{
    public class CurrencyView
    {
        public long Id { get; set; }
        public string Currency { get; set; }
        public string Flag { get; set; }
        public decimal Selling { get; set; }
        public decimal Buying { get; set; }
        public bool Static { get; set; }
        public string Abbreviation { get; set; }
        public decimal Difference { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
