using System;
using System.Collections.Generic;
using System.Linq;

namespace Shortlist.Web.Services
{
    public class ShortlistAssistantService : IShortlistAssistantService
    {
        public string GetReply(string message, string? currentPage = null)
        {
            var text = (message ?? "").Trim().ToLowerInvariant();
            var page = (currentPage ?? "").Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(text))
            {
                return GetPageAwareWelcome(page);
            }

            if (ContainsAny(text, "hello", "hi", "hey"))
                return "Hi! I’m the Shortlist Assistant. I can help with filters, map usage, results, compare, premium, AI reports, and general navigation.";

            if (ContainsAny(text, "filter", "filters"))
                return "Filters help narrow your search. Start by dropping a pin on the map, set your budget and max distance if needed, then choose up to 3 priorities.";

            if (ContainsAny(text, "priority", "priorities"))
                return "You can choose up to 3 priorities. These priorities help Shortlist score areas based on what matters most to you, such as Grocery, Transit, Parks, or Gym/Fitness.";

            if (ContainsAny(text, "map", "pin", "location"))
                return "Click on the map to drop a pin. That pin becomes the center of your search area. You can also drag the pin to refine the location.";

            if (ContainsAny(text, "radius"))
                return "Radius controls how far around your selected pin Shortlist looks for nearby places and amenities. A larger radius usually gives broader coverage.";

            if (ContainsAny(text, "max distance", "distance"))
                return "Max Distance means how far you are willing to travel from your selected location. Lower values usually give more focused results.";

            if (ContainsAny(text, "budget"))
                return "Budget is optional. If you enter it, Shortlist can use it as part of your housing search context.";

            if (ContainsAny(text, "overall match", "score", "match"))
                return "Overall Match summarizes how well an area fits your selected priorities using the live nearby data found around your chosen location.";

            if (ContainsAny(text, "confidence"))
                return "Confidence reflects how strong the result is based on the amount of useful live data found nearby. More matching data usually means higher confidence.";

            if (ContainsAny(text, "top strength"))
                return "Top Strength shows the category where the selected area performed best based on the nearby live data.";

            if (ContainsAny(text, "weakest area"))
                return "Weakest Area highlights the part of the result that had the lowest support from nearby live data.";

            if (ContainsAny(text, "heatmap"))
                return "Heatmap View is a premium feature that visually highlights stronger and weaker nearby matches on the map.";

            if (ContainsAny(text, "premium"))
                return "Premium unlocks AI Best Area Recommendation, Heatmap View, and AI Report generation.";

            if (ContainsAny(text, "cancel premium", "cancel subscription", "cancel"))
                return "You can cancel Premium from the Premium page. Your access will remain active until the end of the current premium period.";

            if (ContainsAny(text, "report", "ai report"))
                return "The AI Report summarizes the strongest recommended area based on your selected priorities and the live data gathered by Shortlist.";

            if (ContainsAny(text, "compare"))
                return "Use Compare to add places or areas side by side so you can review them before deciding.";

            if (ContainsAny(text, "save", "saved search", "saved searches"))
                return "You can save a search from the Results page. Saved searches help you come back to important combinations of location and priorities later.";

            if (ContainsAny(text, "listing", "listings"))
                return "The Listings section helps you browse available housing results connected to your search context.";

            if (ContainsAny(text, "tenant checklist"))
                return "The Tenant Checklist helps you keep track of important steps and considerations before renting.";

            if (ContainsAny(text, "no results", "nothing shows", "not showing", "empty"))
                return "If no results appear, try increasing the radius, choosing a different location, or selecting priorities with broader nearby coverage.";

            if (ContainsAny(text, "faq", "help page"))
                return "You can open the FAQ page from Settings for quick answers about filters, results, premium, compare, reports, and general Shortlist usage.";

            return GetFallbackReply(page);
        }

        private static bool ContainsAny(string text, params string[] phrases)
        {
            return phrases.Any(p => text.Contains(p));
        }

        private static string GetPageAwareWelcome(string page)
        {
            if (page.Contains("filters"))
                return "I can help you choose priorities, understand radius, and use the map pin correctly.";

            if (page.Contains("results"))
                return "I can explain Overall Match, Confidence, Heatmap, Compare, and AI Reports.";

            if (page.Contains("premium"))
                return "I can explain premium features, free trial details, and how cancellation works.";

            if (page.Contains("report"))
                return "I can explain the AI Summary, your score, key strengths, and considerations.";

            if (page.Contains("listings"))
                return "I can help you understand the Listings page and how it connects to your search.";

            return "Ask me anything about Shortlist — filters, results, premium, reports, compare, listings, or general help.";
        }

        private static string GetFallbackReply(string page)
        {
            if (page.Contains("results"))
                return "I can help explain your results, scores, confidence, compare options, or premium features on this page.";

            if (page.Contains("filters"))
                return "I can help with the map pin, radius, max distance, and choosing up to 3 priorities.";

            return "I can help with filters, map usage, scores, premium, reports, compare, listings, tenant checklist, and navigation.";
        }
    }
}