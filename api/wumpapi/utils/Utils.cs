namespace wumpapi.utils;

public static class Utils
{
    public static T DictToObject<T>(this IDictionary<string, object> source) where T : new()
    {
        return (T) DictToObject(source, typeof(T));
    }

    private static object DictToObject(this IDictionary<string, object> source, Type targetType)
    {
        var result = targetType.GetConstructor(Type.EmptyTypes).Invoke(null);
        foreach (var item in source)
        {
            if (targetType.GetProperty(item.Key) != null)
            {
                var conversionType = targetType.GetProperty(item.Key)?.PropertyType;
                targetType.GetProperty(item.Key)?.SetValue(result,
                    item.Value is IDictionary<string, object>
                        ? ((Dictionary<string, object>)item.Value).DictToObject(conversionType)
                        : Convert.ChangeType(item.Value, conversionType));
            }
            else
                throw new Exception($"Cannot convert {item.Key} to {targetType}");
        }
        return result;
    }
}