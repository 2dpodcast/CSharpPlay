using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPlay.Engine.Twitter
{
	internal class MentionPack : TweetPack
	{
		public string MentionerScreenName => ScreenName;
		public string MentioneeScreenName { get; }

		public bool IsReplyable => (0 < TweetId) && !string.IsNullOrWhiteSpace(MentionerScreenName);

		public MentionPack(
			ulong tweetId,
			DateTimeOffset tweetTime,
			string content,
			string mentionerScreenName,
			string mentioneeScreenName)
			: base(
				  tweetId: tweetId,
				  tweetTime: tweetTime,
				  content: content,
				  screenName: mentionerScreenName)
		{
			this.MentioneeScreenName = mentioneeScreenName;
		}
	}
}