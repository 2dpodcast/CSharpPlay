using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPlay.Engine.Scripting
{
	public interface IUnusable
	{
		string ExtractFirst(string source);
		IEnumerable<string> ExtractAll(string source);
	}
}