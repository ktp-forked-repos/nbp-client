using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBPClient.Core.Constants
{
    public static class Constants
    {
        public static string MainPageTable = "a";
        public static string MainPageCurrenciesAdress = "http://api.nbp.pl/api/exchangerates/tables/";
        public static int NumOfDates = 365;
        public static string MainPageName = "MainPage";
        public static string SecondPageName = "DetailsPage";

        public static string MainPageMoneyAdress = "http://api.nbp.pl/api/cenyzlota/last/30/?format=json";
    }
}
