namespace TextDataAnalysis
{
    public class DetectLanguageDocumentResult
    {
        public string Id { get; set; }
        public LanguageResult[] DetectedLanguages { get; set; }
    }
}