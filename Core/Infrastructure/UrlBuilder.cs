using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBPClient.Core.Infrastructure
{
   public static class UrlBuilder
    {
        public static List<string> SetUpURLList(int totalDays,string tableName,string currencyCode, DateTime startDate)
        {

            List<int> intervals = UrlBuilder.GetIntervals(totalDays);
            int totalNumOfDays = 0;
            List<string> urls = new List<string>();
            for (int i = 0; i < intervals.Count; i++)
            {

                urls.Add("http://api.nbp.pl/api/exchangerates/rates/" + tableName + "/" + currencyCode + "/"
                    +startDate.AddDays(totalNumOfDays).ToString("yyyy-MM-dd")
                    + "/" + (startDate.AddDays(totalNumOfDays + intervals[i])).ToString("yyyy-MM-dd")
                    + "?format=json");
                totalNumOfDays += intervals[i];
            }

            return urls;
        }
        private static List<int> GetIntervals(int totalDays)
        {
            var lista = new List<int>();
            if (totalDays > Constants.Constants.NumOfDates)
            {
                while (totalDays > Constants.Constants.NumOfDates)
                {
                    lista.Add(Constants.Constants.NumOfDates);
                    totalDays -= Constants.Constants.NumOfDates;
                }
                lista.Add(System.Convert.ToInt32(totalDays));
                return lista;

            }
            else
            {
                return new List<int> { totalDays };
            }
        }
    }
}
