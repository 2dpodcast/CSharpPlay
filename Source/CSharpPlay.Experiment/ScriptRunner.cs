using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace CSharpPlay.Experiment
{
	internal class ScriptRunner
	{
		private class ScriptPack
		{
			public string Import { get; set; }
			public string Script { get; set; }
		}

		private ScriptPack[] _packs = new[]
		{
			new ScriptPack { Import = "CSharpPlay.Experiment.Extensions", Script = "(1+2).ExtensionMethodOne();" },
			new ScriptPack { Import = "CSharpPlay.Experiment.Extensions", Script = "ClassOne.ExtensionMethodOne(1+2);" },
			new ScriptPack { Import = "CSharpPlay.Experiment.Extensions", Script = "NonExtensionMethodOne(1+2);" },
			new ScriptPack { Import = "CSharpPlay.Experiment.Extensions", Script = "ClassOne.NonExtensionMethodOne(1+2);" },
			new ScriptPack { Import = "CSharpPlay.Experiment.Extensions", Script = "(3+4).ExtensionMethodTwo();" },
			new ScriptPack { Import = "CSharpPlay.Experiment.Extensions", Script = "ClassTwo.ExtensionMethodTwo(1+2);" },

			new ScriptPack { Import = "CSharpPlay.Experiment.Extensions.ClassOne", Script = "(1+2).ExtensionMethodOne();" },
			new ScriptPack { Import = "CSharpPlay.Experiment.Extensions.ClassOne", Script = "ClassOne.ExtensionMethodOne(1+2);" },
			new ScriptPack { Import = "CSharpPlay.Experiment.Extensions.ClassOne", Script = "NonExtensionMethodOne(1+2);" },
			new ScriptPack { Import = "CSharpPlay.Experiment.Extensions.ClassOne", Script = "ClassOne.NonExtensionMethodOne(1+2);" },
			new ScriptPack { Import = "CSharpPlay.Experiment.Extensions.ClassOne", Script = "(3+4).ExtensionMethodTwo();" },
			new ScriptPack { Import = "CSharpPlay.Experiment.Extensions.ClassOne", Script = "ClassTwo.ExtensionMethodTwo(1+2);" },
		};

		public void EvaluateScripts()
		{
			int index = 1;
			foreach (var pack in _packs)
			{
				Console.WriteLine($"<-- Sample {index++} -->");
				Console.WriteLine($"Import: {pack.Import}");
				Console.WriteLine($"Script: {pack.Script}");
				Console.Write("Result: ");
				EvaluateScriptAsync(pack.Import, pack.Script).Wait();
			}
			Console.Read();
		}

		public async Task EvaluateScriptAsync(string import, string script)
		{
			var options = ScriptOptions.Default
				.WithImports(
					"System",
					import)
				.WithReferences(
					typeof(object).Assembly,
					typeof(CSharpPlay.Experiment.Program).Assembly);

			try
			{
				await CSharpScript.EvaluateAsync(script, options);
			}
			catch (CompilationErrorException ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}