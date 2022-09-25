namespace UrlTracker.Backoffice.UI.Controllers.Models
{
    public interface IPaginationRequest
    {
        int? Skip { get; set; }
        int? Amount { get; set; }
    }
}
