using Core.Extensions;

namespace Core.Helpers
{
    /// <summary>
    /// Provides utility methods for generating cache keys.
    /// </summary>
    /// <remarks>The <see cref="CachingHelper"/> class offers methods to create cache keys by combining
    /// different components such as prefixes, keys, and optional hashes. This can be useful for ensuring unique and
    /// consistent cache key generation across an application.</remarks>
    public static class CachingHelper
    {
        /// <summary>
        /// Generates a cache key by combining a prefix, a key, and an optional hash.
        /// </summary>
        /// <param name="prepend">The prefix to prepend to the generated cache key.</param>
        /// <param name="key">The main key component of the cache key.</param>
        /// <param name="hash">An optional hash to include in the cache key. If not provided, the key is generated without it.</param>
        /// <returns>A string representing the generated cache key, which includes the prefix, key, and optional hash.</returns>
        public static string GenCacheKey(string prepend, string key, string? hash = null)
        {
            return $"{prepend}_{($"{key}_{hash}").GenHashString()}";
        }
    }
}
