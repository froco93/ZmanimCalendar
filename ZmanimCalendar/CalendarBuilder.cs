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

            // go to chabad and get all of the "Day" objects 
            var listOfTimesByDay = chabadZmanimService.GetChabadZmanResults(userInput).SelectMany(_ => _.Days);

            // Filter only days that are needed (Saturday, Holiday, or a day that has candle lighting)
            var filteredDays = listOfTimesByDay.Where(_ =>
                _.IsHoliday ||
                _.DayOfWeek == 6 ||
                (_.TimeGroups != null &&
                    (_.TimeGroups.Any(timeGroup => timeGroup?.EssentialZmanType == "CandleLighting") ||
                    _.TimeGroups.Any(timeGroup => timeGroup?.Title == "Sunset (Shkiah) | Fast Begins"))
                )
            );

            string candles = string.Empty;
            string prevDate = string.Empty;
            var holidayCount = 0;
            string shkiah = string.Empty;
            bool candlesSet = false;
            foreach (var day in filteredDays)
            {
                Console.WriteLine($"Processing date {day.DisplayDate}");
                DayResult? dayResult = null;

                // If its a fast day, get the fast times
                if (day.Parsha == "Fast")
                {
                    dayResult = day.GetFastTimes();
                }
                // If its a special fast day (9 Av or YomKippur), get the correct times
                else if (day.HolidayName.IsSpecialFastDay() || day.IsErev9Av())
                {
                    // For Eve of 9Av, override start time with shkiah instead of candle lighting
                    if (day.IsErev9Av() || day.HolidayName.StartsWith("Eve"))
                    {
                        if (day.IsErev9Av() || day.HolidayName.Contains("Av"))
                        {
                            candles = day.TimeGroups.FirstOrDefault(_ => _.EssentialZmanType == "Shkiah")?
                                .Items.FirstOrDefault()?.Zman ?? string.Empty;
                            candlesSet = true;
                            prevDate = day.DisplayDate;
                        }
                    }
                    else
                    {
                        // Yom Kippur, use candle lighiting and shabbat end times
                        if (day.HolidayName == ("Yom Kippur"))
                        {
                            dayResult = day.GetShabbatResult(candles, prevDate);
                        }
                        // 9 Av, use Shkiah and Fast end time
                        else
                        {
                            dayResult = day.GetAvTimes(shkiah, prevDate);
                        }
                    }

                }
                // If the day is a Chag, we will need to get correct times
                else if (day.IsChag())
                {
                    // Eve of Holiday
                    if (day.IsErev())
                    {
                        /*  if start of holiday is on saturday night
                            Finish Shabbat row with just candlelighting
                            Add first day of holiday with shabbat end time as candle lighting
                            eg: 06/03/22 Bamidbar 8:43, 06/03/22 Shavuot 1 9:43,
                        */
                        if (day.DayOfWeek == 6)
                        {
                            dayResult = new DayResult(prevDate, candles, string.Empty, day.Parsha);
                            results.Add(dayResult);

                            dayResult = new DayResult(day.DisplayDate, day.GetSecondDayHolidayCandlesTime(), string.Empty, day.HolidayName.ParseHolidayName());
                        }
                        // Otherwise just add standard row using 1st night candle lighting times
                        // eg:09/06/21 Rosh Hashana 1 7:20
                        else
                        {
                            dayResult = new DayResult(day.DisplayDate, day.GetShkiahCandleLightingTime(), string.Empty, day.HolidayName.ParseHolidayName());
                        }
                    }
                    // If its the first day of the holiday, just save the candle lighting times for second night. 
                    // Above code will already add 1st night candle times
                    else if (holidayCount == 0)
                    {
                        candles = day.GetSecondDayHolidayCandlesTime();
                        candlesSet = true;
                        prevDate = day.DisplayDate;
                        holidayCount++;
                    }
                    // If its the second night, use last nights candles plus tonight's end time to form the correct times
                    // eg: 09/07/21 8:15, 8:13
                    else if (holidayCount > 0)
                    {
                        dayResult = day.GetHolidayEveResult(candles, prevDate);
                        holidayCount = 0;
                    }

                }
                // If Shabbat, get regular shabbt times using last nights candles! 
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
