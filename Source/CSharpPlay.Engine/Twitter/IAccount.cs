
namespace CSharpPlay.Engine.Twitter
{
	public interface IAccount
	{
		string ConsumerKey { get; }
		string ConsumerSecret { get; }
		string AccessToken { get; }
		string AccessTokenSecret { get; }
		string SelfScreenName { get; }
	}
}