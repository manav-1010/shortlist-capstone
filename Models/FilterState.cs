using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shortlist.Web.Models
{
    public class FilterState
    {
        [Range(0, 100000, ErrorMessage = "Budget must be between 0 and 100,000.")]
        public decimal? Budget { get; set; }

        [Range(0,100, ErrorMessage = "Distance must be between 0 and 100 km.")]
        public int? MaxDistanceKm { get; set; }

        // store selected priorities
        public List<string> Priorities { get; set; } = new List<string>();
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public int RadiusKm { get; set; } = 3; // default
        public string? LocationLabel { get; set; } 
    }
}
