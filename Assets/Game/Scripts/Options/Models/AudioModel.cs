using System;
using Game.Scripts.Data.Saver;
using Newtonsoft.Json;

namespace Game.Scripts.Options.Models
{
    public class AudioModel : IJsonSerializable
    {
        #region Volumes

        [JsonProperty(Required = Required.Always)]
        public float MasterVolume {get; set;}
        
        [JsonProperty(Required = Required.Always)]
        public float Music {get; set;}
        
        [JsonProperty(Required = Required.Always)]
        public float Effects {get; set;}
        
        [JsonProperty(Required = Required.Always)]
        public float Interface {get; set;}

        #endregion

        public void Validate()
        {
            MasterVolume = Clamp(MasterVolume);
            Music = Clamp(Music);
            Effects = Clamp(Effects);
            Interface = Clamp(Interface);
            
            float Clamp(float value) => (value < 0 || value > 100) ? 100f : value;
        }
    }
}