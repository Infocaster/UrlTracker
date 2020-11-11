using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfoCaster.Umbraco.UrlTracker.Models
{
	public class UrlTrackerGetResult
	{
		public List<UrlTrackerModel> Records { get; set; }
		public int TotalRecords { get; set; }
	}
}