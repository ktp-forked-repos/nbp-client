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
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NBPClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailsPage : Page
    {
        private DetailsPageViewModel ViewModel;
        CancellationTokenSource cts;
        public DetailsPage()
        {
            this.ViewModel = new DetailsPageViewModel();
            this.InitializeComponent();
            this.Loaded += DetailsPage_Loaded;
        
        }

        private async void DetailsPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadChartContents();
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

            cts = null;
            // GetCurrencySet();
        }
        private void LoadChartContents()
        {
            //Random rand = new Random();
            //List<FinancialStuff> financialStuffList = new List<FinancialStuff>();
            //financialStuffList.Add(new FinancialStuff() { EffectiveDate = "MSFT", Bid =1.5 });
            //financialStuffList.Add(new FinancialStuff() { EffectiveDate = "AAPL", Bid =3.1 });
            //financialStuffList.Add(new FinancialStuff() { EffectiveDate = "GOOG",Bid =2.5 });
            //financialStuffList.Add(new FinancialStuff() { EffectiveDate = "BBRY", Bid=4.7 });
            //(LineChart.Series[0] as LineSeries).ItemsSource = financialStuffList;
            (LineChart.Series[0] as LineSeries).ItemsSource = this.ViewModel.Currencies;

            ((LineSeries)LineChart.Series[0]).DependentRangeAxis = new LinearAxis()
            {
                Maximum = 4.5,
                Minimum = 2.5,
                
                Orientation = AxisOrientation.Y,
                Interval = 0.1,
                ShowGridLines = true,
            };
            //((LineSeries)LineChart.Series[0]).IndependentAxis = new LinearAxis()
            //{
            //    Maximum = 5,
            //    Minimum = 0,
               
            //    Orientation = AxisOrientation.X,
            //    Interval = 0.01,
            //    ShowGridLines = true,
            //};
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DetailsPage));
        }

        private void StartDateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            this.ViewModel.StartDate = sender.Date.Value.Date;
            this.GetCurrencySet();
        }

        private void EndDateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            this.ViewModel.EndDate = sender.Date.Value.Date;
        }
        public async void GetCurrencySet()
        {
            if (ViewModel.StartDate != null && ViewModel.EndDate != null)
            {
                // var res = WebServiceConsumer.GetCurrencySet("http://api.nbp.pl/api/exchangerates/rates/c/usd/2016-04-04/2016-05-04/?format=json", "usd", () => { });

                ///  string d = data.ToFileTimeUtc().ToString("yyyy-MM-dd");
                this.ViewModel.Currencies.Clear();

                var res = await WebServiceConsumer.GetCurrencySet("http://api.nbp.pl/api/exchangerates/rates/c/usd/2008-01-01/2008-01-31?format=json", () => { });
                Random rand = new Random();
                foreach (var item in res)
                {
                    
                    this.ViewModel.Currencies.Add(item);
                    await Task.Delay(TimeSpan.FromMilliseconds(300));
                }
            }
        }

        public class DetailsPageViewModel
        {
            public DetailsPageViewModel()
            {
            }
            private ObservableCollection<RateMode> currencies = new ObservableCollection<RateMode>();
            public ObservableCollection<RateMode> Currencies { get { return this.currencies; } }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        public class FinancialStuff
        {
            public double Bid { get; set; }
            public string EffectiveDate { get; set; }
        }
        async Task AccessTheWebAsync(CancellationToken ct)
        {
            HttpClient client = new HttpClient();

            // Make a list of web addresses.  
            List<string> urlList = SetUpURLList();

            // ***Create a query that, when executed, returns a collection of tasks.  
            IEnumerable<Task<List<RateMode>>> downloadTasksQuery =
                from url in urlList select ProcessURL(url, client, ct);

            // ***Use ToList to execute the query and start the tasks.   
            List<Task<List<RateMode>>> downloadTasks = downloadTasksQuery.ToList();

            // ***Add a loop to process the tasks one at a time until none remain.  
            while (downloadTasks.Count > 0)
            {
                // Identify the first task that completes.  
                Task<List<RateMode>> firstFinishedTask = await Task.WhenAny(downloadTasks);

                // ***Remove the selected task from the list so that you don't  
                // process it more than once.  
                downloadTasks.Remove(firstFinishedTask);

                // Await the completed task.  
                var res2 = await firstFinishedTask;
                foreach(var item in res2) {
                     ViewModel.Currencies.Add(item);
                    await Task.Delay(TimeSpan.FromMilliseconds(300));
                }
            }
        }
        async Task<List<RateMode>> ProcessURL(string url, HttpClient client, CancellationToken ct)
        {
            // GetAsync returns a Task<HttpResponseMessage>.   
            HttpResponseMessage response = await client.GetAsync(url, ct);


            // Retrieve the website contents from the HttpResponseMessage.  
            string a  = await response.Content.ReadAsStringAsync();

            var json = JObject.Parse(a).ToString();
            var list = JsonConvert.DeserializeObject<TableModel2>(json);

            return list.Rates;
        }
        private List<string> SetUpURLList()
        {
            List<string> urls = new List<string>
             {
                "http://api.nbp.pl/api/exchangerates/rates/c/usd/2008-01-01/2008-01-31?format=json",
                "http://api.nbp.pl/api/exchangerates/rates/c/usd/2009-01-01/2009-01-31?format=json",
                 "http://api.nbp.pl/api/exchangerates/rates/c/usd/2010-01-01/2010-12-31?format=json",
                //"http://api.nbp.pl/api/exchangerates/rates/c/usd/2011-01-01/2011-12-31?format=json",
                //"http://api.nbp.pl/api/exchangerates/rates/c/usd/2012-01-01/2012-12-31?format=json",
                //"http://api.nbp.pl/api/exchangerates/rates/c/usd/2013-01-01/2013-12-31?format=json",
                //"http://api.nbp.pl/api/exchangerates/rates/c/usd/2014-01-01/2014-12-31?format=json",
                //"http://api.nbp.pl/api/exchangerates/rates/c/usd/2015-01-01/2015-12-31?format=json",
                //"http://api.nbp.pl/api/exchangerates/rates/c/usd/2016-01-01/2016-12-31?format=json",
             };
            return urls;
        }

       
        private void cancelSet(object sender, RoutedEventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
            }
        }
    }
}
