using System;
using InfoCaster.Umbraco.UrlTracker.Models;
using InfoCaster.Umbraco.UrlTracker.ViewModels;
using System.Linq;
using System.Web.Http;
using InfoCaster.Umbraco.UrlTracker.NewRepositories;
using Umbraco.Web.WebApi;

namespace InfoCaster.Umbraco.UrlTracker.Controllers
{
	public class UrlTrackerManagerController : UmbracoApiController
	{
		private readonly IUrlTrackerNewRepository _urlTrackerRepository;

	    public UrlTrackerManagerController(IUrlTrackerNewRepository urlTrackerRepository)
	    {
		    _urlTrackerRepository = urlTrackerRepository;
	    }

        [HttpGet]
        public IHttpActionResult GetRedirects(int skip, int amount)
        {
	        var entriesResult = _urlTrackerRepository.GetRedirects(skip, amount);

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
	        var entriesResult = _urlTrackerRepository.GetNotFounds(skip, amount);

	        var model = new UrlTrackerOverviewModel
	        {
		        Entries = entriesResult.Records,
                NumberOfEntries = entriesResult.TotalRecords
	        };

	        return Ok(model);
        }

        [HttpGet]
        public IHttpActionResult GetRedirectsByFilter(int skip, int amount, string query)
        {
	        var entriesResult = _urlTrackerRepository.GetRedirectsByFilter(skip, amount, searchQuery: query);

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
            //var entry = UrlTrackerRepository.GetUrlTrackerEntryById(id);
            //return Ok(entry);

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult SaveChanges(UrlTrackerModel model)
        {
            //try
            //{
            //    _urlTrackerRepository.UpdateEntry(model);
            //}
            //catch(Exception e)
            //{
            //    return BadRequest();
            //}

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult Create([FromBody]UrlTrackerModel model)
        {
            //try
            //{
            //    UrlTrackerRepository.AddUrlTrackerEntry(model);
            //    return Ok();
            //}
            //catch
            //{
            //    return BadRequest();
            //}

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult Delete(int id)
        {
            //try
            //{
            //    UrlTrackerRepository.DeleteUrlTrackerEntry(id);
            //    return Ok();
            //}
            //catch
            //{
            //    return BadRequest();
            //}

            return Ok();
        }
    }
}