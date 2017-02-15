using NBPClient.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NBPClient.Infrastructure
{
    public class WebServiceConsumer
    {
        static HttpClient client = new HttpClient();
        CancellationTokenSource cts;
        public static async Task<List<CurrencyModel>> GetCurrency(string path, Action onComplete)
        {
           
            HttpResponseMessage response = await client.GetAsync(path);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<CurrencyModel>();
            }
            else
            {
                string prod = await response.Content.ReadAsStringAsync();
                onComplete();
                var json = JArray.Parse(prod)[0].ToString();
                var list = JsonConvert.DeserializeObject<TableModel>(json);
                return list.Rates;
            }
        }
        public static async Task<List<RateMode>> GetRates(string path)
        {

            HttpResponseMessage response = await client.GetAsync(path);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<RateMode>();
            }
            else
            {
                string a = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var json = JObject.Parse(a).ToString();
                var list = JsonConvert.DeserializeObject<TableModel2>(json);
                return list.Rates;
            }
        }
    }
}
