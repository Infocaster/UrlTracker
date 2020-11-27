using InfoCaster.Umbraco.UrlTracker.Services;
using InfoCaster.Umbraco.UrlTracker.Settings;
using System;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;

namespace InfoCaster.Umbraco.UrlTracker.Models
{
	public class UrlTrackerDomain
	{
		private IUrlTrackerService _urlTrackerService => DependencyResolver.Current.GetService<IUrlTrackerService>();
		private IUrlTrackerNewSettings _urlTrackerSettings => DependencyResolver.Current.GetService<IUrlTrackerNewSettings>();

		private Lazy<IPublishedContent> _node => new Lazy<IPublishedContent>(() =>
		{
			return _urlTrackerService.GetNodeById(NodeId);
		});
		private Lazy<string> _urlWithDomain => new Lazy<string>(() =>
		{
			if (_urlTrackerSettings.HasDomainOnChildNode() && Node != null && Node.Parent != null)
				return Node.Url;

			if (Name.Contains(Uri.UriSchemeHttp))
				return Name;

			return string.Format("{0}{1}{2}",
				HttpContext.Current != null ? HttpContext.Current.Request.Url.Scheme : Uri.UriSchemeHttp,
				Uri.SchemeDelimiter, Name
			);
		});

		public IPublishedContent Node => _node.Value;
		public string UrlWithDomain => _urlWithDomain.Value;

		public int Id { get; set; }
		public int NodeId { get; set; }
		public string Name { get; set; }
		public string LanguageIsoCode { get; set; }

		public override string ToString()
		{
			return UrlWithDomain;
		}
	}
}