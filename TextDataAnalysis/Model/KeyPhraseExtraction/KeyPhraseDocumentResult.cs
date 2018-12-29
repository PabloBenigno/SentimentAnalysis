using System.Collections.Generic;

namespace TextDataAnalysis
{
    public class KeyPhraseDocumentResult
    {
        public string Id { get; set; }
        public IEnumerable<string> KeyPhrases { get; set; }
    }
}