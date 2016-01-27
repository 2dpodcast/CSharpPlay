using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSharpPlay.Engine.Scripting;

namespace CSharpPlay.Test.Scripting
{
	[TestClass]
	public class UnusableTest
	{
		private class AddedUnusable : Unusable
		{
			public void AddUnusableName(string name)
			{
				UnusableNames.Add(name, GetRegexFromName(name));
			}
		}

		private AddedUnusable _unusable;

		[TestInitialize]
		public void UnusableTestInitialize()
		{
			_unusable = new AddedUnusable();
		}

		[TestCleanup]
		public void UnusableTestCleanup()
		{ }

		[TestMethod]
		public void ExtractUnusableName()
		{
			Assert.AreEqual(nameof(CSharpPlay), _unusable.ExtractFirst(nameof(CSharpPlay)));
			Assert.AreEqual(nameof(CSharpPlay), _unusable.ExtractFirst($"return new {typeof(CSharpPlay.Engine.Manager).Namespace}.{nameof(CSharpPlay.Engine.Manager)}(null, null);"));
		}

		[TestMethod]
		public void ExtractUnusableNamespace()
		{
			Assert.AreEqual("System.Diagnostics", _unusable.ExtractFirst(@"return System.Diagnostics.Process.GetProcesses();"));
			Assert.AreEqual("System.IO", _unusable.ExtractFirst(@"return System.IO.File.Exists(null);"));
			Assert.AreEqual("System.IO", _unusable.ExtractFirst(@"
using System.IO;
class Program{public static void Main(){Console.WriteLine(File.Exists(null));}}
Program.Main();"));
			Assert.AreEqual("System.IO", _unusable.ExtractFirst(@"
var bytes=Encoding.UTF8.GetBytes(""Atom"");
using(var stream=new System.IO.MemoryStream(bytes))
{return Encoding.UTF8.GetString(stream.ToArray());}"));

			Assert.AreEqual("System.Net", _unusable.ExtractFirst(@"
var client=new System.Net.Http.HttpClient();
client.GetStringAsync(""http://www.google.com/"").Result;
client.Dispose();"));
			Assert.AreEqual("System.Net", _unusable.ExtractFirst(@"return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();"));
		}

		[TestMethod]
		public void ExtractUnusableMemberName()
		{
			Assert.AreEqual("Environment.OSVersion", _unusable.ExtractFirst($@".Environment. OSVersion"));
			Assert.AreEqual(null, _unusable.ExtractFirst($@" Environment.NewLine"));
		}

		[TestMethod]
		public void ExtractAddedUnusableName()
		{
			var name = "Company.Product";
			_unusable.AddUnusableName(name);

			Assert.AreEqual(name, _unusable.ExtractFirst(@"Company.Product"));
			Assert.AreEqual(name, _unusable.ExtractFirst(@"Company . Product"));
			Assert.AreEqual(name, _unusable.ExtractFirst(@"(Company   .Product"));
			Assert.AreEqual(name, _unusable.ExtractFirst(@" Company.	Product."));
			Assert.AreEqual(name, _unusable.ExtractFirst(@"..Company .
				Product"));
			Assert.AreEqual(name, _unusable.ExtractFirst(@"Company
.
Product"));

			Assert.AreEqual(null, _unusable.ExtractFirst("Company,Product"));
			Assert.AreEqual(null, _unusable.ExtractFirst("Company Product"));
			Assert.AreEqual(null, _unusable.ExtractFirst("cCompany.Product "));
			Assert.AreEqual(null, _unusable.ExtractFirst(" Company.Productt"));
		}

		[TestMethod]
		public void ExtractUnusableDirectives()
		{
			Assert.AreEqual("#r", _unusable.ExtractFirst(@"#r Microsoft.Build"));
			Assert.AreEqual("#r", _unusable.ExtractFirst(@" #r	Microsoft.Build"));
			Assert.AreEqual("#r", _unusable.ExtractFirst(@"
#r	Microsoft.Build"));
			Assert.AreEqual("#r", _unusable.ExtractFirst(@"// Comment
#r Microsoft.Build"));
			Assert.AreEqual("#r", _unusable.ExtractFirst(@"/* Comment */
#r Microsoft.Build"));
		}
	}
}