using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models.PublishedContent;

namespace InfoCaster.Umbraco.UrlTracker.Models
{
	public class UrlTrackerNodeModel
	{
		public int Id { get; set; }
		public string Url { get; set; }
		public string Name { get; set; }
		public UrlTrackerNodeModel Parent { get; set; }
	}
}