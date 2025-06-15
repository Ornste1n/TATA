using Game.Scripts.Options;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Game.Scripts.Options.Models;
using Game.Scripts.UI.Options.Base;
using Game.Scripts.UI.Frontend.CustomElements;

namespace Game.Scripts.UI.Options
{
    public class ControlsPanel : OptionPanel
    {
        private readonly List<CustomSlider<ControlsSensitivity>> _sliderCallbacks = new();
        private readonly Dictionary<IEventHandler, CustomSlider<ControlsSensitivity>> _sliderChanges = new();
        
        public ControlsPanel(OptionsWindow window, string name, string button) : base(window, name, button)
        {
            RegisterSlider(ControlsSensitivity.MouseScroll,"mouse-scroll-slider", "mouse-scroll-value-label");
            RegisterSlider(ControlsSensitivity.KeyboardScroll,"keyboard-scroll-slider", "keyboard-scroll-value-label");
            RegisterSlider(ControlsSensitivity.DragScroll,"drag-scroll-slider", "drag-scroll-value-label");
        }

        public override void Save()
        {
            ControlsModelRaw modelRaw = new ControlsModelRaw()
            {
                DragScrollSpeed = -1f,
                MouseScrollSpeed = -1f,
                KeyboardScrollSpeed = -1f,
            };
            
            foreach (CustomSlider<ControlsSensitivity> slider in _sliderCallbacks)
            {
                switch (slider.Type)
                {
                    case ControlsSensitivity.DragScroll:
                        modelRaw.DragScrollSpeed = slider.Slider.value;
                        break;
                    case ControlsSensitivity.MouseScroll:
                        modelRaw.MouseScrollSpeed = slider.Slider.value;
                        break;
                    case ControlsSensitivity.KeyboardScroll:
                        modelRaw.KeyboardScrollSpeed = slider.Slider.value;
                        break;
                }
                
                slider.Slider.RegisterCallback<ChangeEvent<float>>(HandleSliderCallback);
            }
            
            _sliderCallbacks.Clear();
            OptionsManager.Controls.UpdateSensitivity(modelRaw);
        }

        public override bool HasChanged() => _sliderCallbacks.Count != 0; 
        
        private void RegisterSlider(ControlsSensitivity type, string sliderName, string labelName)
        {
            float value = OptionsManager.Controls.GetValue(type);
            CustomSlider<ControlsSensitivity> customSlider = new(OptionsWindow.Root, type, sliderName, labelName, value);

            _sliderChanges.Add(customSlider.Slider, customSlider);
            
            customSlider.Slider.RegisterCallback<ChangeEvent<float>>(HandleSliderCallback);
        }
        
        private void HandleSliderCallback(ChangeEvent<float> changeEvent)
        {
            CustomSlider<ControlsSensitivity> customSlider = _sliderChanges[changeEvent.currentTarget];
            
            customSlider.Slider.UnregisterCallback<ChangeEvent<float>>(HandleSliderCallback);
            
            _sliderCallbacks.Add(customSlider);
        }
        
        public override void Reset() { }
        
        public override void Dispose() { }
    }
}