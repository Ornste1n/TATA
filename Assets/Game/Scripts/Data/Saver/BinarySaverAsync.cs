using System.IO;
using MessagePack;
using UnityEngine;
using System.Threading.Tasks;

namespace Game.Scripts.Data.Saver
{
    public class BinarySaverAsync : ISaverAsync<IBinarySerializable>
    {
        public static BinarySaverAsync Instance {get; private set;}
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            Instance = new BinarySaverAsync();
        }
        
        public async Task SaveAsync(string path, IBinarySerializable data)
        {
            byte[] result = MessagePackSerializer.Serialize(data);
            await File.WriteAllBytesAsync(path, result);
        }

        public async Task<TM> LoadAsync<TM>(string path) where TM : class, IBinarySerializable
        {
            byte[] result = await File.ReadAllBytesAsync(path);
            return MessagePackSerializer.Deserialize<TM>(result);
        }
    }
}
