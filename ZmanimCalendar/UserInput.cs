using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZmanimCalendar
{
    public class UserInput
    {
        public UserInput(string zipCode, DateTime startDate, DateTime endDate)
        {
            ZipCode = zipCode;
            StartDate = startDate;
            EndDate = endDate;
        }

        public string ZipCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Format()
        {
            return $"{ZipCode}_{StartDate:yyyy_MM_dd}_{EndDate:yyyy_MM_dd}";
        }
    }
}
