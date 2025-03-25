using Azure.AI.TextAnalytics;

namespace ChatWebAPI.Services
{
    public class TextAnalyticsService
    {
        private readonly TextAnalyticsClient _client;

        public TextAnalyticsService(TextAnalyticsClient client)
        {
            _client = client;
        }

        public string AnalyzeSentiment(string text)
        {
            var response = _client.AnalyzeSentiment(text);
            return response.Value.Sentiment.ToString(); 
        }
    }
}
