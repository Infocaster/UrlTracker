using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.WebApi;
using UrlTracker.Core.Configuration;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Database.Models;
using UrlTracker.Resources.Testing.Clients.Models;
using UrlTracker.Resources.Website.Preset;

namespace UrlTracker.Resources.Website.Controllers
{
    [RoutePrefix("api/preset")]
    public class PresetController : UmbracoApiController
    {
        private readonly IPresetService _presetService;
        private readonly IConfiguration<UrlTrackerSettings> _configuration;

        public PresetController(IPresetService presetService, IConfiguration<UrlTrackerSettings> configuration)
        {
            _presetService = presetService;
            _configuration = configuration;
        }

        [HttpPost, Route("reset")]
        public async Task<IHttpActionResult> Reset()
        {
            _presetService.EnsureContent();
            await _presetService.WipeUrlTrackerTablesAsync();
            _presetService.SetConfiguration(null);
            return Ok();
        }

        [HttpPost, Route("seedRedirect")]
        public IHttpActionResult SeedRedirect([FromBody] SeedRedirectRequest request)
        {
            if (request is null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entries = Mapper.MapEnumerable<SeedRedirectRequestRedirect, UrlTrackerEntry>(request.Redirects);
            _presetService.Insert(entries);
            return Ok();
        }

        [HttpGet, Route("tree")]
        public IHttpActionResult GetTree()
        {
            var result = new ContentTreeViewModelCollection
            {
                RootContent = Mapper.MapEnumerable<IPublishedContent, ContentTreeViewModel>(Umbraco.ContentAtRoot())
            };

            return Ok(result);
        }

        [HttpGet, Route("settings")]
        public IHttpActionResult GetSettings()
        {
            return Ok(_configuration.Value);
        }

        [HttpPost, Route("settings")]
        public IHttpActionResult SetSettings([FromBody] UrlTrackerSettings settings)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _presetService.SetConfiguration(settings);
            return Ok();
        }
    }
}