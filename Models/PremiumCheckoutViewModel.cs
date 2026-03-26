using System.ComponentModel.DataAnnotations;

namespace Shortlist.Web.Models
{
    public class PremiumCheckoutViewModel
    {
        public string Plan { get; set; } = "monthly";

        [Required]
        public string CardholderName { get; set; } = "";

        [Required]
        public string CardNumber { get; set; } = "";

        [Required]
        public string ExpiryDate { get; set; } = "";

        [Required]
        public string CVV { get; set; } = "";

        [Required, EmailAddress]
        public string BillingEmail { get; set; } = "";

        public string PostalCode { get; set; } = "";
        public string Country { get; set; } = "Canada";
    }
}