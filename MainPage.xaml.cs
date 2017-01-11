using NBPClient.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            this.ViewModel = new Page1ViewModel();
            this.GetCurrencies(DateTime.Now);

        }
        public Page1ViewModel ViewModel { get; set; }
        private async void GetCurrencies(DateTime data)
        {
            ///  string d = data.ToFileTimeUtc().ToString("yyyy-MM-dd");
            this.ViewModel.Currencies.Clear();
           
            string s = data.ToString("yyyy-MM-dd");
            var res = await  WebServiceConsumer.GetCurrency("http://api.nbp.pl/api/exchangerates/tables/a/" + s, onDataComplete);
            foreach(var item in res)
            {
                this.ViewModel.Currencies.Add(item);
            }

        }
        public void onDataComplete()
        {
            //currencieProgresRing.IsActive = false;
        }

        private void CurrencyListViewItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(DetailsPage));
        }

        private void CalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            this.GetCurrencies(sender.Date.Value.Date);
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void ListView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            var a = 10;
        }
    }
    public class Page1ViewModel
    {
        private ObservableCollection<CurrencyModel> currencies = new ObservableCollection<CurrencyModel>();
        public ObservableCollection<CurrencyModel> Currencies { get { return this.currencies; } }
        public Page1ViewModel()
        {
            
        }
    }
    public static class WebServiceConsumer{
         static HttpClient client = new HttpClient();
       public static async Task<List<CurrencyModel>> GetCurrency(string path, Action onComplete)
        {
            string prod = "";
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                prod = await response.Content.ReadAsStringAsync();
                onComplete();
            }
            var a = prod;
           
            var json = JArray.Parse(a)[0].ToString();
            var list = JsonConvert.DeserializeObject<TableModel>(json);
           
            return list.Rates;
        }
    }
}
