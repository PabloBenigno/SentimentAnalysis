using System.Collections.Generic;

namespace TextDataAnalysis
{
    public class EntitiesDocumentResult
    {
        public string Id { get; set; }
        public IEnumerable<EntityDocument> Entities { get; set; }
    }
}