using InfoCaster.Umbraco.UrlTracker.Models;
using System.Collections.Generic;

namespace InfoCaster.Umbraco.UrlTracker.ViewModels
{
	public class UrlTrackerOverviewModel
    {
        public IEnumerable<UrlTrackerModel> Entries { get; set; }
        public int NumberOfEntries { get; set; }
    }
}