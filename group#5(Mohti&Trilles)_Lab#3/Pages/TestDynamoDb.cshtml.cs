using group_5_Mohti_Trilles__Lab_3.Model;
using group_5_Mohti_Trilles__Lab_3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace group_5_Mohti_Trilles__Lab_3.Pages
{
    public class TestDynamoDbModel : PageModel
    {
        private readonly StatisticsService _statisticsService;

        public string Message {  get; set; }

        public List<PodcastStatistic> Statistics { get; set; } = new();

        public TestDynamoDbModel(StatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        public async Task OnGetAsync()
        {
            try
            {
                //saves a test statistic
                var statistic = new PodcastStatistic
                {
                    StatisticType = "ConnectionTest",
                    StatisticId = "FirstTest",
                    Value = 1,
                    LastUpdated = DateTime.Now.ToString("O")
                };

                await _statisticsService.SaveStatistic(statistic);

                //reads statistics from DynamoDB
                Statistics = await _statisticsService.GetStatistics();

                Message = "Successfully connected to DynamoDB.";
            }
            catch (Exception ex)
            {
                Message = "Connection Failed: " + ex.Message;
            }
        }
    }
}
