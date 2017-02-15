﻿using System;
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
            StartDate = null;
            EndDate = null;
            ChartAxis = new LinearAxis()
            {
                Maximum = 3.5,
                Minimum = 2.5,
                Orientation = AxisOrientation.Y,
                Interval = 0.1,
                ShowGridLines = true,
            };
           
        }
        private RangeObservableCollection<RateMode> currencies = new RangeObservableCollection<RateMode>();
        public RangeObservableCollection<RateMode> Currencies { get { return this.currencies; } }
        public DateTime? StartDate
        {
            get
            {
                return startDate;
            }
            set
            {
                startDate = value;
                OnPropertyChanged();
            }
        }
        private DateTime? startDate;
        private DateTime? endDate;
        public DateTime? EndDate { get { return endDate; }  set {
                endDate = value;
                OnPropertyChanged(); } }
        public LinearAxis ChartAxis { get; set; }
        public string errorText { get; set; }
        public bool ProgressBarActive { get; set; }

       

        public void ResetProgres()
        {
            this.ProgressBardProgess = 0;
            this.ProgressBarActive = false;
            this.ProgressBarVisibility = Visibility.Visible;
        }

        

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
            get { return _ProgressBardProgess; }
            set
            {
                _ProgressBardProgess = value;
                OnPropertyChanged("ProgressBardProgess");
            }
        }
        private double _ProgressBardProgess;
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private int ProgressInterval { get; set; }
        public Visibility ProgressBarVisibility { get; private set; }
        public string CurrencyCode { get; internal set; }
        public string Table { get; internal set; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void CurrenciesSet(List<RateMode> res)
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
            this.ProgressBarActive = false;
            this.ProgressBarVisibility = Visibility.Collapsed; 
        }
    }
}