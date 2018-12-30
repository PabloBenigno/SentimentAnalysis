using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using TextDataAnalysis;
using TextDataSource;

namespace SentimentTest
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }
        public static CognitiveServicesConfiguration CognitiveServicerConfiguration { get; set; }

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddUserSecrets<TwitterConfiguration>();

            Configuration = builder.Build();
            var twitterConfiguration = Configuration.GetSection(nameof(TwitterConfiguration)).Get<TwitterConfiguration>();
            CognitiveServicerConfiguration = Configuration.GetSection(nameof(CognitiveServicesConfiguration))
                .Get<CognitiveServicesConfiguration>();

            ITextDataSource dataSource = new TwitterDataSource(twitterConfiguration);

            var userName = Configuration.GetValue<string>("UserName");

            Console.WriteLine($"Retrieving timeline from {userName}. Please wait...");
            var timeline = dataSource.GetTimelineByUserName(userName);

            Console.WriteLine("\nTimeline from: {0}\n", userName);
            timeline.Tweets.ForEach(entry =>
                Console.WriteLine(
                    "ID: {0}, Number of replies {1} \nContent: {2}\n",
                    entry.TweetId, entry.Replies.Count, entry.Text));

            Console.ReadKey();

            ITextAnalysisClient textAnalysisClient = new AzureCongnitiveServicesAnalysisClient(CognitiveServicerConfiguration);
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            // Extracting sentiment
            Console.WriteLine("\n\n===== SENTIMENT ANALYSIS ======");

            foreach (var tweet in timeline.Tweets.Where(_ => _.Replies.Any()))
            {
                var sentimentInputs = tweet.Replies.Select(_ => new MultipleLanguageInput
                {
                    Id = _.TweetId.ToString(),
                    Text = _.Text,
                    Language = "es" // can be retrieved using the language detection
                }).ToList();

                var calculatedSentiments = textAnalysisClient.SentimentAsync(sentimentInputs).Result;

                var averageSentiment = calculatedSentiments.Documents.Average(_ => _.Score);


                Console.WriteLine($"Analysis of tweet with ID {tweet.TweetId} with text: \n {tweet.Text} \n The average sentiment of the replies is {averageSentiment}");
                foreach (var document in calculatedSentiments.Documents)
                {
                    var reply = tweet.Replies.Single(_ => _.TweetId.ToString() == document.Id);
                    Console.WriteLine($"Document ID: {document.Id} , Sentiment Score: {document.Score} \n {reply.Text}");
                }

                Console.WriteLine("Press a key to continue...");
                Console.ReadKey();
            }

            
            Console.WriteLine("End. Press ENTER to close.");
            Console.ReadLine();
        }


    }
}
