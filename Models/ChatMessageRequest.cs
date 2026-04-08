namespace Shortlist.Web.Models
{
    public class ChatMessageRequest
    {
        public string Message { get; set; } = "";
        public string? CurrentPage { get; set; }
    }
}
