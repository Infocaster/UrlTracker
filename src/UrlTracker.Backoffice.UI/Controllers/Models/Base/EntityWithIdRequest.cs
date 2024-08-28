using System.ComponentModel.DataAnnotations;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Base;

public class EntityWithIdRequest<TData>
{
    [Required]
    public int Id { get; set; }

    [Required]
    public TData Data { get; set; }
}
