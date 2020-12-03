using System;
using System.Linq;
using InfoCaster.Umbraco.UrlTracker.Models;
using InfoCaster.Umbraco.UrlTracker.Services;
using InfoCaster.Umbraco.UrlTracker.ViewModels;
using System.Web.Http;
using Newtonsoft.Json;
using Umbraco.Web;
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
		public IHttpActionResult DeleteEntry(int id, bool is404 = false)
		{
			if(!_urlTrackerService.DeleteEntryById(id, is404))
				return BadRequest();

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
			if (!RedirectIsValid(model))
				return BadRequest("Not all fields are filled in correctly");

			_urlTrackerService.AddRedirect(model);
			return Ok();
		}

		[HttpPost]
		public IHttpActionResult UpdateRedirect([FromBody] UrlTrackerModel model)
		{
			if (!RedirectIsValid(model))
				return BadRequest("Not all fields are filled in correctly");

			_urlTrackerService.UpdateEntry(model);
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

		private bool RedirectIsValid(UrlTrackerModel redirect)
		{
			if ((string.IsNullOrEmpty(redirect.OldUrl) && string.IsNullOrEmpty(redirect.OldRegex)) ||
				(redirect.RedirectRootNodeId == 0 || redirect.RedirectRootNodeId == null) ||
				((redirect.RedirectNodeId == null || redirect.RedirectNodeId == 0) && string.IsNullOrEmpty(redirect.RedirectUrl)) ||
				(redirect.RedirectHttpCode != 301 && redirect.RedirectHttpCode != 302))
				return false;

			return true;
		}
	}
}