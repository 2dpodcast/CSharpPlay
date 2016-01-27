using System;

namespace CSharpPlay.Experiment.Extensions
{
	public static class ClassOne
	{
		public static void ExtensionMethodOne(this object source) { Console.WriteLine(source); }
		public static void NonExtensionMethodOne(object source) { Console.WriteLine(source); }
	}

	public static class ClassTwo
	{
		public static void ExtensionMethodTwo(this object source) { Console.WriteLine(source); }
	}
}