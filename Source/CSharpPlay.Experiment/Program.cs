using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using CSharpPlay.Experiment.Extensions;
using static CSharpPlay.Experiment.Extensions.ClassOne;

namespace CSharpPlay.Experiment
{
	class Program
	{
		static void Main(string[] args)
		{
			//Run();
			new ScriptRunner().EvaluateScripts();
		}

		static void Run()
		{
			//ExtensionOne.DumpOne(1 + 2);
			//ExtensionOne.DumpOneNonExtension(1 + 2);
			//ExtensionTwo.DumpTwo(3 + 4);

			(1 + 2).ExtensionMethodOne();
			NonExtensionMethodOne(1 + 2);
		}
	}
}