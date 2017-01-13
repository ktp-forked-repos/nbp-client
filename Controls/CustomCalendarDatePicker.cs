using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NBPClient
{
    public class CustomCalendarDatePicker : CalendarDatePicker
    {
        public DateTimeOffset Max
        {
            get { return (DateTimeOffset)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, DateTimeOffset.Now); }
        }

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register(
                nameof(Max),                     // The name of the DependencyProperty
                typeof(DateTimeOffset),                   // The type of the DependencyProperty
                typeof(CustomCalendarDatePicker), // The type of the owner of the DependencyProperty
                new PropertyMetadata(
                      null, onMaxChanged                    // The default value of the DependencyProperty
                ));

        private static void onMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var calendar = d as CustomCalendarDatePicker;
            calendar.MaxDate = (DateTimeOffset)e.NewValue;
        }

        public DateTimeOffset Min
        {
            get { return (DateTimeOffset)GetValue(MinProperty); }
            set {

                //  DateTime utcTime1 = new DateTime(2008, 6, 19, 0, 0, 0);
                // utcTime1 = DateTime.SpecifyKind(utcTime1, DateTimeKind.Utc);
                //DateTimeOffset utcTime2 = utcTime1;
                var a = value.ToString();
                SetValue(MinProperty, DateTimeOffset.Parse(value.ToString())); }
        }

        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register(
                nameof(Min),                     // The name of the DependencyProperty
                typeof(DateTimeOffset),                   // The type of the DependencyProperty
                typeof(CustomCalendarDatePicker), // The type of the owner of the DependencyProperty
                new PropertyMetadata(
                    null, onMinChanged                      // The default value of the DependencyProperty
                ));

        private static void onMinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var calendar = d as CustomCalendarDatePicker;
            calendar.MinDate = (DateTimeOffset)e.NewValue;
        }
    }
}
