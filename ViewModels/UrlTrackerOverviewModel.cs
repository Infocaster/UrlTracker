using InfoCaster.Umbraco.UrlTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfoCaster.Umbraco.UrlTracker.ViewModels
{
    public class UrlTrackerOverviewModel
    {
        public IEnumerable<UrlTrackerModel> Entries { get; set; }
        public int TotalPages { get; set; }
        
    }
}