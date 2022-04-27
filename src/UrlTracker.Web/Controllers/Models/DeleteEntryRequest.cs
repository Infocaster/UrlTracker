using System.ComponentModel.DataAnnotations;

namespace UrlTracker.Web.Controllers.Models
{
    public class DeleteEntryRequest
        : IIdRequest
    {
        [Required]
        public int? Id { get; set; }

        public bool Is404 { get; set; } = false;
    }
}
