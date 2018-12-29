using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

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
        }
        public async Task<SearchDataResult> GetSearchData(string searchString)
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
    }
}