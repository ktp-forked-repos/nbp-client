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
        public string Table { get;  set; }

        public  bool HasDateOnFirstPage()
        {
            return this.DateOnFirstPage != null;
        }

        public  void SetDateOnFirstPage(DateTime date)
        {
            this.DateOnFirstPage = date;
        }
        public bool HasStartDateOnSecondPage()
        {
            return this.StartDateOnSecondPage != null;
        }

        public void SetStartDateOnSecondPage(DateTime date)
        {
            this.StartDateOnSecondPage = date;
        }
        public bool HasEndDateOnSecondPage()
        {
            return this.EndDateOnSecondPage != null;
        }

        public void SetEndDateOnSecondPage(DateTime date)
        {
            this.EndDateOnSecondPage = date;
        }
        public bool HasCurrencCode(string code)
        {
            return !String.IsNullOrEmpty(this.CurrencyCode);
        }
        public void SetCode(string code)
        {
            this.CurrencyCode = code;
        }

        internal bool HasTable()
        {
            return !String.IsNullOrEmpty(this.Table);
        }

        internal bool HasCurrencCode()
        {
            return !String.IsNullOrEmpty(this.CurrencyCode);
        }
    }
}
