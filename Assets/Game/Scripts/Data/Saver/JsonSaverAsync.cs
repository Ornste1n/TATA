using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Game.Scripts.Data.Saver
{
    public class JsonSaverAsync : ISaverAsync<IJsonSerializable>
    {
        public static JsonSaverAsync Instance {get; private set;}
        private static JsonSerializerSettings _jsonSettings;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void Init()
        {
            Instance = new JsonSaverAsync();
            _jsonSettings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };
        }
         
        public async Task SaveAsync(string path, IJsonSerializable data)
        {
            string content = JsonConvert.SerializeObject(data, Formatting.Indented, _jsonSettings);

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

            return JsonConvert.DeserializeObject<TM>(content, _jsonSettings);
        }
    }
}