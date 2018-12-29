using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Rest;
using TextDataAnalysis;
using TextDataSource;

namespace SentimentTest
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }
        public static ITextDataSource DataSource { get; set; }
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

            DataSource = new TwitterDataSource(twitterConfiguration);
            SearchDataResult searchDataResult = DataSource.GetSearchData("#Trifachito").Result;
            

            Console.WriteLine("\nQuery: {0}\n", searchDataResult.Query);
            searchDataResult.TextDataDocuments.ForEach(entry =>
                Console.WriteLine(
                    "ID: {0, -15}, Source: {1}\nContent: {2}\n",
                    entry.Id, entry.Source, entry.Text));

            Console.ReadKey();

            ITextAnalysisClient textAnalysisClient = new AzureCongnitiveServicesAnalysisClient(CognitiveServicerConfiguration);


            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Extracting language
            Console.WriteLine("===== LANGUAGE EXTRACTION ======");
            var result = textAnalysisClient.DetectLanguageAsync(new List<DetectLanguageInput>
            {
                new DetectLanguageInput
                {
                    Id = "1",
                    Text = "This is a document written in English."
                },
                new DetectLanguageInput
                {
                    Id = "2",
                    Text = "Este es un document escrito en Español."
                },
                new DetectLanguageInput
                {
                    Id = "3",
                    Text = "这是一个用中文写的文件"
                }
            }).Result;

            
            

            // Printing language results.
            foreach (var document in result.Documents)
            {
                Console.WriteLine("Document ID: {0} , Language: {1}", document.Id, document.DetectedLanguages[0].Name);
            }

            var jander = searchDataResult.TextDataDocuments.Select(_ => new DetectLanguageInput
            {
                Id = _.Id,
                Text = _.Text
            }).ToList();

            result = textAnalysisClient.DetectLanguageAsync(jander).Result;

            foreach (var document in result.Documents)
            {
                Console.WriteLine("Document ID: {0} , Language: {1}", document.Id, document.DetectedLanguages[0].Name);
            }

            // Getting key-phrases
            Console.WriteLine("\n\n===== KEY-PHRASE EXTRACTION ======");

            KeyPhraseResult result2 = textAnalysisClient.KeyPhrasesAsync(new List<KeyPhraseInput>()
                        {
                            new KeyPhraseInput
                            {
                                Language = "ja",
                                Id = "1",
                                Text = "猫は幸せ"
                            },
                            new KeyPhraseInput
                            {
                                Language = "de",
                                Id = "2",
                                Text = "Fahrt nach Stuttgart und dann zum Hotel zu Fu."
                            },
                            new KeyPhraseInput
                            {
                                Language = "en",
                                Id = "3",
                                Text = "My cat is stiff as a rock."
                            },
                            new KeyPhraseInput
                            {
                                Language = "es",
                                Id = "4",
                                Text = "A mi me encanta el fútbol!"
                            }
                        }).Result;

            // Printing keyphrases
            foreach (var document in result2.Documents)
            {
                Console.WriteLine("Document ID: {0} ", document.Id);

                Console.WriteLine("\t Key phrases:");

                foreach (string keyphrase in document.KeyPhrases)
                {
                    Console.WriteLine("\t\t" + keyphrase);
                }
            }

            // Extracting sentiment
            Console.WriteLine("\n\n===== SENTIMENT ANALYSIS ======");

            SentimentResult result3 = textAnalysisClient.SentimentAsync(
                    new List<SentimentInput>()
                        {
                            new SentimentInput
                            {
                                Language ="en",
                                Id = "0",
                                Text = "I had the best day of my life."
                            },
                            new SentimentInput
                            {
                                Language ="en",
                                Id = "1",
                                Text = "This was a waste of my time. The speaker put me to sleep."
                            },
                            new SentimentInput
                            {
                                Language ="es",
                                Id = "2",
                                Text = "No tengo dinero ni nada que dar..."
                            },
                            new SentimentInput
                            {
                                Language ="it",
                                Id = "3",
                                Text = "L'hotel veneziano era meraviglioso. È un bellissimo pezzo di architettura."
                            }
                        }).Result;


            // Printing sentiment results
            foreach (var document in result3.Documents)
            {
                Console.WriteLine("Document ID: {0} , Sentiment Score: {1:0.00}", document.Id, document.Score);
            }

            var jander3 = searchDataResult.TextDataDocuments.Select(_ => new SentimentInput
            {
                Id = _.Id.ToString(),
                Text = _.Text,
                Language = result.Documents.First(d => d.Id == _.Id.ToString()).DetectedLanguages.First()
                    .Iso6391Name
            }).ToList();

            result3 = textAnalysisClient.SentimentAsync(jander3).Result;
            foreach (var document in result3.Documents)
            {
                Console.WriteLine("Document ID: {0} , Sentiment Score: {1:0.00}", document.Id, document.Score);
            }

            //// Identify entities
            //Console.WriteLine("\n\n===== ENTITIES ======");

            //var result4 = client.EntitiesAsync(
            //        new MultiLanguageBatchInput(
            //            new List<MultiLanguageInput>()
            //            {
            //              new MultiLanguageInput("en", "0", "The Great Depression began in 1929. By 1933, the GDP in America fell by 25%.")
            //            })).Result;

            //// Printing entities results
            //foreach (var document in result4.Documents)
            //{
            //    Console.WriteLine("Document ID: {0} ", document.Id);

            //    Console.WriteLine("\t Entities:");

            //    foreach (var entity in document.Entities)
            //    {
            //        Console.WriteLine("\t\t" + entity.Name);
            //    }
            //}

            Console.ReadLine();
        }


    }
}
