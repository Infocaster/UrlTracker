using System.ComponentModel.DataAnnotations;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Base
{
    internal class PaginationRequest
    {
        [Required, Range(1, uint.MaxValue)]
        public uint Page { get; set; }

        [Required, Range(1, uint.MaxValue)]
        public uint PageSize { get; set; }
    }
}
