using System;
using Newtonsoft.Json;
using Game.Scripts.Data.Saver;

namespace Game.Scripts.Options.Models
{
    [Serializable]
    public class ControlsModel : IJsonSerializable
    {
        [JsonProperty(Required = Required.Always)]
        public float DragScrollSpeed { get; set; }

        [JsonProperty(Required = Required.Always)]
        public float MouseScrollSpeed { get; set; }

        [JsonProperty(Required = Required.Always)]
        public float KeyboardScrollSpeed { get; set; }

        public void Validate()
        {
            DragScrollSpeed = Clamp(DragScrollSpeed);
            MouseScrollSpeed = Clamp(MouseScrollSpeed);
            KeyboardScrollSpeed = Clamp(KeyboardScrollSpeed);

            float Clamp(float value) => (value < 0 || value > 100) ? 100f : value;
        }
    }

    public struct ControlsModelRaw
    {
        public float DragScrollSpeed;
        public float MouseScrollSpeed;
        public float KeyboardScrollSpeed;
    }
}