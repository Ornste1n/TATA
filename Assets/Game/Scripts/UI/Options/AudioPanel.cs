using Game.Scripts.Options;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Game.Scripts.UI.Options.Base;
using Game.Scripts.UI.Frontend.CustomElements;

namespace Game.Scripts.UI.Options
{
    public class AudioPanel : OptionPanel
    {
        private readonly List<CustomSlider> _sliderCallbacks = new();
        private readonly Dictionary<IEventHandler, CustomSlider> _sliderChanges = new();
        
        public AudioPanel(OptionsWindow window, string name, string button) : base(window, name, button)
        {
            RegisterSlider(AudioVolumeEnum.MasterVolume,"master-volume-slider", "master-volume-value-label");
            RegisterSlider(AudioVolumeEnum.Music,"music-volume-slider", "music-volume-value-label");
            RegisterSlider(AudioVolumeEnum.Effects,"effects-volume-slider", "effects-volume-value-label");
            RegisterSlider(AudioVolumeEnum.Interface,"interface-volume-slider", "interface-volume-value-label");
        }

        public override void Save()
        {
            foreach (CustomSlider slider in _sliderCallbacks)
            {
                OptionsManager.Audio.ChangeVolume(slider.Type, slider.Slider.value);
                slider.Slider.RegisterCallback<ChangeEvent<float>>(HandleSliderCallback);
            }
            
            _sliderCallbacks.Clear();
        }

        public override bool HasChanged() => _sliderCallbacks.Count != 0;

        private void RegisterSlider(AudioVolumeEnum type, string sliderName, string labelName)
        {
            float value = OptionsManager.Audio.GetValue(type);
            CustomSlider customSlider = new CustomSlider(OptionsWindow.Root, type, sliderName, labelName, value);

            _sliderChanges.Add(customSlider.Slider, customSlider);
            
            customSlider.Slider.RegisterCallback<ChangeEvent<float>>(HandleSliderCallback);
        }

        private void HandleSliderCallback(ChangeEvent<float> changeEvent)
        {
            CustomSlider customSlider = _sliderChanges[changeEvent.currentTarget];
            
            customSlider.Slider.UnregisterCallback<ChangeEvent<float>>(HandleSliderCallback);
            
            _sliderCallbacks.Add(customSlider);
        }
        
        public override void Reset(){}
        
        public override void Dispose(){}
    }
}