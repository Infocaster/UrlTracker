using System.ComponentModel.DataAnnotations;

namespace UrlTracker.Resources.Website.Models
{
    public class GenerateRandomRequest
    {
        [Required]
        public string BaseUrl { get; set; }
    }
}
