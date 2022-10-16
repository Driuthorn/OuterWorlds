using System.Threading.Tasks;

namespace Database.Interface
{
    public interface IRedisCache
    {
        Task<bool> Add<TObject>(string key, TObject obj, int expirationDuration);
        Task<TObject> Retrieve<TObject>(string key);
        Task<bool> Remove(string key);
    }
}
