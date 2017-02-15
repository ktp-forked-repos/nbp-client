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
using NBPClient.Core.Infrastructure;
using NBPClient.Infrastructure;
using NBPClient.Models;

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
        AppSettings AppSetttings { get; set; }
        
        public DetailsPage()
        {
                this.InitializeComponent();
                this.ViewModel = new DetailsPageViewModel();
                this.Loaded += DetailsPage_Loaded;
                ReadAppSettings();
       
        }
        public void ReadAppSettings()
        {
            AppSetttings = (App.Current as App).appSettings;
        }
        public async Task SetInitialDate()
        {
            if (AppSetttings.HasStartDateOnSecondPage())
            {
                this.ViewModel.StartDate = this.AppSetttings.StartDateOnSecondPage;
            }
            if (AppSetttings.HasEndDateOnSecondPage())
            {
                this.ViewModel.EndDate = this.AppSetttings.EndDateOnSecondPage;
            }
            if (AppSetttings.HasCurrencCode())
            {
                this.ViewModel.CurrencyCode = AppSetttings.CurrencyCode;
            }
            if (AppSetttings.HasTable())
            {
                this.ViewModel.Table = AppSetttings.Table;
            }
            await Do();

        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter == "")
            {
              await  SetInitialDate();
            }
            else
            {
                parameters = (DetailPageParametersModel)e.Parameter;

                AppSetttings.CurrencyCode = parameters.CurrencyCode;
                AppSetttings.Table = parameters.Table;
                AppSetttings.LastOpenPage = "DetailsPage";

            }
            base.OnNavigatedTo(e);
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (e.Parameter != null)
            {
                parameters = (DetailPageParametersModel)e.Parameter;
                AppSetttings.CurrencyCode = parameters.CurrencyCode;
                AppSetttings.Table = parameters.Table;
            }
            base.OnNavigatingFrom(e);
        }

        private async void DetailsPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadChartContents();
            if (parameters == null)
            {
                await SetInitialDate();
            }
            else
            {
                this.ViewModel.Table = parameters.Table;
                this.ViewModel.CurrencyCode = parameters.CurrencyCode;
            }

        }
        private void LoadChartContents()
        {
            ((LineSeries)LineChart.Series[0]).DependentRangeAxis = this.ViewModel.ChartAxis;
        }

        private async void StartDateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            this.ViewModel.StartDate = sender.Date.Value.Date;
            this.ViewModel.ResetErrorText();
            AppSetttings.SetStartDateOnSecondPage(sender.Date.Value.Date);
            await Do();
        }

        public async Task Do()
        {
            cts = new CancellationTokenSource();
            if (DateValidator.CheckDateRange(ViewModel.StartDate, ViewModel.EndDate, () => this.ViewModel.ErrorText = "Wrong date"))
            {
                this.ViewModel.Currencies.Clear();
                await AccessTheWebAsync(cts.Token).ConfigureAwait(false);
            }
            cts = null;
        }
        private  async void EndDateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            this.ViewModel.EndDate = sender.Date.Value.Date;
            this.ViewModel.ResetErrorText();
            AppSetttings.SetEndDateOnSecondPage(sender.Date.Value.Date);
            await Do();
        }

        async Task AccessTheWebAsync(CancellationToken ct)
        {
            try
            {
                HttpClient client = new HttpClient();
                List<string> urlList = UrlBuilder.SetUpURLList(Convert.ToInt32((this.ViewModel.EndDate - this.ViewModel.StartDate).TotalDays), ViewModel.Table, ViewModel.CurrencyCode, ViewModel.StartDate);
                List<Task<List<RateModel>>> downloadTasks = (from url in urlList select WebServiceConsumer.ProcessURL(url, client, ct, () => this.ViewModel.SetErrorText("Error"))).ToList();
                this.ViewModel.ShowProgressBars();
                this.ViewModel.SetProgressInterval(Convert.ToInt32(Math.Ceiling(100D / downloadTasks.Count())));
                /*  Task<List<RateModel>> task1 = downloadTasks.First();
                  await task1.ContinueWith((t) =>
                  {
                      this.ViewModel.CurrenciesSet(t.Result);
                      this.ViewModel.UpdateProgress();

                  }, TaskScheduler.FromCurrentSynchronizationContext());*/
                while (downloadTasks.Count > 0)
                {
                    Task<List<RateModel>> firstFinishedTask = await Task.WhenAny(downloadTasks);
                    downloadTasks.Remove(firstFinishedTask);
                    await firstFinishedTask.ContinueWith((t) =>
                    {
                    
                        this.ViewModel.CurrenciesSet(Randomizer.RandomArrayEntries(t.Result, 10));
                        this.ViewModel.UpdateProgress();

                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                this.ViewModel.HideProgress();
            }
            catch(AggregateException aggex)
            {
                this.ViewModel.HideProgress();
                this.ViewModel.SetErrorText("Connection can not be established");
            }
            catch(Exception ex)
            {
            }
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame.CanGoBack)
                {
                    this.Frame.Navigate(typeof(MainPage));
                }else
                {
                    this.Frame.Navigate(typeof(MainPage));
                }
            }catch(Exception ex)
            {
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private  async void SaveChartButtonClick(object sender, RoutedEventArgs e)
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
            bool unsnapped = ((ApplicationView.Value != ApplicationViewState.Snapped) || ApplicationView.TryUnsnap());
            if (!unsnapped)
            {
            }
            return unsnapped;
        }
    }
}
