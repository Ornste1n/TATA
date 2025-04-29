using System.IO;
using System.Text;
using MessagePack;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Game.Scripts.Data.Saver
{
    public class JsonSaverAsync : ISaverAsync<IJsonSerializable>
    {
        public static JsonSaverAsync Instance {get; private set;}
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void Init()
        {
            Instance = new JsonSaverAsync();
        }
        
        public async Task SaveAsync(string path, IJsonSerializable data)
        {
            string content = JsonConvert.SerializeObject(data, Formatting.Indented);

            using (var stream = new StreamWriter(path, false, Encoding.UTF8, 4086))
            {
                await stream.WriteAsync(content);
            }
        }

        public async Task<TM> LoadAsync<TM>(string path) where TM : class, IJsonSerializable
        {
            string content = null;
            
            using (var stream = new StreamReader(path, Encoding.UTF8))
            {
                content = await stream.ReadToEndAsync();
            }

            return JsonConvert.DeserializeObject<TM>(content);
        }
    }
}