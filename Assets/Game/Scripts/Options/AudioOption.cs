using Game.Scripts.Options.Models;

namespace Game.Scripts.Options
{
    public class AudioOption : Option<AudioModel>
    {
        protected override void PresetOptions()
        {
            
        }
        
        public void ChangeVolume(AudioVolumeEnum type, float value)
        {
            switch (type)
            {
                case AudioVolumeEnum.MasterVolume:
                    Model.MasterVolume = value;
                    break;
                case AudioVolumeEnum.Music:
                    Model.Music = value;
                    break;
                case AudioVolumeEnum.Effects:
                    Model.Effects = value;
                    break;
                case AudioVolumeEnum.Interface:
                    Model.Interface = value;
                    break;
            }
        }
        
        public float GetValue(AudioVolumeEnum type)
        {
            return type switch
            {
                AudioVolumeEnum.MasterVolume => Model.MasterVolume,
                AudioVolumeEnum.Music => Model.Music,
                AudioVolumeEnum.Effects => Model.Effects,
                AudioVolumeEnum.Interface => Model.Interface,
                _ => 0
            };
        }
        
        public override AudioModel CreateDefaultModel()
        {
            return new AudioModel()
            {
                MasterVolume = 100f,
                Music = 100f,
                Effects = 100f,
                Interface = 100f
            };
        }
    }

    public enum AudioVolumeEnum
    {
        MasterVolume,
        Music,
        Effects,
        Interface
    }
}