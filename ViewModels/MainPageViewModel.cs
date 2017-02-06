using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBPClient.ViewModels
{
    public class MainPageViewModel
    {
        private ObservableCollection<CurrencyModel> currencies = new ObservableCollection<CurrencyModel>();
        public DateTime CurrenciesDate { get; set; }
        public string WrongDateAlert { get; set; }
        public DateTime CurrentDate { get; set; }
        public System.Nullable<DateTimeOffset> Date { get; set; }
        public ObservableCollection<CurrencyModel> Currencies { get { return this.currencies; } }
        public MainPageViewModel()
        {
            WrongDateAlert = "";
        }

        public void SetDate(DateTime _date)
        {
            CurrentDate = _date;
            Date = new DateTime(_date.Year, _date.Month, _date.Day);
        }

        public void SetWrongDateAlert()
        {
            this.WrongDateAlert = "No data for this date " + this.CurrentDate.ToString("yyyy-MM-dd") + "choose diffrent date";
        }
        public void ResetWrongDataAlert()
        {
            this.WrongDateAlert = "";
        }
    }
}
