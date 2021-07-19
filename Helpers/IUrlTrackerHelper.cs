namespace InfoCaster.Umbraco.UrlTracker.Helpers
{
	public interface IUrlTrackerHelper
	{
		string ResolveShortestUrl(string url);
		string ResolveUmbracoUrl(string url);
		bool IsReservedPathOrUrl(string url);
	}
}
