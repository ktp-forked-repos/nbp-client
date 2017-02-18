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
            try
            {
                this.InitializeComponent();
                this.InitViewModel();
                this.ReadAppSettings();
                this.SetInitialData();
                this.GetCurrencies();
            }catch(Exception ex)
            {
                this.ViewModel.SetErrorAlert();
            }
        }

        public void InitViewModel()
        {
            this.ViewModel = new MainPageViewModel();
        }

        public void ReadAppSettings()
        {
            AppSettings = (App.Current as App).appSettings;
            AppSettings.LastOpenPage = "MainPage";
        }

        public void SetInitialData()
        {
            if(AppSettings.HasDateOnFirstPage())
            {
                this.ViewModel.SetDate(AppSettings.DateOnFirstPage);
            }else
            {
                this.ViewModel.SetDate(DateTime.Now);
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
            catch(AggregateException aggregateEx)
            {
                this.ViewModel.SetErrorAlert();
            }
            catch(Exception ex)
            {
                this.ViewModel.SetErrorAlert();
            }
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

        private void CurrencyListViewItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(DetailsPage));
        }

        private string GetApiAddress()
        {
            return Core.Constants.Constants.MainPageCurrenciesAdress + this.ViewModel.CurrentDate.ToString("yyyy-MM-dd");
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            AppSettings.LastOpenPage = "MainPage";
            base.OnNavigatedTo(e);
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }


    }
  
}
