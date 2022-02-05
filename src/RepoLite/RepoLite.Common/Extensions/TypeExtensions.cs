using System;

namespace RepoLite.Common.Extensions
{
    public static class TypeExtensions
    {
        public static object GetDefault(this Type type)
        {
            if (type == typeof(string))
                return "''";
            if (type == typeof(DateTime))
                return $"'{DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")}'";
            return type.IsValueType ? $"'{Activator.CreateInstance(type)}'" : "null";
        }
    }
}