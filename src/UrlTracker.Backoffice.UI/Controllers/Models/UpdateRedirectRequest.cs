using System.ComponentModel.DataAnnotations;

namespace UrlTracker.Backoffice.UI.Controllers.Models
{
    public class UpdateRedirectRequest
        : RedirectRequestBase, IIdRequest
    {
        [Required]
        public int? Id { get; set; }
    }
}
