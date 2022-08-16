using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZmanimCalendar
{
    public class DayResult
    {
        public DayResult(string date, string startTime, string endTime, string title)
        {
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
            // Replace commas with dashes so there are no csv conflicts.
            //Title = Encoding.Unicode.GetString(Encoding.Unicode.GetBytes(title.Replace(",", "-")));
            Title = title.Replace(",", "-");
        }

        public string Date { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string Title { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is DayResult result &&
                   Date == result.Date &&
                   StartTime == result.StartTime &&
                   EndTime == result.EndTime &&
                   Title == result.Title;
        }
    }
}
