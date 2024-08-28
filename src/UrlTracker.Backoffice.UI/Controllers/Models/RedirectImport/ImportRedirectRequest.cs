using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace UrlTracker.Backoffice.UI.Controllers.Models.RedirectImport
{
    internal class ImportRedirectRequest
    {
        [Required]
        public IFormFile Redirects { get; set; } = null!;
    }
}
