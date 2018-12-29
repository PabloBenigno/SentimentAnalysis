using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using Tweetinvi;
using Tweetinvi.Models;

namespace TextDataSource
{
    public class TwitterDataSource : ITextDataSource
    {
        private readonly TwitterContext _context;
        public TwitterDataSource(TwitterConfiguration configuration)
        {
            var auth = new ApplicationOnlyAuthorizer
            {
                CredentialStore = new InMemoryCredentialStore()
                {
                    ConsumerKey = configuration.ConsumerKey,
                    ConsumerSecret = configuration.ConsumerSecret
                }
            };

            auth.AuthorizeAsync().Wait();

            _context = new TwitterContext(auth);

            Auth.SetUserCredentials(configuration.ConsumerKey, configuration.ConsumerSecret, configuration.AccessToken, configuration.AccessTokenSecret);

        }
        public async Task<SearchDataResult> GetSearchDataAsync(string searchString)
        {
            var searchResult =
                await (from search in _context.Search
                    where search.Type == SearchType.Search &&
                          search.Query == searchString
                 select search)
                .SingleOrDefaultAsync();

            return new SearchDataResult
            {
                Query = searchResult.Query,
                TextDataDocuments = searchResult.Statuses.Select(_ => new TextDataDocument
                {
                    Id = _.StatusID.ToString(),
                    Source = _.Source,
                    Text = _.Text
                }).ToList()
            };
        }

        public UserResult GetUserFromScreenName(string screenName)
        {
            var user = Tweetinvi.User.GetUserFromScreenName(screenName);
            return new UserResult
            {
                Name = user.Name,
                UserId = user.UserIdentifier.Id
            };
        }

        public TimelineResult GetTimelineByUserId(long userId)
        {
            UserIdentifier userIdentifier = new UserIdentifier(userId);
            var timeline = Timeline.GetUserTimeline(userIdentifier);
            return new TimelineResult
            {

            };
        }
    }
}