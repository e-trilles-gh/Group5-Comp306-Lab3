using Amazon.DynamoDBv2.DataModel;
using System.Globalization;

namespace group_5_Mohti_Trilles__Lab_3.Model
{
    [DynamoDBTable("PodcastStatistics")]
    public class PodcastStatistic
    {
        [DynamoDBHashKey]
        public string StatisticType { get; set; }

        [DynamoDBRangeKey]
        public string StatisticId { get; set; }

        [DynamoDBProperty]
        public int Value { get; set; }

        [DynamoDBProperty]
        public string LastUpdated { get; set; }
    }
}
