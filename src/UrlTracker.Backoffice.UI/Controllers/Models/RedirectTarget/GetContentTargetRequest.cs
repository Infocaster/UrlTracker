using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlTracker.Backoffice.UI.Controllers.Models.RedirectTarget
{
    internal class GetContentTargetRequest
    {
        [Required]
        public int? Id { get; set; }

        public string? Culture { get; set; }
    }
}
