using System.Collections;
using System.Text.Json;

namespace RinneganCore.Tools.Extensions;

public static class Formatter
{
    /// <summary>
    /// Cast given object to Json format using Newtonsoft
    /// </summary>
    /// <returns>json string</returns>
    public static string ToJson(this object @this)
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(@this);
    }


    /// <summary>
    /// Cast string to given format json type using Newtonsoft
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="this"></param>
    /// <returns></returns>
    public static T? FromJson<T>(this string @this)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(@this);
    }


    /// <summary>
    /// Cast string to given Enum format
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    /// <returns>Enum format object</returns>
    public static T ToEnum<T>(this string @this) where T : struct, Enum
    {
        return (T)Enum.Parse(typeof(T), @this, true);
    }


    /// <summary>
    /// Cast string to given Enum format, returns null if string cannot be cast
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    /// <returns>Enum format object or null if cast fails</returns>
    public static T? TryToEnum<T>(this string @this) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(@this))
            return null;

        if (Enum.TryParse<T>(@this, true, out T result))
            return result;

        return null;
    }


    /// <summary>
    /// Cast Dictionary to given class type
    /// </summary>
    /// <typeparam name="TResult">response type</typeparam>
    /// <param name="this">Dictionary<string, object></param>
    /// <returns>TResult type object</returns>
    public static TResult? To<TResult>(this Dictionary<string, object> @this)
        where TResult : class
    {
        return @this.ToJson()
           .FromJson<TResult>();
    }


    /// <summary>
    /// Cast Dictianry to given class type
    /// </summary>
    /// <typeparam name="TResult">response type</typeparam>
    /// <typeparam name="TKey">dictionary key type</typeparam>
    /// <typeparam name="TValue">dictionary value type</typeparam>
    /// <param name="this">Dictionary<TKey,TValue></param>
    /// <returns>TResult type object</returns>
    public static TResult? To<TResult, TKey, TValue>(this Dictionary<TKey, TValue> @this)
        where TResult : class
    {
        return @this.ToJson()
           .FromJson<TResult>();
    }


    /// <summary>
    /// Cast Dictionary to given class type
    /// </summary>
    /// <typeparam name="TResult">response type</typeparam>
    /// <param name="this"></param>
    /// <returns>TResult type object</returns>
    public static TResult? To<TResult>(this IDictionary @this)
        where TResult : class
    {
        return @this.ToJson()
           .FromJson<TResult>();
    }


    /// <summary>
    /// Cast JsonElement to Dictionary<string,object>
    /// </summary>
    /// <param name="this"></param>
    /// <returns>Dictionary<string,object></string></returns>
    public static Dictionary<string, object> ToDictionary(this JsonElement @this)
    {
        var result = new Dictionary<string, object>();
        foreach (var property in @this.EnumerateObject())
        {
            result[property.Name] = property.Value.ValueKind switch
            {
                JsonValueKind.String => property.Value.GetString()!,
                JsonValueKind.Number => property.Value.TryGetInt64(out var l) ? l : property.Value.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null!,
                _ => property.Value.GetRawText()
            };
        }

        return result;
    }


    /// <summary>
    /// Cast JsonElement to Dictionary<string,object>
    /// </summary>
    /// <param name="this"></param>
    /// <returns>Dictionary<string,object></string></returns>
    public static Dictionary<string, object> ToDictionary(this JsonElement? @this)
    {
        if (!@this.HasValue || @this.Value.ValueKind != JsonValueKind.Object)
            return default;

        return @this.Value.ToDictionary();
    }


    /// <summary>
    /// Get value of a JsonElement in proper format
    /// </summary>
    /// <typeparam name="T">Expected value type</typeparam>
    /// <param name="this"></param>
    /// <param name="name">name of property in json</param>
    /// <returns>value as expected type</returns>
    public static T? GetValueOrNull<T>(this JsonElement? @this, string name) where T : struct
    {
        if (!TryGetProperty(@this, name, out var property))
            return null;

        return property.Deserialize<T>();
    }


    /// <summary>
    /// Get value of a JsonElement in string format
    /// </summary>
    /// <param name="this"></param>
    /// <param name="name">name of property in json</param>
    /// <returns>value as string</returns>
    public static string? GetStringOrNull(this JsonElement? @this, string name)
    {
        if (!TryGetProperty(@this, name, out var property))
            return null;

        return property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : property.ToString();
    }


    /// <summary>
    /// Navigate inside json
    /// </summary>
    /// <param name="this"></param>
    /// <param name="name">desired property name</param>
    /// <param name="property">found property</param>
    /// <returns>it returens if functions found desired property or not</returns>
    private static bool TryGetProperty(JsonElement? @this, string name, out JsonElement property)
    {
        property = default;
        if (!@this.HasValue || @this.Value.ValueKind != JsonValueKind.Object)
            return false;
        if (!@this.Value.TryGetProperty(name, out property))
            return false;
        return property.ValueKind is not JsonValueKind.Null and not JsonValueKind.Undefined;
    }
}
