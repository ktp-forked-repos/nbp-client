using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;
using Windows.UI.Xaml;
using NBPClient.Models;

namespace NBPClient.ViewModels
{
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        private bool _suppressNotification = false;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
                base.OnCollectionChanged(e);
        }

        public void AddRange(IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            _suppressNotification = true;

            foreach (T item in list)
            {
                Add(item);
            }
            _suppressNotification = false;
             OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

    public class DetailsPageViewModel : INotifyPropertyChanged
    {
        public DetailsPageViewModel()
        {
            ProgressBardProgess = 0D;
            StartDate = DateTime.Now;
            ProgressBarVisibility = Visibility.Collapsed;
            IsProgressBarActive = false;
            EndDate = DateTime.Now;
            ChartAxis = new LinearAxis()
            {
                Maximum = 3.5,
                Minimum = 2.5,
                Orientation = AxisOrientation.Y,
                Interval = 0.1,
                ShowGridLines = true,
            };
           
        }
        private RangeObservableCollection<RateModel> currencies = new RangeObservableCollection<RateModel>();
        private DateTime startDate;
        private DateTime endDate;
        public string errorText { get; set; }
        private double progressBardProgess;
        private bool isProgressBarActive;
        public RangeObservableCollection<RateModel> Currencies { get { return this.currencies; } }
        public DateTime StartDate
        {
            get
            {
                return startDate;
            }
            set
            {
                startDate = value;
                StartDateOff = new DateTime(value.Year, value.Month, value.Day);
                OnPropertyChanged();
            }
        }
        public DateTime EndDate { get { return endDate; }  set {
                endDate = value;
                EndDateOff = new DateTime(value.Year, value.Month, value.Day);
                OnPropertyChanged(); } }
        public LinearAxis ChartAxis { get; set; }
      
        public string ErrorText
        {
            get
            {
                return errorText;
            }set
            {
                errorText = value;
                OnPropertyChanged();
            }
        }    
        public double ProgressBardProgess
        {
            get { return progressBardProgess; }
            set
            {
                progressBardProgess = value;
                OnPropertyChanged("ProgressBardProgess");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private int ProgressInterval { get; set; }
        public string CurrencyCode { get; internal set; }
        public string Table { get; internal set; }
        public System.Nullable<DateTimeOffset>  StartDateOff { get;  set; }
        public System.Nullable<DateTimeOffset>  EndDateOff { get;  set; }
       
        private Visibility progressBarVisibility;
        public bool IsProgressBarActive {
            get
            {
                return isProgressBarActive;
            }
            set
            {
                isProgressBarActive = value;
                OnPropertyChanged();
            }
        }
        public Visibility ProgressBarVisibility
        {
            get
            {
                return progressBarVisibility;
            }
            set
            {
                progressBarVisibility = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void CurrenciesSet(List<RateModel> res)
        {

            foreach (var item in res)
            {
                if (Convert.ToDouble(item.Mid) - 0.3 < this.ChartAxis.Minimum)
                {
                    this.ChartAxis.Minimum = Convert.ToDouble(item.Mid) - 0.3;
                    this.ChartAxis.Maximum = Convert.ToDouble(item.Mid) + 1.3;
                }

                else if (Convert.ToDouble(item.Mid) + 0.3 > this.ChartAxis.Maximum)
                {
                    this.ChartAxis.Maximum = Convert.ToDouble(item.Mid) + 0.3;
                    this.ChartAxis.Minimum = Convert.ToDouble(item.Mid) - 1.3;
                }
               
            }
            this.Currencies.AddRange(res);
    
            //OnPropertyChanged("currencies");
        }

        public  void ResetErrorText()
        {
            this.ErrorText = "";
        }

        public void SetProgressInterval(int interval)
        {
            ProgressInterval = interval;
        }

        public void UpdateProgress()
        {
            if (this.ProgressBardProgess + this.ProgressInterval <= 100)
            {
                this.ProgressBardProgess += this.ProgressInterval;
            }
            else
            {
                this.ProgressBardProgess = 100;
            }
        }

        public void HideProgress()
        {
            this.ProgressBarVisibility = Visibility.Collapsed;
            this.IsProgressBarActive = false;
          
        }

        public void SetErrorText(string error)
        {
            this.errorText = error;
        }

        public void ShowProgressBars()
        {
            this.IsProgressBarActive = true;
            this.ProgressBardProgess = 0;
            this.ProgressBarVisibility = Visibility.Visible;
        }
    }
}
