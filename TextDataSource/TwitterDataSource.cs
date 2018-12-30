using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using Search = Tweetinvi.Search;
using User = Tweetinvi.User;

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
            var user = User.GetUserFromScreenName(screenName);
            return new UserResult
            {
                Name = user.Name,
                UserId = user.UserIdentifier.Id
            };
        }

        public TimelineResult GetTimelineByUserName(string userName)
        {
            UserIdentifier userIdentifier = new UserIdentifier(userName);
            var timeline = Timeline.GetUserTimeline(userIdentifier);
            var result = new TimelineResult{Tweets = new List<TweetResult>()};
            
            foreach (var tweet in timeline)
            {
                var replies = Search.SearchRepliesTo(tweet, false);

                result.Tweets.Add(new TweetResult
                {
                    TweetId = tweet.Id,
                    Text = tweet.Text,
                    Replies = replies.Select(r => new TweetResult
                    {
                        TweetId = r.Id,
                        Text = r.Text
                        //No further replies will be stored
                    }).ToList()
                });
            }

            return result;
        }
    }
}