using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Shortlist.Web.Helpers
{
    // helper extensions for storing complex objects in ASP.NET Session.
    public static class SessionJsonExtensions
    {
        // stores an object as JSON in the session under the specified key.
        public static void SetJson<T>(this ISession session, string key, T value)
            => session.SetString(key, JsonSerializer.Serialize(value));

        // retrieves an object from Session by deserializing JSON.
        public static T? GetJson<T>(this ISession session, string key)
        {
            var json = session.GetString(key);

            // if no value exists in session, return default.
            if (string.IsNullOrWhiteSpace(json)) return default;
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}