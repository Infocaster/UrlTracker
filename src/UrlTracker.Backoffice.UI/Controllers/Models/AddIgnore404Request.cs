using System.ComponentModel.DataAnnotations;

namespace UrlTracker.Backoffice.UI.Controllers.Models
{
    public class AddIgnore404Request
        : IIdRequest
    {
        [Required]
        public int? Id { get; set; }
    }
}
