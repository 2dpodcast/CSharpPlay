using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPlay.Engine.Log
{
	public interface IStorage
	{
		Task<string> LoadAsync();
		Task SaveAsync(string content);
		Task ClearAsync();
	}
}