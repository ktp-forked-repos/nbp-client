using NBPClient.Parameters;
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
using System.Threading;
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
           /// data = new DateTime(2016, 1, 1);
            string s = data.ToString("yyyy-MM-dd");
            var res = await  WebServiceConsumer.GetCurrency("http://api.nbp.pl/api/exchangerates/tables/a/" + s, onDataComplete);
            if (res.Count == 0)
            {
                dateErrorTextBlock.Text = "No data for this date " + data.ToString("yyyy-MM-dd") + "choose diffrent date";
            }
            else
            {
                foreach (var item in res)
                {
                    this.ViewModel.Currencies.Add(item);
                }
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

            if (sender.Date.Value.Date > new DateTime(2002,1, 1) && sender.Date.Value.Date <= DateTime.Now)
            {
                dateErrorTextBlock.Text = "";
                this.GetCurrencies(sender.Date.Value.Date);
            }
            else
            {
                dateErrorTextBlock.Text = "Wrong date! Choose date starting from 23 Sep 2002";

            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void CurrencySelectedItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(DetailsPage));
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ListViewItem item = (ListViewItem)sender;
            this.Frame.Navigate(typeof(DetailsPage), new DetailPageParametersModel()
            {
                Table ="a",
                CurrencyCode = "USD"
            });
        }   

        private void AppBarToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
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
    public  class WebServiceConsumer{
        static HttpClient client = new HttpClient();
        CancellationTokenSource cts;
        public static async Task<List<CurrencyModel>> GetCurrency(string path, Action onComplete)
        {
            string prod = "";
            HttpResponseMessage response = await client.GetAsync(path);

            if(response.StatusCode == System.Net.HttpStatusCode.NotFound) { 
                return new List<CurrencyModel>();
            }
            else
            {

                prod = await response.Content.ReadAsStringAsync();
                onComplete();
                var a = prod;

                var json = JArray.Parse(a)[0].ToString();
                var list = JsonConvert.DeserializeObject<TableModel>(json);

                return list.Rates;
            }
           
           
        }
        public static async Task<List<RateMode>> GetCurrencySet(string path, Action onComplete)
        {
            string prod = "";
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                prod = await response.Content.ReadAsStringAsync();
                onComplete();
            }
            var a = prod;

            var json = JObject.Parse(a).ToString();
            var list = JsonConvert.DeserializeObject<TableModel2>(json);

            return list.Rates;
        }
        public static async Task<List<RateMode>> GetCurrencySet2(string path, Action onComplete)
        {
            string prod = "";
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                prod = await response.Content.ReadAsStringAsync();
                onComplete();
            }
            var a = prod;

            var json = JObject.Parse(a).ToString();
            var list = JsonConvert.DeserializeObject<TableModel2>(json);

            return list.Rates;
        }
        /* public  async void GetCurrencySets(string path, string currency, Action onComplete)
         {
             cts = new CancellationTokenSource();
             try
             {
                 await AccessTheWebAsync(cts.Token);
             }
             catch (OperationCanceledException)
             {
             }
             catch (Exception)
             {
             }
         }


         async IEnumerable<Task<List<CurrencyModel>>> AccessTheWebAsync(CancellationToken ct)
         {
             HttpClient client = new HttpClient();

             // Make a list of web addresses.  
             List<string> urlList = SetUpURLList();

             // ***Create a query that, when executed, returns a collection of tasks.  
             IEnumerable<Task<List<CurrencyModel>>> downloadTasksQuery =
                 from url in urlList select ProcessURL(url, client, ct);

             // ***Use ToList to execute the query and start the tasks.   
             List<Task<List<CurrencyModel>>> downloadTasks = downloadTasksQuery.ToList();

             // ***Add a loop to process the tasks one at a time until none remain.  
             while (downloadTasks.Count > 0)
             {
                 // Identify the first task that completes.  
                 Task<List<CurrencyModel>> firstFinishedTask = await Task.WhenAny(downloadTasks);

                 // ***Remove the selected task from the list so that you don't  
                 // process it more than once.  
                 downloadTasks.Remove(firstFinishedTask);

                 // Await the completed task.  
                 yield return firstFinishedTask;
             }
         }

         private List<string> SetUpURLList()
         {
             List<string> urls = new List<string>
             {
                "http://api.nbp.pl/api/exchangerates/rates/c/usd/2008-01-01/2008-12-31?format=json",
                "http://api.nbp.pl/api/exchangerates/rates/c/usd/2009-01-01/2009-12-31?format=json",
                "http://api.nbp.pl/api/exchangerates/rates/c/usd/2010-01-01/2010-12-31?format=json",
                "http://api.nbp.pl/api/exchangerates/rates/c/usd/2011-01-01/2011-12-31?format=json",
                "http://api.nbp.pl/api/exchangerates/rates/c/usd/2012-01-01/2012-12-31?format=json",
                "http://api.nbp.pl/api/exchangerates/rates/c/usd/2013-01-01/2013-12-31?format=json",
                "http://api.nbp.pl/api/exchangerates/rates/c/usd/2014-01-01/2014-12-31?format=json",
                "http://api.nbp.pl/api/exchangerates/rates/c/usd/2015-01-01/2015-12-31?format=json",
                "http://api.nbp.pl/api/exchangerates/rates/c/usd/2016-01-01/2016-12-31?format=json",
             };
             return urls;
         }

         async Task<List<CurrencyModel>> ProcessURL(string url, HttpClient client, CancellationToken ct)
         {
             // GetAsync returns a Task<HttpResponseMessage>.   
             HttpResponseMessage response = await client.GetAsync(url, ct);

             // Retrieve the website contents from the HttpResponseMessage.  
             string a = await response.Content.ReadAsStringAsync();
             var json = JArray.Parse(a)[0].ToString();
             var list = JsonConvert.DeserializeObject<TableModel>(json);

             return list.Rates;

         }
         private void cancelSet(object sender, RoutedEventArgs e)
         {
             if (cts != null)
             {
                 cts.Cancel();
             }
         }
         */

    }
}
