using System.ComponentModel.DataAnnotations;

namespace UrlTracker.Web.Controllers.Models
{
    public class GetLanguagesFromNodeRequest
    {
        [Required]
        public int? NodeId { get; set; }
    }
}
