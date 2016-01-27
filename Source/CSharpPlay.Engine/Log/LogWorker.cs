using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CSharpPlay.Engine.Log
{
	internal class LogWorker
	{
		#region Constructor

		private readonly IStorage _logStorage;
		private readonly int _logMax;

		public LogWorker(IStorage logStorage, int logMax)
		{
			if (logStorage == null)
				throw new ArgumentNullException(nameof(logStorage));
			if (logMax <= 0)
				throw new ArgumentOutOfRangeException(nameof(logMax), "Max count must be greater than 0.");

			this._logStorage = logStorage;
			this._logMax = logMax;
		}

		#endregion

		#region Storage

		private LogPack[] _logs;

		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

		public async Task<LogPack[]> GetLogsAsync()
		{
			try
			{
				await _semaphore.WaitAsync();

				if (_logs == null)
				{
					_logs = await Task.Run(async () =>
					{
						var buff = await _logStorage.LoadAsync();
						if (string.IsNullOrWhiteSpace(buff))
							return Array.Empty<LogPack>(); // Array

						return JsonConvert.DeserializeObject<LogPack[]>(buff);
					});
				}
				return _logs;
			}
			finally
			{
				_semaphore.Release();
			}
		}

		public async Task SetLogsAsync(LogPack[] value)
		{
			try
			{
				await _semaphore.WaitAsync();

				_logs = value;

				await Task.Run(async () =>
				{
					var buff = JsonConvert.SerializeObject(_logs);

					await _logStorage.SaveAsync(buff);
				});
			}
			finally
			{
				_semaphore.Release();
			}
		}

		#endregion

		public async Task<ulong[]> ReadOldTweetIdsAsync()
		{
			return (await GetLogsAsync()).Select(x => x.TweetId).ToArray();
		}

		public async Task AppendNewLogsAsync(IEnumerable<LogPack> source)
		{
			var oldLogs = await GetLogsAsync();
			var newLogs = source.OrderBy(x => x.TweetTime).ToArray();

			var gap = Math.Max((oldLogs.Length + newLogs.Length - _logMax), 0);

			await SetLogsAsync(oldLogs.Concat(newLogs).Skip(gap).ToArray());
		}
	}
}