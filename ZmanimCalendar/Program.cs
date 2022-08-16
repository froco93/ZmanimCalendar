// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using System.Text.Json.Serialization;
using ZmanimCalendar;

var input = ConsoleInput.GetUserInput();

Console.WriteLine($"Creating Calender for Zip code {input.ZipCode}, Start Date {input.StartDate:d}, End Date {input.EndDate:d} ");

var calendarBuilder = new CalendarBuilder(input);

var result = calendarBuilder.CalculateCalendar();

var fileName = $"Output_{input.Format()}";
Console.WriteLine($"Writing results to {fileName}");

using (TextWriter tw = File.CreateText(fileName + ".csv"))
{
    tw.WriteLine(result.ToCsv(","));
}

using (TextWriter tw = File.CreateText(fileName + ".html"))
{
    tw.WriteLine(result.ToHtmlTable());
}

Console.WriteLine("Press any key to exit");
Console.ReadLine();
