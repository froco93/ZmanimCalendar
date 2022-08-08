using FluentAssertions;
using Moq;
using System.Diagnostics;
using System.Text.Json;
using ZmanimCalendar;

namespace ZmanimCalanedarTests
{
    public class CalendarBuilderTests
    {
        private readonly Mock<IChabadZmanimService> mockZmanService;
        private readonly List<DayResult> smallExpectedResults;
        private List<DayResult> fullYearExpectedResults;

        public CalendarBuilderTests()
        {
            mockZmanService = new Mock<IChabadZmanimService>();

            string fullYearExpectedResultsFile = File.ReadAllText(@"FullYearExpectedResults.json");
            var parsedResults = JsonSerializer.Deserialize<ExpectedResults>(fullYearExpectedResultsFile) ?? throw new Exception();
            fullYearExpectedResults = parsedResults.Results;

            string smallExpectedResultsFile = File.ReadAllText(@"SmallExpectedResults.json");
            parsedResults = JsonSerializer.Deserialize<ExpectedResults>(smallExpectedResultsFile) ?? throw new Exception();
            smallExpectedResults = parsedResults.Results;

            var mockChabadData = File.ReadAllText(@"MockChabadData.json");
            var parsedData = JsonSerializer.Deserialize<ChabadZmanResult>(mockChabadData) ?? throw new Exception();
            mockZmanService.Setup(_ => _.GetChabadZmanResults(It.IsAny<UserInput>())).Returns(new List<ChabadZmanResult> { parsedData });
        }

        [Fact]
        public void FullYear_CallsChabad_ReturnsValidResults()
        {
            var fullYearInput = new UserInput("98115", DateTime.Parse("2021-09-01"), DateTime.Parse("2022-10-11"));

            var calendarBuilder = new CalendarBuilder(fullYearInput);

            var result = calendarBuilder.CalculateCalendar();

            result.Should().Equal(fullYearExpectedResults);
        }

        [Fact]
        public void SmallRange_WithTestZmanim_ReturnsValidResults()
        {
            var fullYearInput = new UserInput("98115", DateTime.Parse("7/17/2022"), DateTime.Parse("8/1/2022"));

            var calendarBuilder = new CalendarBuilder(fullYearInput, mockZmanService.Object);

            var result = calendarBuilder.CalculateCalendar();

            result.Should().Equal(smallExpectedResults);
        }
    }
}