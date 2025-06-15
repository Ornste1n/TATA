using Newtonsoft.Json;
using Game.Scripts.Data.Saver;
using Game.Scripts.Options.Models;

namespace Game.Scripts.Options
{
    public class OptionModel : IJsonSerializable
    {
        [JsonProperty(Required = Required.Always)]
        public GraphicModel Graphics;
        
        [JsonProperty(Required = Required.Always)]
        public AudioModel Audio;

        [JsonProperty(Required = Required.Always)]
        public ControlsModel Controls;

        public void Validate()
        {
            Audio.Validate();
            Graphics.Validate();
            Controls.Validate();
        }
    }
}