using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoCaster.Umbraco.UrlTracker.Models;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace InfoCaster.Umbraco.UrlTracker.Helpers
{
	public interface IUrlTrackerNewHelper
	{
		string ResolveShortestUrl(string url);
		string ResolveUmbracoUrl(string url);
		bool IsReservedPathOrUrl(string url);
	}
}
