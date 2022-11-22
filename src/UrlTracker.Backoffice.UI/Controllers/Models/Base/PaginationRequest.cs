using System.ComponentModel.DataAnnotations;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Base
{
    internal class PaginationRequest
    {
        [Required]
        public uint Page { get; set; }

        [Required]
        public uint PageSize { get; set; }
    }
}
