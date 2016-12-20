using NBPClient.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NBPClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.GetCurrencies();
        }

        private void GetCurrencies()
        {
            WebServiceConsumer.GetCurrency("http://api.nbp.pl/api/exchangerates/tables/a", onDataComplete);

        }
        public void onDataComplete()
        {
            currencieProgresRing.IsActive = false;
        }
    }
    public static class WebServiceConsumer{
         static HttpClient client = new HttpClient();
       public static async Task<CurrencyViewModel> GetCurrency(string path, Action onComplete)
        {
            CurrencyViewModel model = null;
            string prod = "";
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                prod = await response.Content.ReadAsStringAsync();
                onComplete();
            }
            var a = prod;
            return null;
        }
    }
}
