using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphBackend.Repositories
{
    public static class Neo4jExtensions
    {
        public static T? GetValueOrDefault<T>(this IReadOnlyDictionary<string, object> props, string key)
        {
            return props.TryGetValue(key, out var value)
                ? (T?)value
                : default;
        }
    }
}
