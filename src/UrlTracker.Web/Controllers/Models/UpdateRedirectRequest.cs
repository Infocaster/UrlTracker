using System.ComponentModel.DataAnnotations;

namespace UrlTracker.Web.Controllers.Models
{
    public class UpdateRedirectRequest
        : RedirectRequestBase, IIdRequest
    {
        [Required]
        public int? Id { get; set; }
    }
}
