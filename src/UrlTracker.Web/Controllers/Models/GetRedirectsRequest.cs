using System.ComponentModel.DataAnnotations;

namespace UrlTracker.Web.Controllers.Models
{
    public class GetRedirectsRequest
        : IPaginationRequest
    {
        [Required]
        public int? Skip { get; set; }
        [Required]
        public int? Amount { get; set; }
        public string? Query { get; set; }
        public OrderBy SortType { get; set; } = OrderBy.CreatedDesc;
    }
}
