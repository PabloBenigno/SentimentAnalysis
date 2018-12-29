using System.Threading.Tasks;

namespace TextDataSource
{
    public interface ITextDataSource
    {
        Task<SearchDataResult> GetSearchDataAsync(string searchString);
        UserResult GetUserFromScreenName(string screenName);
        TimelineResult GetTimelineByUserId(long userUserId);
    }
}
