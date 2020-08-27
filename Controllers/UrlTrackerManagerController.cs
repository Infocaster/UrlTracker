using InfoCaster.Umbraco.UrlTracker.Models;
using InfoCaster.Umbraco.UrlTracker.Repositories;
using InfoCaster.Umbraco.UrlTracker.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Umbraco.Web.WebApi;

namespace InfoCaster.Umbraco.UrlTracker.Controllers
{
    public class UrlTrackerManagerController : UmbracoApiController
    {
        [HttpGet]
        public IHttpActionResult Index(int skip, int ammount)
        {
            var allEntries = UrlTrackerRepository.GetUrlTrackerEntries();
            var model = new UrlTrackerOverviewModel
            {
                Entries = allEntries.Skip(skip).Take(ammount).AsEnumerable(),
                TotalPages = (int)(allEntries.Count / ammount)
            };
            return Ok(model);
        }

        [HttpGet]
        public IHttpActionResult Details(int id)
        {
            var entry = UrlTrackerRepository.GetUrlTrackerEntryById(id);
            return Ok(entry);
        }

        [HttpPost]
        public IHttpActionResult SaveChanges(UrlTrackerModel model)
        {
            try
            {
                UrlTrackerRepository.UpdateUrlTrackerEntry(model);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IHttpActionResult Create(UrlTrackerModel model)
        {
            try
            {
                UrlTrackerRepository.AddUrlTrackerEntry(model);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                UrlTrackerRepository.DeleteUrlTrackerEntry(id);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}