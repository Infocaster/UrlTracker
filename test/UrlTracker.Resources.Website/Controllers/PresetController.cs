using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Database.Models;
using UrlTracker.Resources.Testing.Clients.Models;
using UrlTracker.Resources.Website.Preset;

namespace UrlTracker.Resources.Website.Controllers
{
    [Route("api/preset")]
    public class PresetController : UmbracoApiController
    {
        private readonly IPresetService _presetService;
        private readonly IOptions<UrlTrackerSettings> _configuration;
        private readonly IUmbracoMapper _mapper;
        private readonly UmbracoHelper _umbraco;

        public PresetController(IPresetService presetService, IOptions<UrlTrackerSettings> configuration, IUmbracoMapper mapper, UmbracoHelper umbraco)
        {
            _presetService = presetService;
            _configuration = configuration;
            _mapper = mapper;
            _umbraco = umbraco;
        }

        [HttpPost, Route("reset")]
        public async Task<IActionResult> Reset()
        {
            _presetService.EnsureContent();
            await _presetService.WipeUrlTrackerTablesAsync();
            _presetService.SetConfiguration(null);
            _presetService.ResetCache();
            return Ok();
        }

        [HttpPost, Route("seedRedirect")]
        public IActionResult SeedRedirect([FromBody] SeedRedirectRequest request)
        {
            if (request is null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entries = _mapper.MapEnumerable<SeedRedirectRequestRedirect, UrlTrackerEntry>(request.Redirects);
            _presetService.Insert(entries);
            _presetService.ResetCache();
            return Ok();
        }

        [HttpGet, Route("tree")]
        public IActionResult GetTree()
        {
            var result = new ContentTreeViewModelCollection
            {
                RootContent = _mapper.MapEnumerable<IPublishedContent, ContentTreeViewModel>(_umbraco.ContentAtRoot())
            };

            return Ok(result);
        }

        [HttpGet, Route("settings")]
        public IActionResult GetSettings()
        {
            return Ok(_configuration.Value);
        }

        [HttpPost, Route("settings")]
        public IActionResult SetSettings([FromBody] UrlTrackerSettings settings)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _presetService.SetConfiguration(settings);
            return Ok();
        }
    }
}