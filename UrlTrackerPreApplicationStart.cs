using InfoCaster.Umbraco.UrlTracker;
using InfoCaster.Umbraco.UrlTracker.Modules;
using InfoCaster.Umbraco.UrlTracker.Providers;
using System.Web.Hosting;

[assembly: System.Web.PreApplicationStartMethod(typeof(UrlTrackerPreApplicationStart), "PreApplicationStart")]
namespace InfoCaster.Umbraco.UrlTracker
{
	public class UrlTrackerPreApplicationStart
	{
		public static void PreApplicationStart()
		{
			Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(UrlTrackerModule));
			HostingEnvironment.RegisterVirtualPathProvider(new EmbeddedResourcesVirtualPathProvider());
		}
	}
}