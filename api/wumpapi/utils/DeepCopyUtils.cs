using System.Runtime.Serialization;

namespace wumpapi.utils;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
/// <summary>
/// Way to much code for properly copying data. This is used for items. Uses a LOT of reflefction
/// </summary>
public static class DeepCopyUtils
{
    private class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public static readonly ReferenceEqualityComparer Instance = new();
        public new bool Equals(object x, object y) => ReferenceEquals(x, y);
        public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }

    private static readonly MethodInfo CastMethod = typeof(Enumerable)
        .GetMethod("Cast", BindingFlags.Public | BindingFlags.Static);
    
    private static readonly MethodInfo ToListMethod = typeof(Enumerable)
        .GetMethod("ToList", BindingFlags.Public | BindingFlags.Static);

    public static T DeepCopy<T>(T original)
    {
        var visited = new Dictionary<object, object>(ReferenceEqualityComparer.Instance);
        return (T)CopyObject(original, visited);
    }

    private static object CopyObject(object original, IDictionary<object, object> visited)
    {
        if (original == null) return null;

        // Check if we've already copied this object
        if (visited.TryGetValue(original, out var copied))
            return copied;

        Type type = original.GetType();

        // Handle delegates (shallow copy)
        if (typeof(Delegate).IsAssignableFrom(type))
            return original;

        // Handle immutable types
        if (IsImmutable(type))
            return original;

        // Handle arrays
        if (type.IsArray)
            return CopyArray((Array)original, visited);

        // Handle collections
        if (IsCollection(type))
            return CopyCollection(original, visited);

        // Handle regular objects
        return CopyComplexObject(original, visited);
    }

    private static bool IsImmutable(Type type)
    {
        if (type.IsPrimitive || type.IsEnum) return true;
        if (type == typeof(string)) return true;
        if (type == typeof(decimal)) return true;
        if (type == typeof(DateTime)) return true;
        if (type == typeof(TimeSpan)) return true;
        if (type == typeof(Guid)) return true;
        return false;
    }

    private static bool IsCollection(Type type)
    {
        if (type.IsArray) return true;
        if (typeof(IEnumerable).IsAssignableFrom(type)) return true;
        return false;
    }

    private static Array CopyArray(Array original, IDictionary<object, object> visited)
    {
        Type elementType = original.GetType().GetElementType();
        Array copied = (Array)Activator.CreateInstance(
            original.GetType(), 
            original.Length
        );

        visited.Add(original, copied);

        for (int i = 0; i < original.Length; i++)
        {
            object originalItem = original.GetValue(i);
            object copiedItem = CopyObject(originalItem, visited);
            copied.SetValue(copiedItem, i);
        }

        return copied;
    }

    private static object CopyCollection(object original, IDictionary<object, object> visited)
    {
        Type type = original.GetType();
        
        // Handle dictionaries
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            return CopyDictionary(original, visited);
        }

        // Handle lists/collections
        IEnumerable<object> elements = ((IEnumerable)original).Cast<object>();
        IList<object> copiedElements = new List<object>();

        // Create a shallow copy first to add to visited dictionary
        object shallowCopy = CreateShallowCopy(original);
        visited.Add(original, shallowCopy);

        // Copy elements recursively
        foreach (var element in elements)
        {
            copiedElements.Add(CopyObject(element, visited));
        }

        // Populate the collection
        if (type.IsArray)
        {
            Array array = (Array)shallowCopy;
            for (int i = 0; i < copiedElements.Count; i++)
            {
                array.SetValue(copiedElements[i], i);
            }
        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            var addRange = type.GetMethod("AddRange");
            addRange?.Invoke(shallowCopy, new object[] { copiedElements });
        }
        else
        {
            var addMethod = type.GetMethod("Add");
            foreach (var item in copiedElements)
            {
                addMethod?.Invoke(shallowCopy, new[] { item });
            }
        }

        return shallowCopy;
    }

    private static object CopyDictionary(object original, IDictionary<object, object> visited)
    {
        Type type = original.GetType();
        Type[] genericArgs = type.GetGenericArguments();
        Type keyType = genericArgs[0];
        Type valueType = genericArgs[1];
        
        IDictionary copiedDict = (IDictionary)Activator.CreateInstance(type);
        visited.Add(original, copiedDict);

        IDictionary originalDict = (IDictionary)original;
        foreach (DictionaryEntry entry in originalDict)
        {
            object keyCopy = IsImmutable(keyType) 
                ? entry.Key 
                : CopyObject(entry.Key, visited);
            
            object valueCopy = CopyObject(entry.Value, visited);
            copiedDict.Add(keyCopy, valueCopy);
        }

        return copiedDict;
    }

    private static object CreateShallowCopy(object original)
    {
        Type type = original.GetType();
        
        // Try parameterless constructor
        ConstructorInfo ctor = type.GetConstructor(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
            null, Type.EmptyTypes, null);
        
        if (ctor != null)
            return ctor.Invoke(null);
        
        // For collections without parameterless constructor
        if (type.IsArray)
            return Array.CreateInstance(type.GetElementType(), ((Array)original).Length);
        
        if (type.IsGenericType)
        {
            Type genericDef = type.GetGenericTypeDefinition();
            if (genericDef == typeof(List<>))
                return Activator.CreateInstance(type, new object[] { ((ICollection)original).Count });
        }
        
        // Fallback for types without parameterless constructor
        return FormatterServices.GetUninitializedObject(type);
    }

    private static object CopyComplexObject(object original, IDictionary<object, object> visited)
    {
        Type type = original.GetType();
        object copied;
        
        // Try to create instance
        try
        {
            copied = Activator.CreateInstance(type, true);
        }
        catch
        {
            // Fallback for types without parameterless constructor
            copied = FormatterServices.GetUninitializedObject(type);
        }

        visited.Add(original, copied);

        // Copy all fields (public and private)
        foreach (FieldInfo field in GetAllFields(type))
        {
            // Skip readonly and const fields
            if (field.IsInitOnly || field.IsLiteral) 
                continue;

            object originalValue = field.GetValue(original);
            object copiedValue = CopyObject(originalValue, visited);
            field.SetValue(copied, copiedValue);
        }

        return copied;
    }

    private static IEnumerable<FieldInfo> GetAllFields(Type type)
    {
        List<FieldInfo> fields = new();
        
        while (type != null)
        {
            fields.AddRange(type.GetFields(
                BindingFlags.Public | 
                BindingFlags.NonPublic | 
                BindingFlags.Instance | 
                BindingFlags.DeclaredOnly));
            
            type = type.BaseType;
        }
        
        return fields;
    }
}