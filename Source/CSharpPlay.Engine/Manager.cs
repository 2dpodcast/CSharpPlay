using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpPlay.Engine.Helper;
using CSharpPlay.Engine.Log;
using CSharpPlay.Engine.Scripting;
using CSharpPlay.Engine.Twitter;

namespace CSharpPlay.Engine
{
	public class Manager
	{
		#region Constructor

		private readonly TweetWorker _tweetWorker;
		private readonly LogWorker _logWorker;
		private readonly ScriptRunner _scriptRunner;

		public Manager(IAccount account, IStorage storage, IUnusable unusable = null)
		{
			if (account == null)
				throw new ArgumentNullException(nameof(account));
			if (storage == null)
				throw new ArgumentNullException(nameof(storage));

			this._tweetWorker = new TweetWorker(account);
			this._logWorker = new LogWorker(storage, 200);
			this._scriptRunner = new ScriptRunner(unusable ?? new Unusable());
		}

		#endregion

		#region Parameters

		/// <summary>
		/// Window duration for mentions
		/// </summary>
		public TimeSpan MentionsWindowDuration
		{
			get { return _mentionsWindowDuration; }
			set
			{
				if (value <= TimeSpan.Zero)
					throw new ArgumentOutOfRangeException(nameof(MentionsWindowDuration), "The duration must be longer than 0.");

				_mentionsWindowDuration = value;
			}
		}
		private TimeSpan _mentionsWindowDuration = TimeSpan.FromMinutes(10);

		/// <summary>
		/// Timeout duration for scripting
		/// </summary>
		public TimeSpan ScriptTimeoutDuration
		{
			get { return _scriptTimeoutDuration; }
			set
			{
				if (value <= TimeSpan.Zero)
					throw new ArgumentOutOfRangeException(nameof(ScriptTimeoutDuration), "The duration must be longer than 0.");

				_scriptTimeoutDuration = value;
			}
		}
		private TimeSpan _scriptTimeoutDuration = TimeSpan.FromSeconds(6);

		/// <summary>
		/// Timeout duration for entire operation
		/// </summary>
		public TimeSpan OperationTimeoutDuration
		{
			get { return _operationTimeoutDuration; }
			set
			{
				if (value <= TimeSpan.Zero)
					throw new ArgumentOutOfRangeException(nameof(OperationTimeoutDuration), "The duration must be longer than 0.");

				_operationTimeoutDuration = value;
			}
		}
		private TimeSpan _operationTimeoutDuration = TimeSpan.FromMinutes(1);

		#endregion

		public async Task RunAsync()
		{
			var timeoutTime = DateTime.Now.Add(OperationTimeoutDuration);

			var mentions = await RetrieveMentionsAsync(MentionsWindowDuration);

			Debug.WriteLine($"Mentions: {mentions?.Length}");

			if (!(mentions?.Any()).GetValueOrDefault())
				return;

			var oldTweetIds = await _logWorker.ReadOldTweetIdsAsync();

			var newLogs = new List<LogPack>();

			foreach (var mention in mentions
				.Where(x => !oldTweetIds.Contains(x.TweetId) && x.IsReplyable)
				.OrderBy(x => x.TweetTime))
			{
				if (timeoutTime < DateTime.Now)
					break;

				Debug.WriteLine($"Mention: {mention.TweetId}, {mention.MentionerScreenName}, {mention.Content}");

				var result = await _scriptRunner.EvaluateAsync(mention.Content, ScriptTimeoutDuration);
				await ReplyResultAsync(mention, result);

				Debug.WriteLine($"Reply: {result.Output}");

				newLogs.Add(new LogPack(
					tweetId: mention.TweetId,
					tweetTime: DateTimeOffset.Now,
					scriptContent: mention.Content,
					resultContent: result.Output,
					state: result.State));
			}

			await _logWorker.AppendNewLogsAsync(newLogs);
		}

		public async Task TweetAsync(string content)
		{
			try
			{
				await _tweetWorker.TweetAsync(content);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		private async Task<MentionPack[]> RetrieveMentionsAsync(TimeSpan windowDuration)
		{
			try
			{
				return await _tweetWorker.RetrieveMentionsAsync(windowDuration);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return null;
			}
		}

		private async Task ReplyResultAsync(MentionPack mention, ScriptResult result)
		{
			if (string.IsNullOrWhiteSpace(result?.Output))
				return;

			var content = result.State.ToDisplayString($"({result.Elapsed.TotalSeconds:f3})")
				+ Environment.NewLine
				+ result.Output;

			try
			{
				await _tweetWorker.ReplyAsync(mention.TweetId, mention.MentionerScreenName, content);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}
	}
}