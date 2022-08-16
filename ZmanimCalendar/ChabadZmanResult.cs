using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZmanimCalendar
{

    public class ChabadZmanResult
    {
        public Footnotes Footnotes { get; set; }
        public object[] FootnoteOrder { get; set; }
        public Day[] Days { get; set; }
        public Groupheading[] GroupHeadings { get; set; }
        public bool IsNewLocation { get; set; }
        public bool IsDefaultLocation { get; set; }
        public string LocationName { get; set; }
        public string City { get; set; }
        public Coordinates Coordinates { get; set; }
        public string LocationDetails { get; set; }
        public string EndDate { get; set; }
        public string GmtStartDate { get; set; }
        public string GmtEndDate { get; set; }
        public bool IsAdvanced { get; set; }
        public string PageTitle { get; set; }
        public string LocationId { get; set; }
    }

    public class Footnotes
    {
    }

    public class Coordinates
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class Day
    {
        public Timegroup[] TimeGroups { get; set; }
        public string Parsha { get; set; }
        public string HolidayName { get; set; }
        public bool IsHoliday { get; set; }
        public string DisplayDate { get; set; }
        public int DayOfWeek { get; set; }
        public bool IsDstActive { get; set; }
        public bool IsDayOfDstChange { get; set; }
        public string GmtDate { get; set; }
    }

    public class Timegroup
    {
        public string Title { get; set; }
        public string ZmanType { get; set; }
        public string FootnoteType { get; set; }
        public string HebrewTitle { get; set; }
        public int Order { get; set; }
        public string EssentialZmanType { get; set; }
        public string EssentialTitle { get; set; }
        public Item[] Items { get; set; }
        public int InfoMessageIndex { get; set; }
        public string InfoMessage { get; set; }
    }

    public class Item
    {
        public string EssentialZmanType { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public string FootnoteType { get; set; }
        public string EssentialTitle { get; set; }
        public string Link { get; set; }
        public string OpinionInformation { get; set; }
        public string OpinionDescription { get; set; }
        public string TechnicalInformation { get; set; }
        public string ZmanType { get; set; }
        public string Zman { get; set; }
        public string Date { get; set; }
        public int InfoMessageIndex { get; set; }
        public string InfoMessage { get; set; }
        public bool Default { get; set; }
    }

    public class Groupheading
    {
        public string EssentialZmanType { get; set; }
        public int Order { get; set; }
        public string EssentialTitle { get; set; }
    }

}
