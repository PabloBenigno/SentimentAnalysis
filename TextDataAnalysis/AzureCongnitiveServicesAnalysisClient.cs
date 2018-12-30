using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Rest;

namespace TextDataAnalysis
{
    public class AzureCongnitiveServicesAnalysisClient : ITextAnalysisClient
    {
        private readonly ITextAnalyticsClient _client;

        public AzureCongnitiveServicesAnalysisClient(CognitiveServicesConfiguration configuration)
        {
            // Create a client.
            _client = new TextAnalyticsClient(new ApiKeyServiceClientCredentials(configuration.SubscriptionKey))
            {
                Endpoint = configuration.EndPoint
            }; //Replace 'westus' with the correct region for your Text Analytics subscription

        }

        public async Task<DetectLanguageResult> DetectLanguageAsync(List<DetectLanguageInput> detectLanguageInputs)
        {
            var result = await _client.DetectLanguageAsync(new BatchInput(
                detectLanguageInputs.Select(_ => new Input(_.Id, _.Text)).ToList()));

            return new DetectLanguageResult
            {
                Documents = result.Documents.Select(_ => new DetectLanguageDocumentResult
                {
                    Id = _.Id,
                    DetectedLanguages = _.DetectedLanguages.Select(l => new LanguageResult
                    {
                        Name = l.Name,
                        Iso6391Name = l.Iso6391Name,
                        Score = l.Score
                    }).ToArray()
                })
            };
        }

        public async Task<KeyPhraseResult> KeyPhrasesAsync(List<MultipleLanguageInput> inputs)
        {
            var result = await _client.KeyPhrasesAsync(
                new MultiLanguageBatchInput(inputs.Select(_ => new MultiLanguageInput(_.Language, _.Id, _.Text))
                    .ToList()));
            return new KeyPhraseResult
            {
                Documents = result.Documents.Select(_ => new KeyPhraseDocumentResult
                {
                    Id = _.Id,
                    KeyPhrases = _.KeyPhrases
                })
            };
        }

        public async Task<SentimentResult> SentimentAsync(List<MultipleLanguageInput> inputs)
        {
            if (inputs.Count == 0) return new SentimentResult{Documents = new SentimentDocumentResult[]{}};

            var result = await _client.SentimentAsync(
                new MultiLanguageBatchInput(inputs.Select(_ => new MultiLanguageInput(_.Language, _.Id, _.Text))
                    .ToList()));
            return new SentimentResult
            {
                Documents = result.Documents.Select(_ => new SentimentDocumentResult
                {
                    Id = _.Id,
                    Score = _.Score
                })
            };
        }

        public async Task<EntitiesResult> EntitiesAsync(List<MultipleLanguageInput> inputs)
        {
            var result = await _client.EntitiesAsync(
                new MultiLanguageBatchInput(inputs.Select(_ => new MultiLanguageInput(_.Language, _.Id, _.Text))
                    .ToList()));
            return new EntitiesResult
            {
                Documents = result.Documents.Select(_ => new EntitiesDocumentResult
                {
                    Id = _.Id,
                    Entities = _.Entities.Select(e => new EntityDocument
                    {
                        Name = e.Name
                    })
                })
            };
        }

        public class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            private readonly string _subscriptionKey;

            public ApiKeyServiceClientCredentials(string subscriptionKey)
            {
                _subscriptionKey = subscriptionKey;
            }

            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }
    }
}