using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LinqToTwitter;

using CSharpPlay.Engine.Helper;

namespace CSharpPlay.Engine.Twitter
{
	internal class TweetWorker
	{
		#region Constructor

		private readonly IAuthorizer _selfAuthorizer;
		private readonly string _selfScreenName;

		public TweetWorker(IAccount account)
		{
			if (account == null)
				throw new ArgumentNullException(nameof(account));

			_selfAuthorizer = new SingleUserAuthorizer
			{
				CredentialStore = new SingleUserInMemoryCredentialStore
				{
					ConsumerKey = account.ConsumerKey,
					ConsumerSecret = account.ConsumerSecret,
					AccessToken = account.AccessToken,
					AccessTokenSecret = account.AccessTokenSecret
				}
			};

			_selfScreenName = account.SelfScreenName;
		}

		#endregion

		public async Task<bool> TweetAsync(string content)
		{
			var tweetContent = CheckContent(content);

			try
			{
				using (var context = new TwitterContext(_selfAuthorizer))
				{
					await context.TweetAsync(content);
					return true;
				}
			}
			catch (TwitterQueryException ex) when (ex.StatusCode == HttpStatusCode.Forbidden) // In case of duplicate
			{
				return false;
			}
		}

		public async Task<bool> ReplyAsync(TweetPack pack)
		{
			return await ReplyAsync(pack.TweetId, pack.Content, pack.ScreenName);
		}

		public async Task<bool> ReplyAsync(ulong tweetId, string screenName, string content)
		{
			var tweetContent = CheckContent($"@{screenName} {content}");

			try
			{
				using (var context = new TwitterContext(_selfAuthorizer))
				{
					await context.ReplyAsync(tweetId, tweetContent);
					return true;
				}
			}
			catch (TwitterQueryException ex) when (ex.StatusCode == HttpStatusCode.Forbidden) // In case of duplicate
			{
				return false;
			}
		}

		public async Task<MentionPack[]> RetrieveMentionsAsync(TimeSpan windowDuration)
		{
			var mentioneeScreenName = _selfScreenName;
			var mentioneeScreenNameWithAt = $"@{_selfScreenName}";

			using (var context = new TwitterContext(_selfAuthorizer))
			{
				var packs = new List<MentionPack>();

				var mentions = await
					(from tweet in context.Status
					 where (tweet.Type == StatusType.Mentions)
						&& (tweet.ScreenName == mentioneeScreenName)
						&& ((tweet.User != null) && (tweet.User.ScreenNameResponse != mentioneeScreenName)) // Mentioner
						&& (new DateTimeOffset(tweet.CreatedAt) >= DateTimeOffset.Now.Add(-windowDuration))
					 select tweet)
					 .ToListAsync();

				if (mentions != null)
				{
					foreach (var mention in mentions)
					{
						var mentionContent = WebUtility.HtmlDecode(mention.Text)?.TrimStartCombined('.');
						if (string.IsNullOrEmpty(mentionContent) ||
							!mentionContent.StartsWith(mentioneeScreenNameWithAt, StringComparison.Ordinal))
							continue;

						mentionContent = mentionContent.Substring(mentioneeScreenNameWithAt.Length).Trim();

						packs.Add(new MentionPack(
							tweetId: mention.StatusID,
							tweetTime: new DateTimeOffset(mention.CreatedAt),
							content: mentionContent,
							mentionerScreenName: mention.User.ScreenNameResponse,
							mentioneeScreenName: mentioneeScreenName));
					}
				}
				return packs.ToArray();
			}
		}

		#region Helper

		private static string CheckContent(string content)
		{
			var buff = content?.Trim();
			if (string.IsNullOrEmpty(buff))
				return string.Empty;

			buff = buff.Normalize(NormalizationForm.FormC); // NFC

			return (buff.Length <= 140)
				? buff
				: buff.Substring(0, 136) + " ...";
		}

		#endregion
	}
}