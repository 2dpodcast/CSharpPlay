using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPlay.Engine.Twitter
{
	internal class TweetPack
	{
		public ulong TweetId { get; }
		public DateTimeOffset TweetTime { get; }
		public string Content { get; }
		public string ScreenName { get; }

		public TweetPack(
			ulong tweetId,
			DateTimeOffset tweetTime,
			string content,
			string screenName)
		{
			this.TweetId = tweetId;
			this.TweetTime = tweetTime;
			this.Content = content;
			this.ScreenName = screenName;
		}
	}
}