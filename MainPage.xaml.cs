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

namespace NBPClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private AppSettings appSettings;
        public MainPageViewModel ViewModel { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            this.InitViewModel();
            this.ReadAppSettings();
            this.SetInitialData();
            this.GetCurrencies();
        }

        public void InitViewModel()
        {
            this.ViewModel = new MainPageViewModel();
        }

        public void ReadAppSettings()
        {
            appSettings = (App.Current as App).appSettings;
        }

        public void SetInitialData()
        {
            if(appSettings != null && appSettings.DateOnFirstPage != null)
            {
                this.ViewModel.SetDate(appSettings.DateOnFirstPage);
            }
        }
       
        private async void GetCurrencies()
        {
            this.ViewModel.Currencies.Clear();
            DateTime date = DateTime.Now;
            if (appSettings.DateOnFirstPage != null)
                date = appSettings.DateOnFirstPage;
            string s = date.ToString("yyyy-MM-dd");
            var res = await  WebServiceConsumer.GetCurrency("http://api.nbp.pl/api/exchangerates/tables/a/" + s, onDataComplete);
            if (res.Count == 0)
            {
                dateErrorTextBlock.Text = "No data for this date " + date.ToString("yyyy-MM-dd") + "choose diffrent date";
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


        private void CalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {

            if ((sender.Date.Value.Date > new DateTime(2002,1, 1) && sender.Date.Value.Date <= DateTime.Now)
                && (sender.Date.Value.DayOfWeek != DayOfWeek.Saturday && sender.Date.Value.DayOfWeek != DayOfWeek.Sunday))
            {
                dateErrorTextBlock.Text = "";
                this.GetCurrencies(sender.Date.Value.Date);
                this.appSettings.DateOnFirstPage = sender.Date.Value.Date;
            }
            else
            {
                dateErrorTextBlock.Text = "Wrong date! Choose date starting from 23 Sep 2002";

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
   
        private void AppBarToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
        private void CurrencyListViewItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(DetailsPage));
        }
    }
}
