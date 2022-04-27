using System.ComponentModel.DataAnnotations;

namespace UrlTracker.Web.Controllers.Models
{
    public class AddIgnore404Request
        : IIdRequest
    {
        [Required]
        public int? Id { get; set; }
    }
}
