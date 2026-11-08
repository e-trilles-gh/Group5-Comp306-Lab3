using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using group_5_Mohti_Trilles__Lab_3.Model;

namespace group_5_Mohti_Trilles__Lab_3.Services
{
    public class StatisticsService
    {
        private readonly DynamoDBContext _dynamoDBContext;

        public StatisticsService(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDBContext = new DynamoDBContext(dynamoDb);
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