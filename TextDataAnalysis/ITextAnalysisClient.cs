using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;

namespace TextDataAnalysis
{
    public interface ITextAnalysisClient
    {
        Task<DetectLanguageResult> DetectLanguageAsync(List<DetectLanguageInput> detectLanguageInputs);
        Task<KeyPhraseResult> KeyPhrasesAsync(List<MultipleLanguageInput> inputs);
        Task<SentimentResult> SentimentAsync(List<MultipleLanguageInput> inputs);
        Task<EntitiesResult> EntitiesAsync(List<MultipleLanguageInput> inputs);
    }
}
