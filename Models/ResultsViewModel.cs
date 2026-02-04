namespace Shortlist.Web.Models
{
    public class ResultsViewModel
    {
        public FilterState Filters { get; set; } = new();
        public List<ResultItem> Items { get; set; } = new();
    }
}
