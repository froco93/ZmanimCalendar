using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    public interface IZmanimCalculator
    {
        Task<DateTime> GetShabbatEndTime(DateTime date);

        DateTime GetShabbatEndTime(EngineResultDay date);

        Task<EngineResultDay> GetZmanimByDay(DateTime date);
    }
}
