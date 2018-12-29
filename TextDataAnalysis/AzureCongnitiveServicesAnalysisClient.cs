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

        public class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            private readonly string _subscriptionKey;//Insert your Text Anaytics subscription key

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