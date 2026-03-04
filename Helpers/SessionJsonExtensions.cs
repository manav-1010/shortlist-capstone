using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Shortlist.Web.Helpers
{
    public static class SessionJsonExtensions
    {
        public static void SetJson<T>(this ISession session, string key, T value)
            => session.SetString(key, JsonSerializer.Serialize(value));

        public static T? GetJson<T>(this ISession session, string key)
        {
            var json = session.GetString(key);
            if (string.IsNullOrWhiteSpace(json)) return default;
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}