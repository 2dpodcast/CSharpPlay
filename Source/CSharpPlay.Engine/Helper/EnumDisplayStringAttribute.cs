using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPlay.Engine.Helper
{
	[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Field, AllowMultiple = false)]
	internal class EnumDisplayStringAttribute : Attribute
	{
		public string Name;
		public string Symbol;
		public bool AppendInfo = false;

		public EnumDisplayStringAttribute(string name)
		{
			this.Name = name;
		}
	}

	internal static class EnumDisplayStringExtension
	{
		public static string ToDisplayString(this Enum value, string info = null)
		{
			var enumType = value.GetType();
			var enumName = Enum.GetName(enumType, value);

			var attribute = enumType.GetFields()
				.Single(x => x.Name == enumName)
				.GetCustomAttributes(typeof(EnumDisplayStringAttribute), false)
				.FirstOrDefault() as EnumDisplayStringAttribute;

			if (attribute == null)
				return value.ToString();

			var sb = new StringBuilder(!string.IsNullOrWhiteSpace(attribute.Name) ? attribute.Name : enumName);
			if (!string.IsNullOrWhiteSpace(attribute.Symbol)) sb.Insert(0, attribute.Symbol);
			if (attribute.AppendInfo && !string.IsNullOrWhiteSpace(info)) sb.Append(info);
			return sb.ToString();
		}
	}
}