using System.Collections.Generic;

namespace TextDataAnalysis
{
    public class DetectLanguageResult
    {
        public IEnumerable<DetectLanguageDocumentResult> Documents { get; set; }
    }
}