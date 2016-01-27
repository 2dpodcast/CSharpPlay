using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPlay.Engine.Helper
{
	internal static class StringExtension
	{
		public static string TrimStartCombined(this string source, params char[] trimChars)
		{
			if (!(trimChars?.Any()).GetValueOrDefault())
				return source.TrimStart();

			for (int i = 0; i < source.Length; i++)
			{
				var sourceChar = source[i];
				if (!char.IsWhiteSpace(sourceChar) && !trimChars.Contains(sourceChar))
					return source.Substring(i);
			}
			return string.Empty;
		}

		public static string TrimEndCombined(this string source, params char[] trimChars)
		{
			if (!(trimChars?.Any()).GetValueOrDefault())
				return source.TrimEnd();

			for (int i = source.Length - 1; i >= 0; i--)
			{
				var sourceChar = source[i];
				if (!char.IsWhiteSpace(sourceChar) && !trimChars.Contains(sourceChar))
					return source.Substring(0, i + 1);
			}
			return string.Empty;
		}
	}
}