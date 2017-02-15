using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBPClient.ViewModels;
using NBPClient.Models;

namespace NBPClient.Core.Infrastructure
{
    public static class LinqExtensions
    { 
        public static IEnumerable<CurrencyModel> WhenSome(this IEnumerable<CurrencyModel> source, Action a)
        {
            if (source.Count() > 0)
                a();
            return source;
        }
        public static IEnumerable<CurrencyModel> WhenNone(this IEnumerable<CurrencyModel> source, Action a)
        {
            if (source.Count() == 0)
                a();
            return source;
        }
    }
}
