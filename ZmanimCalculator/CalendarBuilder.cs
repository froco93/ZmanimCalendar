using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ZmanimCalendar
{
    public class CalendarBuilder
    {
        private readonly string? zipCode;
        private readonly DateTime startDate;
        private readonly DateTime endDate;
        private readonly IChabadZmanimService chabadZmanimService;
        private readonly UserInput userInput;

        public CalendarBuilder(UserInput userInput)
        {
            this.chabadZmanimService = new ChabadZmanimService();
            this.userInput = userInput;
        }

        public CalendarBuilder(UserInput userInput, IChabadZmanimService chabadZmanimService)
        {
            this.userInput = userInput;
            this.chabadZmanimService = chabadZmanimService;
        }

        public List<DayResult> CalculateCalendar()
        {
            var results = new List<DayResult>(); 
            var listOfTimesByDay = chabadZmanimService.GetChabadZmanResults(userInput).SelectMany(_ => _.Days);

            var filteredDays = listOfTimesByDay.Where(_ =>
                _.IsHoliday ||
                _.DayOfWeek == 6 ||
                (_.TimeGroups != null &&
                _.TimeGroups.Any(timeGroup => timeGroup?.EssentialZmanType == "CandleLighting")));

            string candles = string.Empty;
            string prevDate = string.Empty;
            var holidayCount = 0;
            string shkiah = string.Empty;
            bool candlesSet = false;
            foreach (var day in filteredDays)
            {
                DayResult? dayResult = null;

                if (day.Parsha == "Fast")
                {
                    dayResult = day.GetFastTimes();
                }
                else if (day.HolidayName.IsSpecialFastDay())
                {
                    if (day.HolidayName.StartsWith("Eve"))
                    {
                        // If 9 Av Starts Saturday Night, set candles to Shkiah instead of candle lighting
                        if (day.HolidayName.Contains("Av"))
                        {
                            candles = day.TimeGroups.FirstOrDefault(_ => _.EssentialZmanType == "Shkiah")?
                                .Items.FirstOrDefault()?.Zman ?? string.Empty;
                            candlesSet = true;
                        }
                    }
                    else
                    {
                        if (day.HolidayName == ("Yom Kippur"))
                        {
                            dayResult = day.GetShabbatResult(candles, prevDate);
                        }
                        // 9 Av
                        else
                        {
                            dayResult = day.GetAvTimes(shkiah, prevDate);
                        }
                    }

                }
                else if (day.IsChag())
                {
                    // Eve of Holiday
                    if (day.IsErev())
                    {
                        // if start of holiday is on saturday night
                        // Finish Shabbat row with just candlelighting
                        // Add first day of holiday with shabbat time as candle lighting
                        if (day.DayOfWeek == 6)
                        {
                            dayResult = new DayResult(prevDate, candles, string.Empty, day.Parsha);
                            results.Add(dayResult);

                            dayResult = new DayResult(day.DisplayDate, day.GetSecondDayHolidayCandlesTime(), string.Empty, day.HolidayName.ParseHolidayName());
                        }
                        else
                        {
                            dayResult = new DayResult(day.DisplayDate, day.GetShkiahCandleLightingTime(), string.Empty, day.HolidayName.ParseHolidayName());
                        }
                    }
                    // First Day Holiday
                    else if (holidayCount == 0)
                    {
                        candles = day.GetSecondDayHolidayCandlesTime();
                        candlesSet = true;
                        prevDate = day.DisplayDate;
                        holidayCount++;
                    }
                    else if (holidayCount > 0)
                    {
                        dayResult = day.GetHolidayEveResult(candles, prevDate);
                        holidayCount = 0;
                    }
                    else
                    {

                    }

                }
                // If Shabbat 
                else if (day.DayOfWeek == 6)
                {
                    dayResult = day.GetShabbatResult(candles, prevDate);
                }

                try
                {
                    if (!candlesSet)
                    {
                        var potentialCandles = day.TimeGroups.FirstOrDefault(timeGroup => timeGroup.EssentialZmanType == "CandleLighting");
                        if (potentialCandles != null)
                        {
                            candles = potentialCandles.Items.FirstOrDefault()?.Zman ?? string.Empty;
                            prevDate = day.DisplayDate;
                            candlesSet = true;
                        }
                    }
                }
                catch
                {
                    continue;
                }

                if (dayResult != null)
                {
                    results.Add(dayResult);
                }
                shkiah = day.TimeGroups.FirstOrDefault(timeGroup => timeGroup.EssentialZmanType == "Shkiah")?.Items.FirstOrDefault()?.Zman ?? string.Empty;
                candlesSet = false;
            }
            return results;
        }
    }
}
