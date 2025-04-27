using System.Threading.Tasks;

namespace Game.Scripts.Data.Saver
{
    public interface ISaverAsync<T>
    {
        public Task SaveAsync(string path, T data);
        
        public Task<TM> LoadAsync<TM>(string path) where TM : class, T;
    }
}