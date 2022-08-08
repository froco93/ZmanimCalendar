// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using System.Text.Json.Serialization;
using ZmanimCalendar;

var input = ConsoleInput.GetUserInput();
//var input = new UserInput("98115", DateTime.Parse("2022-09-01"), DateTime.Parse("2022-10-11"));
//var input = new UserInput("98115", DateTime.Parse("2022-06-01"), DateTime.Parse("2022-06-11"));

Console.WriteLine($"Creating Calender for Zip code {input.ZipCode}, Start Date {input.StartDate:d}, End Date {input.EndDate:d} ");

var calendarBuilder = new CalendarBuilder(input);

var result = calendarBuilder.CalculateCalendar();


using (TextWriter tw = File.CreateText($"Output_{input.Format()}.csv"))
{
    tw.WriteLine(result.ToCsv(",")); 
}

var c = Console.ReadLine();

c.ToString();
