using System;
using InfoCaster.Umbraco.UrlTracker.Models;
using InfoCaster.Umbraco.UrlTracker.ViewModels;
using System.Linq;
using System.Web.Http;
using InfoCaster.Umbraco.UrlTracker.NewRepositories;
using InfoCaster.Umbraco.UrlTracker.Services;
using Umbraco.Web.WebApi;

namespace InfoCaster.Umbraco.UrlTracker.Controllers
{
	public class UrlTrackerManagerController : UmbracoApiController
	{
		private readonly IUrlTrackerService _urlTrackerService;

		public UrlTrackerManagerController(IUrlTrackerService urlTrackerService)
		{
			_urlTrackerService = urlTrackerService;
		}

		[HttpGet]
        public IHttpActionResult GetRedirects(int skip, int amount)
        {
	        var entriesResult = _urlTrackerService.GetRedirects(skip, amount);

            var model = new UrlTrackerOverviewModel
            {
                Entries = entriesResult.Records,
                NumberOfEntries = entriesResult.TotalRecords
            };

            return Ok(model);
        }

        [HttpGet]
        public IHttpActionResult GetNotFounds(int skip, int amount)
        {
	        var entriesResult = _urlTrackerService.GetNotFounds(skip, amount);

	        var model = new UrlTrackerOverviewModel
	        {
		        Entries = entriesResult.Records,
                NumberOfEntries = entriesResult.TotalRecords
	        };

	        return Ok(model);
        }

        [HttpGet]
        public IHttpActionResult GetRedirectsByFilter(int skip, int amount, string query, UrlTrackerSortType sortType)
        {
	        var entriesResult = _urlTrackerService.GetRedirectsByFilter(skip, amount, sortType, query);

            var model = new UrlTrackerOverviewModel
            {
	            Entries = entriesResult.Records,
	            NumberOfEntries = entriesResult.TotalRecords
            };

            return Ok(model);
        }

        [HttpGet]
        public IHttpActionResult Details(int id)
        {
			return Ok(_urlTrackerService.GetEntryById(id));
        }

        [HttpPost]
        public IHttpActionResult SaveChanges(UrlTrackerModel model)
        {
            _urlTrackerService.UpdateEntry(model);
            return Ok();
        }

		[HttpPost]
		public IHttpActionResult AddEntry([FromBody] UrlTrackerModel model)
		{
			_urlTrackerService.AddEntry(model);
			return Ok();
		}

		[HttpPost]
		public IHttpActionResult Delete(int id)
		{
			_urlTrackerService.DeleteEntryById(id);
			return Ok();
		}
	}
}