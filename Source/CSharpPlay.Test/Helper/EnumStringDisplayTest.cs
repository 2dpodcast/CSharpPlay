using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSharpPlay.Engine.Helper;

namespace CSharpPlay.Test.Helper
{
	[TestClass]
	public class EnumStringDisplayTest
	{
		private enum TestEnum
		{
			None,

			[EnumDisplayString("Inner sphere")]
			Inner,

			[EnumDisplayString("Outer sphere", Symbol = ":-) ")]
			Outer,

			[EnumDisplayString("Boundary surface", Symbol = "m(_ _)m ", AppendInfo = true)]
			Surface
		}

		[TestMethod]
		public void PerformToDisplayString()
		{
			Assert.AreEqual("None", TestEnum.None.ToDisplayString("[INFO]"));
			Assert.AreEqual("Inner sphere", TestEnum.Inner.ToDisplayString("[INFO]"));
			Assert.AreEqual(":-) Outer sphere", TestEnum.Outer.ToDisplayString("[INFO]"));
			Assert.AreEqual("m(_ _)m Boundary surface[INFO]", TestEnum.Surface.ToDisplayString("[INFO]"));
		}
	}
}