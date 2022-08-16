using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZmanimCalendar
{
    public static class ConsoleInput
    {
        public const string USZipRegEx = @"^\d{5}(?:[-\s]\d{4})?$";

        public static UserInput GetUserInput()
        {
            var zipCode = GetZipCode();
            var startDate = GetDate("Start");
            var endDate = GetDate("End");

            return new UserInput(zipCode, startDate, endDate);

            static string GetZipCode()
            {
                while (true)
                {
                    Console.WriteLine("Enter Zip Code:");
                    string zipCodeString = Console.ReadLine() ?? string.Empty;
                    if (zipCodeString == null || !Regex.Match(zipCodeString, USZipRegEx).Success)
                    {
                        Console.WriteLine("Invalid Zip Code. Please enter valid US Zip Code");
                    }
                    else
                    {
                        return zipCodeString;
                    }
                }
            }

            static DateTime GetDate(string dateType)
            {
                while (true)
                {
                    Console.WriteLine($"Enter {dateType} Date (YYYY-MM-DD):");
                    string dateTimeString = Console.ReadLine() ?? string.Empty;
                    if (!DateTime.TryParse(dateTimeString, out var dateTime))
                    {
                        Console.WriteLine("Invalid Date Time. Please enter valid date. (YYYY-MM-DD)");
                    }
                    else
                    {
                        return dateTime;
                    }
                }
            }
        }
    }
}
