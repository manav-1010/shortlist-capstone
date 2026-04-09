namespace Shortlist.Web.Models
{
    public class AiRecommendationReportViewModel
    {
        public string AreaName { get; set; } = "";
        public int Score { get; set; }
        public string Summary { get; set; } = "";
        public List<string> Priorities { get; set; } = new();
        public List<string> Strengths { get; set; } = new();
        public List<string> Weaknesses { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }
}