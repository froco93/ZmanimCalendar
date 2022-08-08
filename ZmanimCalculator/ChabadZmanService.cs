using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ZmanimCalendar
{
    public class ChabadZmanimService : IChabadZmanimService
    {
        private const string chabadApiUri = "webservices/zmanim/zmanim/Get_Zmanim?additional=true&locationid={0}&locationtype=2&save=1&tdate={1}&jewish=Halachic-Times.htm&aid=143790&startdate={1}&enddate={2}";
        private const string acceptHeader = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
        private readonly HttpClient chabadHttpClient;

        public ChabadZmanimService()
        {
            chabadHttpClient = new HttpClient() { BaseAddress = new Uri("https://www.chabad.org") };
        }

        public IEnumerable<ChabadZmanResult> GetChabadZmanResults(UserInput userInput)
        {

            TimeSpan interval = TimeSpan.FromDays(10);
            DateTime intervalStartTime = userInput.StartDate;
            while (intervalStartTime < userInput.EndDate)
            {
                var intervalEnd = intervalStartTime + interval;
                string content = GetZmanBlock(intervalEnd).GetAwaiter().GetResult();
                Console.WriteLine($"Retrived Times between {intervalStartTime:s} and {intervalEnd:s}");
                intervalStartTime = intervalEnd.AddDays(1);
                yield return JsonSerializer.Deserialize<ChabadZmanResult>(content);
            }

            async Task<string> GetZmanBlock(DateTime intervalEnd)
            {
                var apiUri = string.Format(
                                    chabadApiUri,
                                    userInput.ZipCode,
                                    intervalStartTime.ToString("u").Substring(0, 10),
                                    intervalEnd.ToString("u").Substring(0, 10));
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, apiUri);
                chabadHttpClient.DefaultRequestHeaders.Add("Accept", acceptHeader);

                var httpResponse = await chabadHttpClient.SendAsync(httpRequest);
                var content = await httpResponse.Content.ReadAsStringAsync();
                return content;
            }
        }
    }
}
