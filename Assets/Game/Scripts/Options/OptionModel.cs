using Newtonsoft.Json;
using Game.Scripts.Data.Saver;
using Game.Scripts.Data.Options;

namespace Game.Scripts.Options
{
    public class OptionModel : IJsonSerializable
    {
        [JsonProperty(Required = Required.Always)]
        public GraphicModel Graphics;
        
        [JsonProperty(Required = Required.Always)]
        public AudioModel Audio;

        public void Validate()
        {
            Audio.Validate();
            Graphics.Validate();
        }
    }
}