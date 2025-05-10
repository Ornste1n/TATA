using System;
using UnityEngine;
using Game.Scripts.Data.Saver;
using System.Collections.Generic;
using Game.Scripts.Options;
using Newtonsoft.Json;

namespace Game.Scripts.Data.Options
{
    [Serializable]
    public class GraphicModel : IJsonSerializable
    {
        #region Display

        [JsonProperty(Required = Required.Always)]
        public int Width { get; set; }
        
        [JsonProperty(Required = Required.Always)]
        public int Height { get; set; }
        
        [JsonProperty(Required = Required.Always)]
        public uint RefreshRate { get; set; }
        
        [JsonProperty(Required = Required.Always)]
        public FullScreenMode ScreenMode { get; set; }
        
        [JsonProperty(Required = Required.Always)]
        public bool VerticalSync { get; set; }

        #endregion

        [JsonProperty(Required = Required.Always)]
        public Dictionary<GraphicsQuality, Quality> Qualities { get; set; }
        
        public void Validate()
        {
            if (Qualities.Count != 8)
                throw new Exception("Invalid count qualities");

            foreach (var (graphicsQuality, quality) in Qualities)
            {
                if (!Enum.IsDefined(typeof(Quality), quality))
                {
                    throw new Exception($"Invalid Quality enum value: {quality} for key {quality}");
                }
            }
        }
    }
}