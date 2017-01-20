using NBPClient.Parameters;
using NBPClient.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NBPClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailsPage : Page
    {
        public  DetailsPageViewModel ViewModel;
        public DetailPageParametersModel parameters;
        CancellationTokenSource cts;
        public DetailsPage()
        {
           
            this.InitializeComponent();
            this.ViewModel = new DetailsPageViewModel();
            //  this.DataContext = new DetailsPageViewModel();
            this.Loaded += DetailsPage_Loaded;


        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            parameters = (DetailPageParametersModel)e.Parameter;

        }
        private async void DetailsPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadChartContents();
         
            //cts = new CancellationTokenSource();

            //try
            //{
            //    await AccessTheWebAsync(cts.Token);
            //}
            //catch (OperationCanceledException)
            //{
            //}
            //catch (Exception)
            //{
            //}

            //cts = null;
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
          //  this.GetCurrencySet();
        }

        private  async void EndDateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            this.ViewModel.EndDate = sender.Date.Value.Date;
            cts = new CancellationTokenSource();

            try
            {
                await AccessTheWebAsync(cts.Token, det);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception)
            {
            }

            cts = null;
        }
        //public async void GetCurrencySet()
        //{
        //    if (ViewModel.StartDate != null && ViewModel.EndDate != null)
        //    {
        //        // var res = WebServiceConsumer.GetCurrencySet("http://api.nbp.pl/api/exchangerates/rates/c/usd/2016-04-04/2016-05-04/?format=json", "usd", () => { });

        //        ///  string d = data.ToFileTimeUtc().ToString("yyyy-MM-dd");
        //        this.ViewModel.Currencies.Clear();

        //        var res = await WebServiceConsumer.GetCurrencySet("http://api.nbp.pl/api/exchangerates/rates/c/usd/2008-01-01/2008-01-31?format=json", () => { });
        //        Random rand = new Random();
        //        foreach (var item in res)
        //        {
                    
        //            this.ViewModel.Currencies.Add(item);
        //           // await Task.Delay(TimeSpan.FromMilliseconds(10));
        //        }
        //    }
        //}

        public class DetailsPageViewModel: INotifyPropertyChanged
        {
            public DetailsPageViewModel()
            {
                ProgressBardProgess = 0D;
            }
            private ObservableCollection<RateMode> currencies = new ObservableCollection<RateMode>();
            public ObservableCollection<RateMode> Currencies { get { return this.currencies; } }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public double ProgressBardProgess
            {
                get { return _ProgressBardProgess; }
                set
                {
                    _ProgressBardProgess = value;
                    OnPropertyChanged();
                }
            }
            private double _ProgressBardProgess;
            public event PropertyChangedEventHandler PropertyChanged = delegate { };


            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            public  void CurrenciesSet(List<RateMode> res2)
            {
                foreach (var item in res2)
                {
                    this.Currencies.Add(item);
                }
                OnPropertyChanged();
            }
        }

        public class FinancialStuff
        {
            public double Bid { get; set; }
            public string EffectiveDate { get; set; }
        }
        async Task AccessTheWebAsync(CancellationToken ct)
        {
            HttpClient client = new HttpClient();
            List<string> urlList = SetUpURLList(GetIntervals());
             IEnumerable<Task<List<RateMode>>> downloadTasksQuery =
            from url in urlList select ProcessURL(url, client, ct);
            this.ViewModel.ProgressBardProgess = 0;
            double progressBarInterval = Math.Ceiling(100D / downloadTasksQuery.Count());
             List<Task<List<RateMode>>> downloadTasks = downloadTasksQuery.ToList();
           
            while (downloadTasks.Count > 0)
            {
                Task<List<RateMode>> firstFinishedTask = await Task.WhenAny(downloadTasks);

                downloadTasks.Remove(firstFinishedTask);
                var res2 = await firstFinishedTask;
                this.ViewModel.CurrenciesSet(res2);
                await Task.Delay(TimeSpan.FromMilliseconds(1));
                if (this.ViewModel.ProgressBardProgess + progressBarInterval <= 100)
                {
                    this.ViewModel.ProgressBardProgess += progressBarInterval;
                }
                else
                {
                    this.ViewModel.ProgressBardProgess = 100;
                }
                // Await the completed task.  
               
                //foreach(var item in res2) {
                //     ViewModel.CurrenciesAdd(item);

                // await Task.Delay(TimeSpan.FromMilliseconds(10));

                // }


            }
        }
        async Task<List<RateMode>> ProcessURL(string url, HttpClient client, CancellationToken ct)
        {
            HttpResponseMessage response = await client.GetAsync(url, ct);
            


            string a  = await response.Content.ReadAsStringAsync();

            var json = JObject.Parse(a).ToString();
            var list = JsonConvert.DeserializeObject<TableModel2>(json);

            return list.Rates;
        }
        private List<string> SetUpURLList(List<int> intervals)
        {

            int totalNumOfDays = 0;
            List<string> urls = new List<string>();
            for (int i = 0; i < intervals.Count; i++)
            {

                urls.Add("http://api.nbp.pl/api/exchangerates/rates/" +parameters.Table +"/" + parameters.CurrencyCode  + "/"
                    + this.ViewModel.StartDate.AddDays(totalNumOfDays).ToString("yyyy-MM-dd")
                    + "/" + (this.ViewModel.StartDate.AddDays(totalNumOfDays + intervals[i])).ToString("yyyy-MM-dd")
                    + "?format=json");
                totalNumOfDays += intervals[i];
            }

            return urls;
        }
        private List<int> GetIntervals()
        {
            double totalDays = (this.ViewModel.EndDate - this.ViewModel.StartDate).TotalDays;
            var lista = new List<int>();
            if (totalDays > 10)
            {
                while (totalDays > 10)
                {
                    lista.Add(10);
                    totalDays -= 10;
                }
                lista.Add(System.Convert.ToInt32(totalDays));
                return lista;

            }
            else
            {
                return new List<int> { System.Convert.ToInt32(Math.Ceiling(totalDays)) };
            }
        }

        private void cancelSet(object sender, RoutedEventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
            }
        }

        private  async Task AppBarToggleButton_Click(object sender, RoutedEventArgs e)
        {
          

        }

        private void AppBarToggleButton_Click_1(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private void AppBarToggleButton_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void AppBarToggleButton_Click_3(object sender, RoutedEventArgs e)
        {

        }
    }

  
}
