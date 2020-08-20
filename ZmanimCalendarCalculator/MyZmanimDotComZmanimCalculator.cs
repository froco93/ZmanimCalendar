using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    public class MyZmanimDotComZmanimCalculator : IZmanimCalculator
    {
        private const string APIURL = "https://api.myzmanim.com/engine1.svc";
        private const string APIUSER = "fill in";
        private const string APIKEY = "fill in";
        private readonly string location;
        private readonly string language;
        private readonly EngineClient client;

        public MyZmanimDotComZmanimCalculator(string location, string language = "en")
        {
            this.location = location ?? throw new ArgumentNullException(nameof(location));
            this.language = language ?? throw new ArgumentNullException(nameof(language));
            this.client = CreateApiInstance();
        }

        public async Task<DateTime> GetShabbatEndTime(DateTime date)
        {
            var dayTimes = await GetZmanimByDay(date);
            var weightedTicks = dayTimes.Zman.NightGra240.Ticks * .1 + dayTimes.Zman.NightMoed.Ticks * .9;
            return new DateTime((long) weightedTicks);
        }

        public DateTime GetShabbatEndTime(EngineResultDay zmanTimesForDay)
        {
            var weightedTicks = zmanTimesForDay.Zman.NightGra240.Ticks * .1 + zmanTimesForDay.Zman.NightMoed.Ticks * .9;
            return new DateTime((long)weightedTicks);
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
    }
}
