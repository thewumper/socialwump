namespace wumpapi.utils;

public static class Utils
{
    public static T DictToObject<T>(this IDictionary<string, object> source) where T : new()
    {
        var result = new T();
        Type resultType = typeof(T);
        foreach (var item in source)
        {
            if (resultType.GetProperty(item.Key) != null)
                resultType.GetProperty(item.Key)?.SetValue(result, Convert.ChangeType(item.Value, resultType));
            else
                throw new Exception($"Cannot convert {item.Key} to {resultType}");
        }

        return result;
    } 
}