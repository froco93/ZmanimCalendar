using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZmanimCalendar
{
    public class ZmanimCalendarCreator
    {
        private readonly IZmanimCalculator zmanimCalculator;

        public ZmanimCalendarCreator(IZmanimCalculator zmanimCalculator)
        {
            this.zmanimCalculator = zmanimCalculator ?? throw new ArgumentNullException(nameof(zmanimCalculator));
        }

        public async Task<List<ZmanResult>> CreateCalendar(DateTime startTime, DateTime endTime)
        {
            DateTime indexDate = startTime;
            var zmanimResultList = new List<ZmanResult>();
            EngineResultDay yesterday = new EngineResultDay();
            while (indexDate < endTime)
            {
                var zmanimforToday = await zmanimCalculator.GetZmanimByDay(indexDate);
                if (ShouldAddZmanToCalandar(zmanimforToday))
                {
                    DateTime? shabbatEndTime = await zmanimCalculator.GetShabbatEndTime(zmanimforToday);
                    var candles = CalculateCandles(zmanimforToday, shabbatEndTime);


                    Console.WriteLine($"Writing result for Date {indexDate.ToShortDateString()}");
                    zmanimResultList.Add(new ZmanResult
                    {
                        IsShabbat = zmanimforToday.Time.IsShabbos,
                        Date = zmanimforToday.Time.DateCivil,
                        EndTime = candles == null ? shabbatEndTime : null,
                        ParshaAndHoliday = zmanimforToday.Time.ParshaAndHoliday,
                        Candles = candles,
                    });
                }
                indexDate = indexDate.AddDays(1);
                yesterday = zmanimforToday;
            }
            return zmanimResultList;
        }

        public async Task<List<FastResult>> CreateFasts(DateTime startTime, DateTime endTime)
        {
            DateTime indexDate = startTime;
            var zmanimResultList = new List<FastResult>();
            while (indexDate < endTime)
            {
                var zmanimforToday = await zmanimCalculator.GetZmanimByDay(indexDate);
                if (zmanimforToday.Time.IsFastDay || zmanimforToday.Time.IsErevTishaBav)
                {
                    DateTime? shabbatEndTime = await zmanimCalculator.GetShabbatEndTime(zmanimforToday);
                    var candles = CalculateCandles(zmanimforToday, shabbatEndTime);

                    Console.WriteLine($"Writing result for Date {indexDate.ToShortDateString()}");
                    zmanimResultList.Add(new FastResult
                    {
                        Date = zmanimforToday.Time.DateCivil,
                        EndTime = zmanimforToday.Zman.NightFastTuc,
                        ParshaAndHoliday = zmanimforToday.Time.ParshaAndHoliday,
                        StartTime = zmanimforToday.Zman.Dawn72fix,
                    });
                }
                indexDate = indexDate.AddDays(1);
            }
            return zmanimResultList;
        }

        private DateTime? CalculateCandles(EngineResultDay engineResultDay, DateTime? stars)
        {
            if (engineResultDay.Time.IsErevShabbos || engineResultDay.Time.IsErevYomTov)
            {
                return engineResultDay.Zman.Candles;
            }
            else if (engineResultDay.Time.IsYomTov && !engineResultDay.Time.TomorrowNightIsYomTov)
            {
                return stars;
            }
            return null;
        }

        private bool ShouldAddZmanToCalandar(EngineResultDay zmanimforDay)
        {
            if (zmanimforDay.Time.IsShabbos || zmanimforDay.Time.IsErevShabbos || zmanimforDay.Time.IsErevYomTov|| zmanimforDay.Time.IsYomTov)
            {
                return true;
            }
            if (zmanimforDay.Time.IsYomTov && !zmanimforDay.Time.TomorrowNightIsYomTov)
            {
                return true;
            }

            return false;
        }
    }
}
