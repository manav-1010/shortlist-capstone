namespace Shortlist.Web.Services
{
    public interface IShortlistAssistantService
    {
        string GetReply(string message, string? currentPage = null);
    }
}
