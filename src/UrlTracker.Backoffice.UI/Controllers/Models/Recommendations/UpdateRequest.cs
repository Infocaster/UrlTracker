using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Recommendations;

internal class UpdateRequest
{
    [Required]
    public int Id { get; set; }

    public Guid? RecommendationStrategy { get; set; }

    public bool? Ignore { get; set; }

}

internal class UpdateCollectionRequest : Collection<UpdateRequest> { }