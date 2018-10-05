using System.Threading.Tasks;

namespace Furiza.Caching.Abstractions
{
    public interface ICacheHandler
    {
        bool TryGetValue<T>(string key, out T value) where T : class;
        Task SetAsync<T>(string key, T value) where T : class;
        Task RemoveAsync<T>(string key) where T : class;
    }
}