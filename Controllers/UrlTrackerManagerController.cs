using InfoCaster.Umbraco.UrlTracker.Models;
using InfoCaster.Umbraco.UrlTracker.Services;
using InfoCaster.Umbraco.UrlTracker.ViewModels;
using System.Web.Http;
using Umbraco.Web.WebApi;

namespace InfoCaster.Umbraco.UrlTracker.Controllers
{
	public class UrlTrackerManagerController : UmbracoAuthorizedApiController
	{
		private readonly IUrlTrackerService _urlTrackerService;

		public UrlTrackerManagerController(IUrlTrackerService urlTrackerService)
		{
			_urlTrackerService = urlTrackerService;
		}

		#region General

		[HttpPost]
		public IHttpActionResult UpdateEntry(UrlTrackerModel model)
		{
			_urlTrackerService.UpdateEntry(model);
			return Ok();
		}


		[HttpPost]
		public IHttpActionResult DeleteEntry(int id, bool is404 = false)
		{
			_urlTrackerService.DeleteEntryById(id, is404);
			return Ok();
		}

		[HttpGet]
		public IHttpActionResult GetLanguagesOutNodeDomains(int nodeId)
		{
			return Ok(_urlTrackerService.GetLanguagesOutNodeDomains(nodeId));
		}

		#endregion

		#region Redirects

		[HttpPost]
		public IHttpActionResult AddRedirect([FromBody] UrlTrackerModel model)
		{
			if (!ValidateRedirect(model))
				return BadRequest("Not all fields are filled in correctly");

			_urlTrackerService.AddRedirect(model);
			return Ok();
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

		#endregion

		#region Not founds

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
		public IHttpActionResult GetNotFoundsByFilter(int skip, int amount, string query, UrlTrackerSortType sortType)
		{
			var entriesResult = _urlTrackerService.GetNotFoundsByFilter(skip, amount, sortType, query);

			var model = new UrlTrackerOverviewModel
			{
				Entries = entriesResult.Records,
				NumberOfEntries = entriesResult.TotalRecords
			};

			return Ok(model);
		}

		[HttpGet]
		public IHttpActionResult CountNotFoundsThisWeek()
		{
			return Ok(_urlTrackerService.CountNotFoundsThisWeek());
		}

		#endregion

		private bool ValidateRedirect(UrlTrackerModel model)
		{
			if ((string.IsNullOrEmpty(model.OldUrl) && string.IsNullOrEmpty(model.OldRegex)) ||
				(model.RedirectRootNodeId == 0 || model.RedirectRootNodeId == null) ||
				((model.RedirectNodeId == null || model.RedirectNodeId == 0) && string.IsNullOrEmpty(model.RedirectUrl)) ||
				(model.RedirectHttpCode != 301 && model.RedirectHttpCode != 302))
				return false;

			return true;
		}
	}
}