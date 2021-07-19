using System.Collections.Generic;

namespace InfoCaster.Umbraco.UrlTracker.Models
{
	public class UrlTrackerGetResult
	{
		public List<UrlTrackerModel> Records { get; set; }
		public int TotalRecords { get; set; }
	}
}