using System.Threading.Tasks;

namespace TextDataSource
{
    public interface ITextDataSource
    {
        Task<SearchDataResult> GetSearchData(string searchString);
    }
}
