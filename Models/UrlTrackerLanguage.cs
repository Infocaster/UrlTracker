using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfoCaster.Umbraco.UrlTracker.Models
{
	public class UrlTrackerLanguage
	{
		public int Id { get; set; }
		public string IsoCode { get; set; }
		public string CultureName { get; set; }
	}
}