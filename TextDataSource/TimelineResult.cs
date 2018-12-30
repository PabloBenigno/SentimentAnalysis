using System.Collections.Generic;
using LinqToTwitter;

namespace TextDataSource
{
    public class TimelineResult
    {
        public List<TweetResult> Tweets { get; set; }
    }

    public class TweetResult
    {
        public long TweetId { get; set; }
        public string Text { get; set; }
        public List<TweetResult> Replies { get; set; }
    }
}