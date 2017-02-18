using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBPClient.Core.Infrastructure
{
    public static class DateValidator
    {
        public static bool CheckDateRange(DateTime? date1, DateTime? date2, Action onNotValid)
        {
            var isRangeValid = date1 != null && date2 != null && date1 >= new DateTime(2002, 1, 1) && date1.Value <= DateTime.Now
                && date2.Value >= new DateTime(2002, 1, 1) && date2.Value <= DateTime.Now
                && date2.Value.DayOfWeek != DayOfWeek.Saturday
                 && date2.Value.DayOfWeek != DayOfWeek.Sunday
                  && date1.Value.DayOfWeek != DayOfWeek.Saturday
                   && date1.Value.DayOfWeek != DayOfWeek.Sunday
                   && date1.Value.Date != date2.Value.Date;
            if (!isRangeValid)
            {
                onNotValid();
            }
            return isRangeValid;
        }
        public static bool CheckDate(DateTime? date1, Action onNotValid)
        {
            var isRangeValid =  date1.Value != null && date1 >= new DateTime(2002, 1, 1) && date1.Value <= DateTime.Now
                  && date1.Value.DayOfWeek != DayOfWeek.Saturday
                   && date1.Value.DayOfWeek != DayOfWeek.Sunday;
            if (!isRangeValid)
            {
                onNotValid();
            }
            return isRangeValid;
        }
    }
}
