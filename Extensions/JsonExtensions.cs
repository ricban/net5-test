using System.Text.Json;

namespace Covid19.Client.Extensions
{
    public static class JsonExtensions
    {
        #region Deserialize
        public static TValue? Deserialize<TValue>(this string json, bool propertyNameCaseInsensitive = true)
        {
            return JsonSerializer.Deserialize<TValue>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = propertyNameCaseInsensitive });
        }

        public static TValue? Deserialize<TValue>(this string json, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<TValue>(json, options);
        }
        #endregion

        #region Serialize
        public static string Serialize<TValue>(this TValue value, bool indented = false)
        {
            if (indented)
            {
                return JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true, IgnoreNullValues = true });
            }

            return JsonSerializer.Serialize(value);
        }

        public static string Serialize<TValue>(this TValue value, JsonSerializerOptions options)
        {
            return JsonSerializer.Serialize(value, options);
        }
        #endregion
    }
}