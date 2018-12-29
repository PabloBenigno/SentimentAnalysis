using System.Collections.Generic;
using System.Threading.Tasks;

namespace TextDataAnalysis
{
    public interface ITextAnalysisClient
    {
        Task<DetectLanguageResult> DetectLanguageAsync(List<DetectLanguageInput> detectLanguageInputs);
    }
}
