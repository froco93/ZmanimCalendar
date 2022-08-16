using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZmanimCalendar
{
    public static class CalendarExtensions
    {
        public static bool IsSpecialFastDay(this string holidayName)
        {
            if (string.IsNullOrEmpty(holidayName))
            {
                return false;
            }

            return holidayName.Contains("Yom Kippur") || holidayName.Contains("Av");
        }

        public static bool IsErev(this Day day)
        {
            return day.HolidayName.StartsWith("Eve");
        }

        public static DayResult GetShabbatResult(this Day saturday, string candleLighting, string fridayDate)
        {
            string shabbatEndTime = saturday.TimeGroups.FirstOrDefault(_ => _.EssentialZmanType == "Tzeis")?
                .Items.FirstOrDefault(item => item.TechnicalInformation == "7.083 degrees")?.Zman ?? string.Empty;

            return new DayResult(fridayDate, candleLighting, shabbatEndTime, saturday.Parsha);
        }

        public static DayResult GetHolidayEveResult(this Day day, string candleLighting, string holidayDay1Date)
        {
            string holidayEndTime = day.TimeGroups.FirstOrDefault(_ => _.EssentialZmanType == "Tzeis")?
                .Items.FirstOrDefault(item => item.TechnicalInformation == "7.083 degrees")?.Zman ?? string.Empty;

            return new DayResult(holidayDay1Date, candleLighting, holidayEndTime, ParseHolidayName(day.Parsha));
        }

        public static string GetSecondDayHolidayCandlesTime(this Day day)
        {
            if (day.DayOfWeek == 5)
            {
                return day.TimeGroups.FirstOrDefault(timeGroup => timeGroup.EssentialZmanType == "CandleLighting")?
                    .Items.FirstOrDefault()?.Zman ?? string.Empty;
            }

            return day.TimeGroups.FirstOrDefault(_ => _.EssentialZmanType == "Tzeis")?
                 .Items.FirstOrDefault(item => item.TechnicalInformation == "7.083 degrees")?.Zman ?? string.Empty;
        }

        public static DayResult GetFastTimes(this Day fastDay)
        {
            var sunrise = fastDay.TimeGroups.FirstOrDefault(_ => _.EssentialZmanType == "NetzHachamah")?
                .Items.FirstOrDefault(item => item.EssentialZmanType == "NetzHachamah")?.Zman ?? string.Empty;
            // Fast start time is 72 minutes before sunrise.
            string fastStart = string.Empty;
            if(DateTime.TryParse(sunrise, out var sunriseDate))
            {
                fastStart = sunriseDate.Subtract(TimeSpan.FromMinutes(72)).ToShortTimeString();
            }

            string fastEnd = fastDay.TimeGroups.FirstOrDefault(_ => _.EssentialZmanType == "Tzeis")?
                .Items.FirstOrDefault(item => item.TechnicalInformation == "5.83 degrees")?.Zman ?? string.Empty;

            return new DayResult(fastDay.DisplayDate, fastStart, fastEnd, fastDay.HolidayName);
        }

        public static DayResult GetAvTimes(this Day fastDay, string candleLighting, string startDate)
        {
            string fastEnd = fastDay.TimeGroups.FirstOrDefault(_ => _.EssentialZmanType == "Tzeis")?
                .Items.FirstOrDefault(item => item.TechnicalInformation == "5.83 degrees")?.Zman ?? string.Empty;

            return new DayResult(startDate, candleLighting, fastEnd, fastDay.HolidayName);
        }

        public static string GetShkiahCandleLightingTime(this Day day)
        {
            return day.TimeGroups.FirstOrDefault(timeGroup => timeGroup.EssentialZmanType == "CandleLighting")?
                .Items.FirstOrDefault()?.Zman ?? string.Empty;
        }

        public static string ParseHolidayName(this string name)
        {
            if (name.StartsWith("Eve"))
            {
                return name.Replace("Eve of First day ", "").Replace("Eve of", "");
            }
            return name;
        }

        public static string ToCsv<T>(this IEnumerable<T> objectlist, string separator)
        {
            Type t = typeof(T);
            PropertyInfo[] properties = t.GetProperties();

            string header = String.Join(separator, properties.Select(f => f.Name).ToArray());

            StringBuilder csvdata = new StringBuilder();
            csvdata.AppendLine(header);

            foreach (var o in objectlist)
                csvdata.AppendLine(ToCsvFields(separator, properties, o));

            return csvdata.ToString();
        }

        public static bool IsChag(this Day day)
        {
            return day.IsHoliday &&
                !day.HolidayName.Contains("Chanukah") &&
                !day.HolidayName.Contains("Purim") &&
                !day.HolidayName.Contains("Intermediate");
        }

        private static string ToCsvFields(string separator, PropertyInfo[] fields, object o)
        {
            StringBuilder linie = new StringBuilder();

            foreach (var f in fields)
            {
                if (linie.Length > 0)
                    linie.Append(separator);

                var x = f.GetValue(o);

                if (x != null)
                    linie.Append(x.ToString());
            }

            return linie.ToString();
        }
    }
}
