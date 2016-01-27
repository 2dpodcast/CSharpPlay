using CSharpPlay.Engine.Helper;

namespace CSharpPlay.Engine.Scripting
{
	internal enum ScriptState
	{
		None = 0,

		[EnumDisplayString("Success", Symbol = "🎵", AppendInfo = true)]
		Success,

		[EnumDisplayString("NoOutput", Symbol = "💬", AppendInfo = true)]
		NoOutput,

		[EnumDisplayString("Failure", Symbol = "💀", AppendInfo = true)]
		Failure,

		[EnumDisplayString("Timeout", Symbol = "🐌", AppendInfo = true)]
		Timeout,

		[EnumDisplayString("Unusable", Symbol = "🚫", AppendInfo = false)]
		Unusable
	}
}