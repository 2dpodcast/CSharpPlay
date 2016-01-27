using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSharpPlay.Engine.Helper;

namespace CSharpPlay.Test.Helper
{
	[TestClass]
	public class StringExtensionTest
	{
		[TestMethod]
		public void PerformTrimStartCombined()
		{
			Assert.AreEqual("abc", @" 	abc".TrimStartCombined());
			Assert.AreEqual("abc", @" 	abc".TrimStartCombined(null));
			Assert.AreEqual("abc", @" 	,abc".TrimStartCombined(','));
			Assert.AreEqual("abc", @" 	, , abc".TrimStartCombined(','));
			Assert.AreEqual("abc", @" 	,.	.
, .abc".TrimStartCombined(',', '.'));
			Assert.AreEqual(string.Empty, @" 	".TrimStartCombined());
			Assert.AreEqual(string.Empty, @" 	".TrimStartCombined(null));
			Assert.AreEqual(string.Empty, @" 	,".TrimStartCombined(','));
			Assert.AreEqual(string.Empty, @" 	, , ".TrimStartCombined(','));
			Assert.AreEqual(string.Empty, @" 	,.	.
, .".TrimStartCombined(',', '.'));
		}

		[TestMethod]
		public void PerformTrimEndCombined()
		{
			Assert.AreEqual("xyz", @"xyz 	".TrimEndCombined());
			Assert.AreEqual("xyz", @"xyz 	".TrimEndCombined(null));
			Assert.AreEqual("xyz", @"xyz 	,".TrimEndCombined(','));
			Assert.AreEqual("xyz", @"xyz 	, , ".TrimEndCombined(','));
			Assert.AreEqual("xyz", @"xyz 	,.	.
, .".TrimEndCombined(',', '.'));
			Assert.AreEqual(string.Empty, @" 	".TrimEndCombined());
			Assert.AreEqual(string.Empty, @" 	".TrimEndCombined(null));
			Assert.AreEqual(string.Empty, @" 	,".TrimEndCombined(','));
			Assert.AreEqual(string.Empty, @" 	, , ".TrimEndCombined(','));
			Assert.AreEqual(string.Empty, @" 	,.	.
, .".TrimEndCombined(',', '.'));
		}
	}
}