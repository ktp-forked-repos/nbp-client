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
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Composition;
using Windows.UI.ViewManagement;
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
using NBPClient.ViewModels;
using NBPClient.Core.Infrastructure;

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
        AppSettings appSetttings;
        
        public DetailsPage()
        {
           
            this.InitializeComponent();
            this.ViewModel = new DetailsPageViewModel();
            this.Loaded += DetailsPage_Loaded;
            appSetttings = (App.Current as App).appSettings;
            SetInitialData();

        }
        public void SetInitialData()
        {
            if(this.appSetttings!= null)
            {
               
            }
           
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            parameters = (DetailPageParametersModel)e.Parameter;
            base.OnNavigatedTo(e);
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
         
            parameters = (DetailPageParametersModel)e.Parameter;
            base.OnNavigatingFrom(e);

        }
        private async void DetailsPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadChartContents();
    
        }
        private void LoadChartContents()
        {
           
            (LineChart.Series[0] as LineSeries).ItemsSource = this.ViewModel.Currencies;

            ((LineSeries)LineChart.Series[0]).DependentRangeAxis = new LinearAxis()
            {
                Maximum = 4.5,
                Minimum = 2.5,
                
                Orientation = AxisOrientation.Y,
                Interval = 0.1,
                ShowGridLines = true,
            };
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DetailsPage));
        }

        private async void StartDateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            this.ViewModel.StartDate = sender.Date.Value.Date;
            cts = new CancellationTokenSource();
            if (this.ViewModel.StartDate != null && this.ViewModel.EndDate != null)
            {
              //  await AccessTheWebAsync(cts.Token);
            }
            cts = null;
        }

        private  async void EndDateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            this.ViewModel.EndDate = sender.Date.Value.Date;
            cts = new CancellationTokenSource();
            if (this.ViewModel.StartDate != null && this.ViewModel.EndDate != null)
            {
                await AccessTheWebAsync(cts.Token);
            }
            cts = null;
        }

        async Task AccessTheWebAsync(CancellationToken ct)
        {
            HttpClient client = new HttpClient();
            List<string> urlList = UrlBuilder.SetUpURLList(Convert.ToInt32((this.ViewModel.EndDate - this.ViewModel.StartDate).TotalDays),parameters.Table,  parameters.CurrencyCode, this.ViewModel.StartDate);
          
            List<Task<List<RateMode>>> downloadTasks = (from url in urlList select ProcessURL(url, client, ct)).ToList();
            this.ViewModel.ProgressBardProgess = 0;
            double progressBarInterval = Math.Ceiling(100D / downloadTasks.Count());
            while (downloadTasks.Count > 0)
            {
                Task<List<RateMode>> firstFinishedTask = await Task.WhenAny(downloadTasks);

                await firstFinishedTask.ContinueWith((t) =>
                 {
                     this.ViewModel.CurrenciesSet(t.Result);
                     if (this.ViewModel.ProgressBardProgess + progressBarInterval <= 100)
                     {
                         this.ViewModel.ProgressBardProgess += progressBarInterval;
                     }
                     else
                     {
                         this.ViewModel.ProgressBardProgess = 100;
                     }
                 }, TaskScheduler.FromCurrentSynchronizationContext());

                downloadTasks.Remove(firstFinishedTask);
              
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
      

      
        private void AppBarToggleButton_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame.CanGoBack)
                {
                    this.Frame.Navigate(typeof(MainPage));
                }
            }catch(Exception ex)
            {
                var a = ex;
            }
        }

        private void AppBarToggleButton_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private  async void AppBarToggleButton_Click_3(object sender, RoutedEventArgs e)
        {

            RenderTargetBitmap bitmap2 = new RenderTargetBitmap();
             await bitmap2.RenderAsync(LineChartStackPanel, (int)LineChartStackPanel.ActualWidth, (int)LineChartStackPanel.ActualHeight);
            var res = await bitmap2.GetPixelsAsync();
           
            try
            {
                if (EnsureUnsnapped())
            {
               
                FileSavePicker savePicker = new FileSavePicker();
                    savePicker.DefaultFileExtension = ".png";
                    savePicker.FileTypeChoices.Add(".png", new List<string> { ".png" });
                    savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                    savePicker.SuggestedFileName = "snapshot.png";

                    StorageFile file = await savePicker.PickSaveFileAsync();
                    if (file == null)
                    {
                        return;
                      
                    }
                    using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);

                        encoder.SetPixelData(
                            BitmapPixelFormat.Rgba8,
                            BitmapAlphaMode.Premultiplied,
                            (uint)bitmap2.PixelWidth,
                            (uint)bitmap2.PixelHeight,
                            DisplayInformation.GetForCurrentView().LogicalDpi,
                            DisplayInformation.GetForCurrentView().LogicalDpi,
                            res.ToArray());

                        await encoder.FlushAsync();
                        using (var outputStream = fileStream.GetOutputStreamAt(0))
                        {
                            await outputStream.FlushAsync();
                            
                        }
                    }

                }
                else
                {
                   
                }
               
            }
            catch (Exception ex)
            {

                var a = ex;
                var b = 10;
            }


        }

        public bool EnsureUnsnapped()
        {
            // FilePicker APIs will not work if the application is in a snapped state.
            // If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
            bool unsnapped = ((ApplicationView.Value != ApplicationViewState.Snapped) || ApplicationView.TryUnsnap());
            if (!unsnapped)
            {
             //   NotifyUser("Cannot unsnap the sample.", NotifyType.StatusMessage);
            }

            return unsnapped;
        }


        private void AppBarToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }
    }

  
}
