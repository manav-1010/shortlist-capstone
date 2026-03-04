using System.ComponentModel.DataAnnotations;

namespace Shortlist.Web.Models
{
    public class SettingsViewModel
    {
        public string Email { get; set; } = "";
        public string AuthProvider { get; set; } = "Local";

        [Range(1, 25, ErrorMessage = "Radius must be between 1 and 25 km.")]
        public int DefaultRadiusKm { get; set; } = 3;

        // comma separated in UI
        public string DefaultPriorities { get; set; } = "";
        public string DefaultLocationLabel { get; set; }
    }
}
