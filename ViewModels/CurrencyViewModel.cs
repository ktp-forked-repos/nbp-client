using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBPClient.ViewModels
{
   public class CurrencyViewModel
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
    }
    public class CurrencyModel
    {
        public string Currency { get; set; }
        
        public decimal Mid { get; set; }
        public string Code { get; set; }
    }
    public class RateMode
    {
        public string No { get; set; }
        public string TableName { get; set; }
        public string Mid { get; set; }

        public double Bid { get; set; }
        public double Ask { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Code { get; set; }
    }
    public class TableModel2
    {
        public string Table { get; set; }
        public string Currency { get; set; }
        public string Code { get; set; }
        public List<RateMode> Rates { get; set; }
    }
    public class TableModel
    {
        public string Table { get; set; }
        public string No { get; set; }
        public DateTime effectiveDate { get; set; }
        public List<CurrencyModel> Rates { get; set; }
    }
}
