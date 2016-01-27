using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSharpPlay.Engine.Scripting;

namespace CSharpPlay.Test.Scripting
{
	[TestClass]
	public class ObjectExtensionTest
	{
		[TestMethod]
		public void DumpObject()
		{
			Assert.AreEqual("System.Object", GetOutput(() => new object().Dump()));
			Assert.AreEqual(string.Empty, GetOutput(() => ((object)null).Dump()));
		}

		[TestMethod]
		public void DumpNumeral()
		{
			Assert.AreEqual(@"100", GetOutput(() => (10 * 10).Dump()));




			Assert.AreSame(typeof(int), 100.Dump().GetType());
			Assert.AreEqual(@"100", GetOutput(() => (10D * 10D).Dump()));
			Assert.AreEqual(typeof(double), 100D.Dump().GetType());
		}

		[TestMethod]
		public void DumpString()
		{
			Assert.AreEqual(@"Dummy", GetOutput(() => @"Dummy".Dump()));
			Assert.AreEqual(typeof(string), @"Dummy".Dump().GetType());
			Assert.AreEqual(@"D
u
m
m
y", GetOutput(() => @"Dummy".ToCharArray().Dump()));
			Assert.AreEqual(typeof(char[]), @"Dummy".ToCharArray().Dump().GetType());
		}

		#region Collection

		[TestMethod]
		public void DumpArrayList()
		{
			Assert.AreEqual(@"True
True
False", GetOutput(() => new ArrayList(new[] { true, true, false }).Dump()));
		}

		[TestMethod]
		public void DumpNonGenericArray()
		{
			Assert.AreEqual(@"Sunday
Monday
Tuesday
Wednesday
Thursday
Friday
Saturday", GetOutput(() => typeof(DayOfWeek).GetEnumValues().Dump()));
		}

		[TestMethod]
		public void DumpGenericArray()
		{
			Assert.AreEqual(@"0
1
2
3
4", GetOutput(() => Enumerable.Range(0, 5).ToArray().Dump()));
		}

		[TestMethod]
		public void DumpList()
		{
			Assert.AreEqual(@"a
b
c", GetOutput(() => new List<char> { 'a', 'b', 'c' }.Dump()));
		}

		[TestMethod]
		public void DumpHashSet()
		{
			Assert.AreEqual(@"0
3
7", GetOutput(() => new HashSet<int> { 0, 3, 7 }.Dump()));
		}

		[TestMethod]
		public void DumpCharArray()
		{
			Assert.AreEqual(@"S
p
r
i
n
g", GetOutput(() => "Spring".ToCharArray().Dump()));
		}

		#endregion

		#region Format

		[TestMethod]
		public void DumpPrimitiveFormat()
		{
			Assert.AreEqual("10", GetOutput(() => ((short)16).Dump("X")));
			Assert.AreEqual("20", GetOutput(() => 32.Dump("X")));
			Assert.AreEqual("6,400%", GetOutput(() => 64L.Dump("P0")));
			Assert.AreEqual("128.2", GetOutput(() => 128.22D.Dump("F1")));
			Assert.AreEqual("0012.30", GetOutput(() => 12.3F.Dump("0000.00")));
		}

		[TestMethod]
		public void DumpDateTimeFormat()
		{
			var source = new DateTime(1999, 12, 31, 0, 0, 0);
			Assert.AreEqual("1999/12/31", GetOutput(() => source.Dump("yyyy/MM/dd")));
		}

		[TestMethod]
		public void DumpDateTimeOffsetFormat()
		{
			var source = new DateTimeOffset(2000, 2, 2, 14, 1, 10, TimeSpan.Zero);
			Assert.AreEqual("00/2/2 14:1:10", GetOutput(() => source.Dump("yy/M/d H:m:s")));
		}

		#endregion

		#region Base

		private string GetOutput<T>(Func<T> dump)
		{
			using (var ms = new MemoryStream())
			using (var sw = new StreamWriter(ms))
			{
				Console.SetOut(sw);

				dump();

				sw.Flush();

				var standard = new StreamWriter(Console.OpenStandardOutput());
				Console.SetOut(standard);

				return Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length).Trim();
			}
		}

		#endregion
	}
}