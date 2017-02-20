using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBPClient.Models
{
    public class TableModel
    {
        public TableModel()
        {
            Rates = new List<CurrencyModel>();
        }
        public string Table { get; set; }
        public string No { get; set; }
        public DateTime effectiveDate { get; set; }
        public List<CurrencyModel> Rates { get; set; }
    }
    public class TableCModel
    {
        public TableCModel()
        {
            Rates = new List<CCurrencyModel>();
        }
        public string Table { get; set; }
        public string No { get; set; }
        public DateTime effectiveDate { get; set; }
        public List<CCurrencyModel> Rates { get; set; }
    }
}
