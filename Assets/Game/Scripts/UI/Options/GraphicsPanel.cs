using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Game.Scripts.UI.Options.Base;

namespace Game.Scripts.UI.Options
{
    public class GraphicsPanel : Option
    {
        private readonly OptionsWindow _optionsWindow;
        
        private readonly HashSet<Action> _changedCallbacks = new(4);
        private readonly Dictionary<IEventHandler, Action> _elementsCallbacks = new(4);
        
        private readonly List<string> _ratesChoicesList = new(5);
        private Dictionary<Resolution, List<RefreshRate>> _resolutionInfo;
        
        private DropdownField _resolutionField;
        private DropdownField _refreshRateField;
        private DropdownField _screenModeField;
        
        public GraphicsPanel(OptionsWindow optionsWindow)
        {
            _optionsWindow = optionsWindow;
            
            Resolution();
            DisplayMode();
        }

        public override void Save()
        {
            foreach (Action callback in _changedCallbacks)
                callback?.Invoke();
            
            _changedCallbacks.Clear();
        }

        public override void Reset()
        {
            
        }

        public override bool HasChanged() => _changedCallbacks.Count != 0;

        private void RegisterChangedCallback(ChangeEvent<string> evt)
        {
            Action callback = _elementsCallbacks[evt.currentTarget];
            _changedCallbacks.Add(callback);
        }
        
        private void SubscribeChangedCallback(VisualElement element, Action callback)
        {
            element.RegisterCallback<ChangeEvent<string>>(RegisterChangedCallback);
            _elementsCallbacks.Add(element, callback);
        }
        
        private void Resolution()
        {
            _resolutionField = _optionsWindow.Root.Q<DropdownField>("resolution");
            _refreshRateField = _optionsWindow.Root.Q<DropdownField>("refresh-rate");
            
            Resolution[] resolutions = Screen.resolutions;
            Resolution current = Screen.currentResolution;

            _resolutionInfo = new Dictionary<Resolution, List<RefreshRate>>(resolutions.Length);
            
            foreach (Resolution resolution in resolutions)
            {
                if (_resolutionInfo.ContainsKey(resolution))
                {
                    _resolutionInfo[resolution].Add(resolution.refreshRateRatio);
                    return;
                }
                
                _resolutionInfo[resolution] = new List<RefreshRate>(5) { resolution.refreshRateRatio };
            }
            
            foreach (List<RefreshRate> value in _resolutionInfo.Values)
                value.TrimExcess();
            
            _resolutionField.value = $"{current.width} x {current.height}";
            _refreshRateField.value = $"{current.refreshRateRatio}Hz";
            
            _resolutionField.choices = new List<string>(_resolutionInfo.Keys.Count);
            
            _ratesChoicesList.Add(_refreshRateField.value);
            _refreshRateField.choices = _ratesChoicesList;
            
            for (var i = resolutions.Length - 1; i >= 0; i--)
                _resolutionField.choices.Add($"{resolutions[i].width} x {resolutions[i].height}");
            
            SubscribeChangedCallback(_resolutionField, SetResolution);
            SubscribeChangedCallback(_refreshRateField, SetResolution);
        }

        private void DisplayMode()
        {
            _screenModeField = _optionsWindow.Root.Q<DropdownField>("display-mode");
            _screenModeField.choices = new List<string>(3);

            _screenModeField.value = _optionsWindow.ScreenModeLocalize.First(x => x.Value == Screen.fullScreenMode).Key;

            foreach (string localString in _optionsWindow.ScreenModeLocalize.Keys)
            {
                _screenModeField.choices.Add(localString);
            }
            
            SubscribeChangedCallback(_screenModeField, SetResolution);
        }

        private void SetResolution()
        {
            Debug.Log("set resolution");
            
            string[] values = _resolutionField.value.Split(' ');
            
            int width = int.Parse(values[0]);
            int height = int.Parse(values[2]);

            FullScreenMode mode =_optionsWindow.ScreenModeLocalize[_screenModeField.value];
            Screen.SetResolution(width, height, mode);
        }
        
        public void Dispose()
        {
            
        }
    }
}
