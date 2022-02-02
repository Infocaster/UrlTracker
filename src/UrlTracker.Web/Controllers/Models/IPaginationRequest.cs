namespace UrlTracker.Web.Controllers.Models
{
    public interface IPaginationRequest
    {
        int? Skip { get; set; }
        int? Amount { get; set; }
    }
}
