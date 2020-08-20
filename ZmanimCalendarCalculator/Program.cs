using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class Program
    {

        static void Main(string[] args)
        {

            // Enter StartDate, EndDate, LocationID
            var startTime = DateTime.Now;
            var endTime = DateTime.Parse("2021-09-05");
            var locationID = "US98115";

            var calculator = new MyZmanimDotComZmanimCalculator(locationID);
            var calendarCreator = new ZmanimCalendarCreator(calculator);

            var result = calendarCreator.CreateCalendar(startTime, endTime).GetAwaiter().GetResult();

            File.WriteAllText(@"C:\dev\zmanim\5781.csv", CreateCSVTextFile(result, ","));
            Console.ReadLine();
        }


        private static string CreateCSVTextFile(List<ZmanResult> data, string seperator = ",")
        {
            var result = new StringBuilder();
            var headers = new List<string> { nameof(ZmanResult.Date), nameof(ZmanResult.IsShabbat), nameof(ZmanResult.Sunset), nameof(ZmanResult.EndTime), nameof(ZmanResult.ParshaAndHoliday)};
            var headerLine = string.Join(seperator, headers);
            result.AppendLine(headerLine);

            foreach (var row in data)
            {
                var values = new List<string> { row.Date.ToShortDateString(), row.IsShabbat.ToString(), row.Sunset.ToShortTimeString(), row.EndTime.ToShortTimeString(), row.ParshaAndHoliday};
                var line = string.Join(seperator, values);
                result.AppendLine(line);
            }

            return result.ToString();
        }
    }
}
