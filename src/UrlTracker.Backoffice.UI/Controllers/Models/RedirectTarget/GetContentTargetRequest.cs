using System.ComponentModel.DataAnnotations;

namespace UrlTracker.Backoffice.UI.Controllers.Models.RedirectTarget
{
    internal class GetContentTargetRequest
    {
        [Required]
        public string Id { get; set; } = null!;

        public string? Culture { get; set; }
    }
}
