using System.Collections.Generic;

namespace TextDataAnalysis
{
    public class SentimentResult
    {
        public IEnumerable<SentimentDocumentResult> Documents { get; set; }
    }
}