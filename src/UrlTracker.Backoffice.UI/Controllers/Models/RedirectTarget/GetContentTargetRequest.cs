using System.ComponentModel.DataAnnotations;

namespace UrlTracker.Backoffice.UI.Controllers.Models.RedirectTarget
{
    internal class GetContentTargetRequest
    {
        [Required]
        public int? Id { get; set; }

        public string? Culture { get; set; }
    }
}
