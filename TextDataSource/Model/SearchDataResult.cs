using System.Collections.Generic;

namespace TextDataSource
{
    public class SearchDataResult
    {
        public string Query { get; set; }
        public List<TextDataDocument> TextDataDocuments { get; set; }
    }
}