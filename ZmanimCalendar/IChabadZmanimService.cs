using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZmanimCalendar
{
    public interface IChabadZmanimService
    {
        public IEnumerable<ChabadZmanResult> GetChabadZmanResults(UserInput userInput);
    }
}
