using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CSharpPlay.Engine.Scripting;

namespace CSharpPlay.Test.Scripting
{
	[TestClass]
	public class EvaluateBaseAsyncTest
	{
		private static ScriptRunner _scriptRunner;

		[ClassInitialize]
		public static void MyClassInitialize(TestContext testContext)
		{
			_scriptRunner = new ScriptRunner(new Unusable());
		}

		[ClassCleanup]
		public static void MyClassCleanup()
		{ }

		[TestMethod]
		public async Task CheckLastSemicolon1()
		{
			// If no semicolon at the end, it will be evaluated as q1expression and the result will be returned.
			Assert.AreEqual("3", await EvaluateBaseAsync("1+2"));
			Assert.AreEqual("3", await EvaluateBaseAsync("var a=1; var b=2; a+b"));
			Assert.AreEqual("3", await EvaluateBaseAsync("var a=1; var b=2; a+=b"));
		}

		[TestMethod]
		[ExpectedException(typeof(CompilationErrorException))]
		public async Task CheckLastSemicolon2()
		{
			// If no semicolon at the end, it will be evaluated as expression.
			Assert.AreEqual("3", await EvaluateBaseAsync("var c=1+2"));
		}

		[TestMethod]
		[ExpectedException(typeof(CompilationErrorException))]
		public async Task CheckLastSemicolon3()
		{
			// If no semicolon at the end, it will be evaluated as expression.		
			Assert.AreEqual("3", await EvaluateBaseAsync("var a=1; var b=2; var c=a+b"));
		}

		[TestMethod]
		[ExpectedException(typeof(CompilationErrorException))]
		public async Task CheckLastSemicolon4()
		{
			// If a semicolon at the end, it will be evaluated as statement.
			Assert.AreEqual("", await EvaluateBaseAsync("1+2;"));
		}

		[TestMethod]
		[ExpectedException(typeof(CompilationErrorException))]
		public async Task CheckLastSemicolon5()
		{
			// If a semicolon at the end, it will be evaluated as statement.
			Assert.AreEqual("", await EvaluateBaseAsync("var a=1; var b=2; a+b;"));
		}

		[TestMethod]
		public async Task CheckEmpty()
		{
			Assert.AreEqual("", await EvaluateBaseAsync(string.Empty));
			Assert.AreEqual("", await EvaluateBaseAsync(";"));
			Assert.AreEqual("", await EvaluateBaseAsync(" ;  ; ; "));
		}

		[TestMethod]
		public async Task UsePrimitives()
		{
			Assert.AreEqual("5", await EvaluateBaseAsync(@"return 2+3;"));
			Assert.AreEqual("2", await EvaluateBaseAsync(@"return 5-3;"));
			Assert.AreEqual("-6", await EvaluateBaseAsync(@"return -3*2;"));
			Assert.AreEqual("1.5", await EvaluateBaseAsync(@"return 3D/2D;"));
			Assert.AreEqual("3", await EvaluateBaseAsync(@"int x=1; int y=2; return x+y;"));
			Assert.AreEqual("2" + Environment.NewLine + "15", await EvaluateBaseAsync(@"var a=5; var b=3; (a-b).Dump(); (a*b).Dump();"));
			Assert.AreEqual("10", await EvaluateBaseAsync(@"var a=4; var b=6; var c=a+b; Console.WriteLine(c);"));
			Assert.AreEqual("9" + Environment.NewLine + "2", await EvaluateBaseAsync(@"var name = ""piyo piyo""; name.Length.Dump(); name.Split().Length.Dump();"));
			Assert.AreEqual("rare", await EvaluateBaseAsync(@"var phrase=""Cats rarely run but sleep.""; return phrase.Substring(5,4);"));
			Assert.AreEqual("明鏡", await EvaluateBaseAsync(@"var idiom=""明鏡止水""; return new string(idiom.ToCharArray().Take(2).ToArray());"));
		}

		[TestMethod]
		public async Task UseSystemCollections()
		{
			Assert.AreEqual("3" + Environment.NewLine + "2", await EvaluateBaseAsync(@"int[] numbers={4,5,-7}; numbers.Length.Dump(); return numbers.Sum();"));
			Assert.AreEqual("8" + Environment.NewLine + "3", await EvaluateBaseAsync(@"var list=new List<int>{3,4,2,8,9}; list[3].Dump(); return list.Buffer(2).Count();"));
		}

		[TestMethod]
		public async Task UseSystemGlobalization()
		{
			Assert.AreEqual("1041", await EvaluateBaseAsync(@"var culture = new CultureInfo(""ja-jp""); return culture.LCID;"));
		}

		[TestMethod]
		public async Task UseSystemMath()
		{
			Assert.AreEqual("65536", await EvaluateBaseAsync("return Math.Pow(2,16);"));
			Assert.AreEqual("-12" + Environment.NewLine + "11", await EvaluateBaseAsync("var x=11; var y=-12; Math.Min(x,y).Dump(); Math.Max(x,y).Dump();"));
		}

		[TestMethod]
		public async Task UseSystemText()
		{
			Assert.AreEqual("World", await EvaluateBaseAsync(@"byte[] data={(byte)87,(byte)111,(byte)114,(byte)108,(byte)100}; return Encoding.UTF8.GetString(data);"));
			Assert.AreEqual("True", await EvaluateBaseAsync(@"var regex=new Regex(""Sat""); regex.IsMatch(""Saturn"").Dump();"));
		}

		[TestMethod]
		public async Task UseIx()
		{
			Assert.AreEqual("5", await EvaluateBaseAsync(@"Enumerable.Range(0,10).Buffer(2).Count()"));
		}

		[TestMethod]
		public async Task UseSystemReflection1()
		{
			Assert.AreEqual("True", await EvaluateBaseAsync(@"var type=typeof(int); return type.IsPrimitive;"));
		}

		[TestMethod]
		[ExpectedException(typeof(CompilationErrorException))]
		public async Task UseSystemReflection2()
		{
			Assert.AreEqual("", await EvaluateBaseAsync("return Assembly.GetExecutingAssembly().Location;"));
		}

		[TestMethod]
		[ExpectedException(typeof(CompilationErrorException))]
		public async Task UseSystemDiagnostics()
		{
			Assert.AreEqual("", await EvaluateBaseAsync("return Process.GetProcesses();"));
		}

		[TestMethod]
		public async Task UseOwnAssembly1()
		{
			Assert.AreEqual("Test", await EvaluateBaseAsync(@"""Test"".Dump();"));
		}

		[TestMethod]
		[ExpectedException(typeof(CompilationErrorException))]
		public async Task UseOwnAssembly2()
		{
			Assert.AreEqual("10", await EvaluateBaseAsync(@"ObjectExtension.Dump(10)"));
		}

		[TestMethod]
		[ExpectedException(typeof(CompilationErrorException))]
		public async Task UseOwnAssembly3()
		{
			Assert.AreEqual("", await EvaluateBaseAsync(@"class AddedUnusable : IUnusable {}"));
		}

		#region Base

		private async Task<string> EvaluateBaseAsync(string script)
		{
			using (var ms = new MemoryStream())
			using (var sw = new StreamWriter(ms))
			{
				Console.SetOut(sw);

				var returned = await _scriptRunner.EvaluateBaseAsync(script, TimeSpan.FromSeconds(5));
				if (returned != null)
					Console.WriteLine(returned);

				sw.Flush();

				return Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length).Trim();
			}
		}

		#endregion
	}
}