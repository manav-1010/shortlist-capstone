namespace Shortlist.Web.Models
{
    public class ResultItem
    {
        public string Name { get; set; } = "";
        public int Score { get; set; }

        public List<string> Pros { get; set; } = new();
        public List<string> Cons { get; set; } = new();
    }
}
