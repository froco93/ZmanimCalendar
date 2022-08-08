﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZmanimCalendar
{
    public class ZmanResult
    {
        public DateTime? EndTime { get; set; }

        public string ParshaAndHoliday { get; set; }

        public bool IsShabbat { get; set; }

        public DateTime Date { get; set; }

        public DateTime? Candles { get; set; }
    }

    public class FastResult
    {
        public DateTime? EndTime { get; set; }

        public string ParshaAndHoliday { get; set; }

        public DateTime Date { get; set; }

        public DateTime? StartTime { get; set; }
    }
}
