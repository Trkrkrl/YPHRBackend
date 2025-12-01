using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Core.Utilities.IoC;
using System.Collections.Concurrent;

namespace Core.CrossCuttingConcerns.Caching.Microsoft
{
    public class MemoryCacheManager : ICacheManager
    {
        private readonly IMemoryCache _memoryCache;

      
        private static readonly ConcurrentDictionary<string, byte> _keys = new();

        public MemoryCacheManager()
        {
            _memoryCache = ServiceTool.ServiceProvider.GetService<IMemoryCache>()!;
        }

        public void Add(string key, object value, int duration)
        {
            _memoryCache.Set(key, value, TimeSpan.FromMinutes(duration));
            _keys.TryAdd(key, 0); 
        }

        public T Get<T>(string key)
        {
            return _memoryCache.Get<T>(key)!;
        }

        public object Get(string key)
        {
            return _memoryCache.Get(key)!;
        }

        public bool IsAdd(string key)
        {
            return _memoryCache.TryGetValue(key, out _);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
            _keys.TryRemove(key, out _);
        }

        public void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var keysToRemove = _keys.Keys
                .Where(k => regex.IsMatch(k))
                .ToList();

            foreach (var key in keysToRemove)
            {
                _memoryCache.Remove(key);
                _keys.TryRemove(key, out _);
            }
        }
    }
}