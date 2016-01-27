using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpPlay.Engine.Scripting;

namespace CSharpPlay.Engine.Log
{
	internal class LogPack
	{
		public ulong TweetId { get; set; } // This must be unique by nature.
		public DateTimeOffset TweetTime { get; set; }
		public string ScriptContent { get; set; }
		public string ResultContent { get; set; }
		public ScriptState State { get; set; }

		public LogPack(
			ulong tweetId,
			DateTimeOffset tweetTime,
			string scriptContent,
			string resultContent,
			ScriptState state)
		{
			this.TweetId = tweetId;
			this.TweetTime = tweetTime;
			this.ScriptContent = scriptContent;
			this.ResultContent = resultContent;
			this.State = state;
		}
	}
}