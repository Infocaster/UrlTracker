using System.ComponentModel.DataAnnotations;

namespace UrlTracker.Backoffice.UI.Controllers.Models
{
    public class GetLanguagesFromNodeRequest
    {
        [Required]
        public int? NodeId { get; set; }
    }
}
