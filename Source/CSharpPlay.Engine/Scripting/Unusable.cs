using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSharpPlay.Engine.Scripting
{
	public class Unusable : IUnusable
	{
		protected IDictionary<string, Regex> UnusableNames { get; }

		public Unusable()
		{
			var names = new IEnumerable<string>[]
				{
					new[] { nameof(CSharpPlay) },
					_unusableNamespaces,
					EnumerateMemberNames(typeof(System.Environment)),
					EnumerateMemberNames(typeof(System.AppDomain)),
					EnumerateMemberNames(typeof(System.Reflection.Assembly))
				}
				.SelectMany(x => x)
				.Except(_exceptedNames)
				.Select(x => new { Key = x, Value = GetRegexFromName(x, RegexOptions.Compiled) });

			var directives = new[] { "#r", "#load" }
				.Select(x => new { Key = x, Value = GetRegexFromDirective(x, RegexOptions.Compiled) });

			UnusableNames = names.Concat(directives).ToDictionary(x => x.Key, x => x.Value);
		}

		/// <summary>
		/// Extract an unusable name first found in source text.
		/// </summary>
		/// <param name="source">Source text</param>
		/// <returns>If found, the unusable name. If not, null.</returns>
		public virtual string ExtractFirst(string source)
		{
			return UnusableNames.FirstOrDefault(x => x.Value.IsMatch(source)).Key;
		}

		/// <summary>
		/// Extract all unusable names found in source text.
		/// </summary>
		/// <param name="source">Source text</param>
		/// <returns>If found, the unusable names. If not, empty sequence.</returns>
		public virtual IEnumerable<string> ExtractAll(string source)
		{
			return UnusableNames.Where(x => x.Value.IsMatch(source)).Select(x => x.Key);
		}

		private static readonly string[] _unusableNamespaces =
			{
				"System.Configuration",
				"System.Diagnostics",
				"System.IO",
				"System.Management",
				"System.Net",
				"System.Resources",
				"System.Runtime",
				"System.Security",
				"Microsoft.Win32",
				"Microsoft.CodeAnalysis"
			};

		private static readonly string[] _exceptedNames =
			{
				"Environment.NewLine",
			};

		#region Helper

		protected static IEnumerable<string> EnumerateMemberNames(Type targetType)
		{
			var flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

			var properties = targetType.GetProperties(flags);
			var methods = targetType.GetMethods(flags)
				.Where(x => !x.Name.StartsWith("get_", StringComparison.Ordinal) && !x.Name.StartsWith("set_", StringComparison.Ordinal)) // Property accessor
				.Where(x => !x.Name.StartsWith("add_", StringComparison.Ordinal) && !x.Name.StartsWith("remove_", StringComparison.Ordinal)); // Event accessor
			var events = targetType.GetEvents(flags);

			var members = new IEnumerable<MemberInfo>[] { properties, methods, events }.SelectMany(x => x);

			foreach (var memberName in members.Select(x => x.Name).Distinct())
				yield return $"{targetType.Name}.{memberName}";
		}

		protected static Regex GetRegexFromName(string name, RegexOptions options = default(RegexOptions))
		{
			return new Regex($@"\b{name.Trim('.').Replace(".", @"\s*\.\s*")}\b", options);
		}

		protected static Regex GetRegexFromDirective(string directive, RegexOptions options = default(RegexOptions))
		{
			return new Regex($@"(^|\s+){directive}\b", options);
		}

		#endregion
	}
}