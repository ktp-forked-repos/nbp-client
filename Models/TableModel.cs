using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBPClient.Models
{
    public class TableModel
    {
        public string Table { get; set; }
        public string No { get; set; }
        public DateTime effectiveDate { get; set; }
        public List<CurrencyModel> Rates { get; set; }
    }
}
