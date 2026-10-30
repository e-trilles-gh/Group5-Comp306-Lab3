using Amazon.DynamoDBv2.DataModel;
using group_5_Mohti_Trilles__Lab_3.Model;

namespace group_5_Mohti_Trilles__Lab_3.Services
{
    public class StatisticsService
    {
        private readonly DynamoDBContext _dynamoDBContext;

        public StatisticsService(DynamoDBContext dynamoDBContext)
        {
            _dynamoDBContext = dynamoDBContext;
        }

        public async Task SaveStatistic(PodcastStatistic statistic)
        {
            await _dynamoDBContext.SaveAsync(statistic);
        }

        public async Task<List<PodcastStatistic>> GetStatistics()
        {
            var results = await _dynamoDBContext.ScanAsync<PodcastStatistic>(
                new List<ScanCondition>()).GetRemainingAsync();

            return results;
        }
    }
}
