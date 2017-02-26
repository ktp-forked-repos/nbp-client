using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBPClient.Models
{
   public interface ICurrencyModel
    {
        
    }
    public class CurrencyModel : ICurrencyModel
    {
        public CurrencyModel()
        {
            IsActive = false;
        }
        public string Currency { get; set; }
        
        public decimal Mid { get; set; }
        public decimal Ask { get; set; }
        public decimal Bid { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
    }
    public class ABCurrencyModel :CurrencyModel, ICurrencyModel
    {

    }
    public class CCurrencyModel : CurrencyModel, ICurrencyModel
    {
        public decimal Ask { get; set; }
        public decimal Bid { get; set; }

       
    }



}
