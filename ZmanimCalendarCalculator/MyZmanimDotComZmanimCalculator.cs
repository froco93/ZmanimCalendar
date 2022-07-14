using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    public class MyZmanimDotComZmanimCalculator : IZmanimCalculator
    {
        private const string APIURL = "https://api.myzmanim.com/engine1.svc";
        private const string APIUSER = "0013121284";
        private const string APIKEY = "";
        private const string chabadApiUri = "webservices/zmanim/zmanim/Get_Zmanim?additional=true&locationid=98115&locationtype=2&save=1&tdate={0}&jewish=Halachic-Times.htm&aid=143790&startdate={0}&enddate={0}";
        private readonly string location;
        private readonly string language;
        private readonly EngineClient client;
        private readonly HttpClient chabadHttpClient;

        public MyZmanimDotComZmanimCalculator(string location, string language = "en")
        {
            this.location = location ?? throw new ArgumentNullException(nameof(location));
            this.language = language ?? throw new ArgumentNullException(nameof(language));
            this.client = CreateApiInstance();
            chabadHttpClient = new HttpClient() { BaseAddress = new Uri("https://www.chabad.org") };
        }

        public async Task<DateTime> GetShabbatEndTime(DateTime date)
        {

            var dayTimes = await GetZmanimByDay(date);
            var weightedTicks = dayTimes.Zman.NightGra240.Ticks * .1 + dayTimes.Zman.NightMoed.Ticks * .9;
            return new DateTime((long)weightedTicks);

        }

        public async Task<DateTime> GetShabbatEndTime(EngineResultDay zmanTimesForDay)
        {
            try { 
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, string.Format(chabadApiUri, zmanTimesForDay.Time.DateCivil.ToString("u").Substring(0,10)));
                chabadHttpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");

                var httpResponse = await chabadHttpClient.SendAsync(httpRequest);
                var content = await httpResponse.Content.ReadAsStringAsync();
                var jsonResult = JObject.Parse(content);

                var time = ((jsonResult["Days"][0]["TimeGroups"].Where(_ => _["ZmanType"].ToString() == "Tzeis").FirstOrDefault() as JObject)["Items"].Where(item => item["TechnicalInformation"].ToString() == "7.083 degrees").FirstOrDefault() as JObject)["Zman"];

                return zmanTimesForDay.Time.DateCivil + DateTime.Parse(time.ToString()).TimeOfDay;
            }
            catch
            {
                var weightedTicks = zmanTimesForDay.Zman.NightGra240.Ticks * .1 + zmanTimesForDay.Zman.NightMoed.Ticks * .9;
                return Round(new DateTime((long)weightedTicks).Add(TimeSpan.FromMinutes(-3.5)), TimeSpan.FromMinutes(1));
            }
        }

        public async Task<EngineResultDay> GetZmanimByDay(DateTime date)
        {
            var result = await client.GetDayAsync(CreateEngineParamDay(date));
            if (result.ErrMsg != null)
            {
                throw new Exception($"Exception occured with message {result.ErrMsg}");
            }
            return result;
        }

        private static EngineClient CreateApiInstance()
        {
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
            System.ServiceModel.EndpointAddress address = new System.ServiceModel.EndpointAddress(APIURL);
            if (APIURL.Contains("https://"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            EngineClient Client = new EngineClient(binding, address);
            return Client;
        }

        private EngineParamDay CreateEngineParamDay(DateTime requestedDate)
        {
            EngineParamDay engineParam = new EngineParamDay();

            engineParam.User = APIUSER;
            engineParam.Key = APIKEY;
            engineParam.Coding = "CS";
            engineParam.Language = language;
            engineParam.InputDate = requestedDate;
            engineParam.LocationID = location;
            return engineParam;
        }

        private static DateTime Round(DateTime date, TimeSpan span)
        {
            long ticks = (date.Ticks + (span.Ticks / 2) + 1) / span.Ticks;
            return new DateTime(ticks * span.Ticks);
        }
    }
}
