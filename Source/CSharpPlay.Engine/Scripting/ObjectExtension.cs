using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPlay.Engine.Scripting
{
	public static class ObjectExtension
	{
		//public static object Dump(this object source) { Console.WriteLine(source); return source; }

		public static T Dump<T>(this T source)
		{
			var text = source as string; // To prioritize string over IEnumerable<char>
			if (text != null)
			{
				Console.WriteLine(text);
			}
			else
			{
				var sequence = source as IEnumerable;
				if (sequence != null)
				{
					foreach (object element in sequence) { Console.WriteLine(element); }
				}
				else
				{
					Console.WriteLine(source);
				}
			}
			return source;
		}

		#region Format

		public static IConvertible Dump(this IConvertible source, IFormatProvider provider) { Console.WriteLine(source.ToString(provider)); return source; }
		public static IFormattable Dump(this IFormattable source, string format, IFormatProvider provider) { Console.WriteLine(source.ToString(format, provider)); return source; }

		public static short Dump(this short source, string format) { Console.WriteLine(source.ToString(format)); return source; }
		public static int Dump(this int source, string format) { Console.WriteLine(source.ToString(format)); return source; }
		public static long Dump(this long source, string format) { Console.WriteLine(source.ToString(format)); return source; }
		public static byte Dump(this byte source, string format) { Console.WriteLine(source.ToString(format)); return source; }
		public static ushort Dump(this ushort source, string format) { Console.WriteLine(source.ToString(format)); return source; }
		public static uint Dump(this uint source, string format) { Console.WriteLine(source.ToString(format)); return source; }
		public static ulong Dump(this ulong source, string format) { Console.WriteLine(source.ToString(format)); return source; }
		public static double Dump(this double source, string format) { Console.WriteLine(source.ToString(format)); return source; }
		public static float Dump(this float source, string format) { Console.WriteLine(source.ToString(format)); return source; }
		public static decimal Dump(this decimal source, string format) { Console.WriteLine(source.ToString(format)); return source; }
		public static DateTime Dump(this DateTime source, string format) { Console.WriteLine(source.ToString(format)); return source; }
		public static DateTimeOffset Dump(this DateTimeOffset source, string format) { Console.WriteLine(source.ToString(format)); return source; }

		#endregion
	}
}