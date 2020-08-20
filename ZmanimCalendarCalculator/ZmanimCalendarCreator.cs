using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
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
            while (indexDate < endTime)
            {
                var zmanimforDay = await zmanimCalculator.GetZmanimByDay(indexDate);
                if (ShouldAddZmanToCalandar(zmanimforDay))
                {
                    var shabbatEndTime = zmanimCalculator.GetShabbatEndTime(zmanimforDay);
                    Console.WriteLine($"Writing result for Date {indexDate.ToShortDateString()}");
                    zmanimResultList.Add(new ZmanResult
                    {
                        IsShabbat = zmanimforDay.Time.IsShabbos,
                        Date = zmanimforDay.Time.DateCivil,
                        EndTime = shabbatEndTime,
                        ParshaAndHoliday = zmanimforDay.Time.ParshaAndHoliday,
                        Sunset = zmanimforDay.Zman.SunsetLevel,
                    });
                }
                indexDate = indexDate.AddDays(1);
            }
            return zmanimResultList;
        }

        private bool ShouldAddZmanToCalandar(EngineResultDay zmanimforDay)
        {
            if (zmanimforDay.Time.IsShabbos)
            {
                return true;
            }
            if(zmanimforDay.Time.IsYomTov && !zmanimforDay.Time.TomorrowNightIsYomTov)
            {
                return true;
            }

            return false;
        }
    }
}
