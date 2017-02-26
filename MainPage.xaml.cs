using NBPClient.Infrastructure;
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
using NBPClient.Core.Infrastructure;
using NBPClient.Models;
using NBPClient.Core.Constants;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

namespace NBPClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private AppSettings AppSettings { get; set; }
        public MainPageViewModel ViewModel { get; set; }

        public MainPage()
        {
                this.InitializeComponent();
                this.InitViewModel();
                this.ReadAppSettings();
                this.SetInitialData();
                this.GetCurrencies();
                this.GetMoney();

        }

       
        public void InitViewModel()
        {
            this.ViewModel = new MainPageViewModel();
            ((LineSeries)MoneyChart.Series[0]).DependentRangeAxis = this.ViewModel.MoneyChartYAxis;
           // ((LineSeries)MoneyChart.Series[0]).IndependentAxis = this.ViewModel.MoneyChartXAxis;
        }

        public void ReadAppSettings()
        {
            AppSettings = (App.Current as App).appSettings;
            AppSettings.LastOpenPage = "MainPage";
        }

        public void SetInitialData()
        {
            var dateToSet = AppSettings.HasDateOnFirstPage() ? AppSettings.DateOnFirstPage : DateTime.Now;
            this.ViewModel.SetDate(dateToSet);
        }
        private async void GetMoney()
        {
            try
            {
                //MoneyProgressRing.IsActive = true;
                this.ViewModel.Money.Clear();
                var res = WebServiceConsumer.GetMoney(GetMoneyAddress(), () => { });

                await res.ContinueWith((t) =>
                {
                     this.HandleMoneyResults(t.Result);
                    // this.OnDataComplete();
                }, TaskScheduler.FromCurrentSynchronizationContext());

            }
            catch (AggregateException aex)
            {
                this.ViewModel.SetErrorAlert();
            }
        }

        private async void GetCurrencies()
        {
            try
            {
                CurrencieProgresRing.IsActive = true;
                this.ViewModel.Currencies.Clear();
                var res = WebServiceConsumer.GetCurrency(GetApiAddress(), () => { });
                await res.ContinueWith((t) =>
                {
                    this.HandleResults(res.Result);
                    this.OnDataComplete();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch(AggregateException aex)
            {

                this.ViewModel.SetErrorAlert();
            }
            catch(Exception ex)
            {
                this.ViewModel.SetErrorAlert();
            }
        }
        private void HandleMoneyResults(List<MoneyModel> results)
        {
            results.ForEach(x => this.ViewModel.Money.Add(x));
          
        }
        private void HandleResults(List<CurrencyModel> result)
        {
            result
             .WhenSome(() => result.ForEach(item => this.ViewModel.Currencies.Add(item)))
             .WhenNone(() => this.ViewModel.SetWrongDateAlert());
        }
        
        public void OnDataComplete()
        {
            this.CurrencieProgresRing.IsActive = false;
        }

        private void CalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {

            if (DateValidator.CheckDate(sender.Date.Value.Date, () => this.ViewModel.SetWrongDateAlert()))
            {
                this.ViewModel.ResetWrongDataAlert();
                this.ViewModel.SetDate(sender.Date.Value.Date);
                this.GetCurrencies();
                this.AppSettings.SetDateOnFirstPage(sender.Date.Value.Date);
            }
            
        }


        private void CurrencySelectedItemClick(object sender, ItemClickEventArgs e)
        {
            var currencyModel = (CurrencyModel)e.ClickedItem;
            this.Frame.Navigate(typeof(DetailsPage), new DetailPageParametersModel()
            {
                Table = Core.Constants.Constants.MainPageTable,
                CurrencyCode = currencyModel.Code
            });
        }
   
        private async void AppBarToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var app = Application.Current as App;
            await app.SaveStateAsync();
            Application.Current.Exit();
        }
        private  void TableButtonClicked(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Table = (e.OriginalSource as Button).Content.ToString().ToLower();
            this.GetCurrencies();
        }

        private void CurrencyListViewItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(DetailsPage));
        }

        private string GetApiAddress()
        {
            return Core.Constants.Constants.MainPageCurrenciesAdress  + this.ViewModel.Table + "/" + this.ViewModel.CurrentDate.ToString("yyyy-MM-dd");
        }

        private string GetMoneyAddress()
        {
            return Core.Constants.Constants.MainPageMoneyAdress;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            AppSettings.LastOpenPage = Constants.MainPageName;
            base.OnNavigatedTo(e);
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

      
    }

    public class MyDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ABListViewDataTempalte { get; set; }
        public DataTemplate CListViewDataTempalte { get; set; }

        protected override DataTemplate SelectTemplateCore(object item,
                                                           DependencyObject container)
        {
            if (item is CurrencyModel)
                return ABListViewDataTempalte;
            if (item is CCurrencyModel)
                return CListViewDataTempalte;

            return base.SelectTemplateCore(item, container);
        }
    }

}
