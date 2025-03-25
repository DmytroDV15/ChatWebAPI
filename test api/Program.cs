using System;
using Azure;
using Azure.AI.TextAnalytics;

class Program
{
    static void Main(string[] args)
    {
        // Replace with your endpoint and key
        string endpoint = "https://chat-language-service.cognitiveservices.azure.com/";
        string apiKey = "GIBiYtyaPR6ki3aJMi2pbm1g4UWq6yIRfYHTxPKBaCYdSq5UonDHJQQJ99ALACi5YpzXJ3w3AAAaACOGOA5E";

        // Create the client
        var client = new TextAnalyticsClient(new Uri(endpoint), new AzureKeyCredential(apiKey));

        // Test input
        string inputText = "I love this product! It's amazing.";

        // Perform sentiment analysis
        try
        {
            var response = client.AnalyzeSentiment(inputText);
            Console.WriteLine($"Text: {inputText}");
            Console.WriteLine($"Sentiment: {response.Value.Sentiment}");
            Console.WriteLine($"Confidence Scores: Positive={response.Value.ConfidenceScores.Positive}, Neutral={response.Value.ConfidenceScores.Neutral}, Negative={response.Value.ConfidenceScores.Negative}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
