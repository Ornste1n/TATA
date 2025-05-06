using System;
using Newtonsoft.Json;
using Game.Scripts.Data.Saver;

namespace Game.Scripts.Data.Options
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
            void Clamp(float value)
            {
                if (value < 0 || value > 100)
                    throw new Exception($"Volume value {value} out of range [0, 100]");
            }

            Clamp(MasterVolume);
            Clamp(Music);
            Clamp(Effects);
            Clamp(Interface);
        }
    }
}