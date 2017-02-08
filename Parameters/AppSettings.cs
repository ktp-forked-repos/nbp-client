using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBPClient.Parameters
{
   public class AppSettings
    {
        public DateTime DateOnFirstPage { get; set; }
        public DateTime StartDateOnSecondPage { get; set; }
        public DateTime EndDateOnSecondPage { get; set; }
        public string CurrencyCode { get; set; }
        public string LastOpenPage { get; set; }

        public  bool HasDateOnFirstPage()
        {
            return this.DateOnFirstPage != null;
        }

        public  void SetDateOnFirstPage(DateTime date)
        {
            this.DateOnFirstPage = date;
        }
    }
}
