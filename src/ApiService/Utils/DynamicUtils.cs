using System.Dynamic;
using System.Text.Json;

namespace ApiService.Utils;

public static class DynamicUtils
{
    public static ExpandoObject ToExpando(dynamic dynamicObj)
    {
        string json = JsonSerializer.Serialize(dynamicObj);
        return JsonSerializer.Deserialize<ExpandoObject>(json)!;
    }

    public static bool HasProperty(dynamic obj, string propertyName)
    {
        return obj is IDictionary<string, object> dict
            && dict.ContainsKey(propertyName);
    }

    public static object GetProperty(dynamic obj, string propertyName)
    {
        if (
            obj is IDictionary<string, object> dict
            && dict.ContainsKey(propertyName)
        )
        {
            return obj[propertyName];
        }

        throw new InvalidOperationException();
    }
}
