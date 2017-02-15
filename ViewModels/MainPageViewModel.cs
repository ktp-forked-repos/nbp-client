using NBPClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NBPClient.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<CurrencyModel> currencies = new ObservableCollection<CurrencyModel>();
        private string wrongDateAlert;
        public DateTime CurrenciesDate { get; set; }
        public string WrongDateAlert {
            get {
                return wrongDateAlert;
                }
            set {
                wrongDateAlert = value;
                OnPropertyChanged();
            }
        }
        public DateTime CurrentDate { get; set; }
        public System.Nullable<DateTimeOffset> Date { get; set; }
        public ObservableCollection<CurrencyModel> Currencies { get { return this.currencies; } }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public MainPageViewModel()
        {
            WrongDateAlert = "";
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void SetDate(DateTime _date)
        {
            CurrentDate = _date;
            Date = new DateTime(_date.Year, _date.Month, _date.Day);
        }

        public void SetWrongDateAlert()
        {
            this.WrongDateAlert = "No data for this date " + this.CurrentDate.ToString("yyyy-MM-dd") + " choose diffrent date";
        }
        public void SetErrorAlert()
        {
            this.WrongDateAlert = "Connection can not be established";
        }
        public void ResetWrongDataAlert()
        {
            this.WrongDateAlert = "";
        }
    }
}
