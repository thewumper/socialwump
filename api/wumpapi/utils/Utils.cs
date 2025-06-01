namespace wumpapi.utils;

public static class Utils
{
    public static void RunAfterDelay(Action action, TimeSpan delay, ILogger logger)
    {
        Task.Run(async () =>
        {
            await Task.Delay(delay);
            action();
        }).ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                logger.LogError($"Task failed: {t.Exception}");
            }
        }, TaskScheduler.Default);
    }
    
    
    
    /// <summary>
    /// Recursively convert a dictionary to an object, Dictionary keys should match objects properties.
    /// </summary>
    /// <param name="source">The original dictionary</param>
    /// <list type="T">The type to convert to, needs an empty constructor</list>
    /// <returns>A new instance of an object</returns>
    /// <exception cref="InvalidCastException">If an item in the dictionary cannot be converted properly</exception>
    public static T DictToObject<T>(this IDictionary<string, object> source) where T : new()
    {
        return (T) DictToObject(source, typeof(T));
    }

    /// <remarks>
    /// This is necessary because you cannot pass a type into a generic function, the generics are evaluated at compile time, so we just wrap it with a generic function because they are prettier
    /// </remarks>
    private static object DictToObject(this IDictionary<string, object> source, Type targetType)
    {
        var result = targetType.GetConstructor(Type.EmptyTypes)!.Invoke(null);
        // Loop through all of the items in the original dictionary
        foreach (var item in source)
        {
            // Ensure that the dictionary key is actually a property of the object
            if (targetType.GetProperty(item.Key) != null)
            {
                // Make sure something isn't wacky
                var conversionType = targetType.GetProperty(item.Key)?.PropertyType;
                // Either do a recursion or try to directly convert
                if (conversionType != null)
                    targetType.GetProperty(item.Key)?.SetValue(result,
                        item.Value is IDictionary<string, object>
                            ? ((Dictionary<string, object>)item.Value).DictToObject(conversionType)
                            : Convert.ChangeType(item.Value, conversionType));
                else
                    throw new InvalidCastException($"Cannot convert {item.Key} to {conversionType}");
            }
            else
                throw new InvalidCastException($"{item.Key} not found in {targetType}");
        }
        return result;
    }
}