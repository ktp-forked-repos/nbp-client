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
    public class DetailsPageViewModel : INotifyPropertyChanged
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

        public void CurrenciesSet(List<RateMode> res2)
        {
            foreach (var item in res2)
            {
                this.Currencies.Add(item);
            }
            OnPropertyChanged();
        }
    }
}
