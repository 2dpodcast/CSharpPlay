using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace CSharpPlay.Engine.Scripting
{
	internal class ScriptRunner
	{
		#region Constructor

		private readonly IUnusable _unusable;

		public ScriptRunner(IUnusable unusable)
		{
			if (unusable == null)
				throw new ArgumentNullException(nameof(unusable));

			this._unusable = unusable;
		}

		#endregion

		public async Task<ScriptResult> EvaluateAsync(string script, TimeSpan timeoutDuration)
		{
			if (string.IsNullOrWhiteSpace(script))
				return null;

			var unusableName = _unusable.ExtractFirst(script);
			if (unusableName != null)
			{
				return new ScriptResult(
					state: ScriptState.Unusable,
					output: $"{unusableName} is unusable.");
			}

			using (var ms = new MemoryStream())
			using (var sw = new StreamWriter(ms))
			{
				Console.SetOut(sw);
				var watch = Stopwatch.StartNew();

				var state = ScriptState.Failure;

				try
				{
					var returned = await EvaluateBaseAsync(script, timeoutDuration);
					if (returned != null)
						returned.Dump();

					state = ScriptState.Success;
				}
				catch (OperationCanceledException ex)
				{
					Console.WriteLine(ex.Message);
					state = ScriptState.Timeout;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}

				watch.Stop();
				await sw.FlushAsync();

				var output = Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length).Trim();
				if (string.IsNullOrEmpty(output))
				{
					state = ScriptState.NoOutput;
					output = "Completed but no output.";
				}

				return new ScriptResult(
					state: state,
					output: output,
					elapsed: watch.Elapsed);
			}
		}

		private static readonly ScriptOptions _options = ScriptOptions.Default
			.WithImports(
				"System",
				"System.Collections",
				"System.Collections.Generic",
				"System.Globalization",
				"System.Linq",
				"System.Math",
				"System.Numerics",
				"System.Text",
				"System.Text.RegularExpressions",
				"System.Xml",
				"System.Xml.Linq",
				"Newtonsoft.Json",
				"Newtonsoft.Json.Linq",
				"CSharpPlay.Engine.Scripting.ObjectExtension")
			.WithReferences(
				typeof(object).Assembly,
				typeof(System.Linq.EnumerableEx).Assembly, // Ix
				typeof(Newtonsoft.Json.JsonConvert).Assembly,
				typeof(CSharpPlay.Engine.Scripting.ObjectExtension).Assembly);

		public async Task<object> EvaluateBaseAsync(string script, TimeSpan timeoutDuration)
		{
			var cts = new CancellationTokenSource(timeoutDuration);
			var tcs = new TaskCompletionSource<bool>();
			cts.Token.Register(() => tcs.TrySetCanceled());

			try
			{
				// Dispatch another thread to create a task because CSharpScript.EvaluateAsync will block
				// its running thread in the case of heavy work including endless loop.
				// Invoking in separated class is to limit the scope of code to be evaluated.
				var scriptTask = Task.Run(async () => await new ScriptSeparatedRunner().EvaluateAsync(script, _options, cts.Token), cts.Token);

				await Task.WhenAny(tcs.Task, scriptTask);

				// Check if the task has been canceled. There are two cases:
				// If canceled in tcs.Task, scriptTask.IsCompleted -> false, scriptTask.IsCanceled -> false.
				// If canceled in scriptTask, scriptTask.IsCompleted -> true, scriptTask.IsCanceled -> true.
				if (!scriptTask.IsCompleted || scriptTask.IsCanceled)
					throw new OperationCanceledException($"Canceled by timeout.");

				return scriptTask.Result;
			}
			catch (AggregateException ex) when (ex.InnerException != null)
			{
				ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
				return null; // This will never be reached.
			}
		}
	}

	internal class ScriptSeparatedRunner
	{
		public async Task<object> EvaluateAsync(string script, ScriptOptions options, CancellationToken token)
		{
			return await CSharpScript.EvaluateAsync(script, options, null, null, token);
		}
	}
}