using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPlay.Engine.Scripting
{
	internal class ScriptResult
	{
		public ScriptState State { get; }
		public string Output { get; }
		public TimeSpan Elapsed { get; }

		public ScriptResult(
			ScriptState state,
			string output,
			TimeSpan elapsed = default(TimeSpan))
		{
			this.State = state;
			this.Output = output;
			this.Elapsed = elapsed;
		}
	}
}