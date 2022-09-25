using System.ComponentModel.DataAnnotations;

namespace UrlTracker.Backoffice.UI.Controllers.Models
{
    public class DeleteEntryRequest
        : IIdRequest
    {
        [Required]
        public int? Id { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Redirects are now deleted through a separate endpoint")]
        public bool Is404 { get; set; } = false;
    }
}
