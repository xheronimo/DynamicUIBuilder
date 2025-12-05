// ============================================================================
// ARCHIVO 4: Controls/CachedReflection.cs
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace DynamicUI.Controls
{
    internal static class CachedReflection
    {
        private static readonly ConcurrentDictionary<(Type, string), PropertyInfo> _propCache = new();

        public static PropertyInfo GetProperty(Type type, string name)
        {
            var key = (type, name.ToLowerInvariant());
            return _propCache.GetOrAdd(key, k => 
                type.GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance));
        }
    }
}