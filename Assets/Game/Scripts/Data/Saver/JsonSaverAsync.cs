using System.IO;
using MessagePack;
using UnityEngine;
using System.Threading.Tasks;

namespace Game.Scripts.Data.Saver
{
    public class JsonSaverAsync : ISaverAsync<IJsonSerializable>
    {
        public static JsonSaverAsync Instance {get; private set;}
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            Instance = new JsonSaverAsync();
        }
        
        public async Task SaveAsync(string path, IJsonSerializable data)
        {
            byte[] result = MessagePackSerializer.Serialize(data);
            await File.WriteAllBytesAsync(path, result);
        }

        public async Task<TM> LoadAsync<TM>(string path) where TM : class, IJsonSerializable
        {
            byte[] result = await File.ReadAllBytesAsync(path);
            return MessagePackSerializer.Deserialize<TM>(result);
        }
    }
}