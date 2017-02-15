using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBPClient.Models
{
    public class RateModel
    {
        public string No { get; set; }
        public string TableName { get; set; }
        public string Mid { get; set; }

        public double Bid { get; set; }
        public double Ask { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Code { get; set; }
    }
}
