using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using Game.Scripts.UI.Options.Base;
using Game.Scripts.UI.Options.Models;

namespace Game.Scripts.UI.Options
{
    public enum QualityLevel
    {
        Low,
        Medium,
        High,
    }
    
    public sealed class GraphicsPanel : Option
    {
        #region Display
        private Toggle _vsync;
        private DropdownField _screenModeField;
        private DropdownField _resolutionField;
        private DropdownField _refreshRateField;
        #endregion

        #region Graphics
        private DropdownField _models;
        private DropdownField _shaders;
        private DropdownField _shadows;
        private DropdownField _terrain;
        private DropdownField _physics;
        private DropdownField _textures;
        private DropdownField _lightning;
        private DropdownField _postProcessing;
        private DropdownField _graphicsQuality;
        #endregion

        private bool _isCustomOptions;
        
        public GraphicsPanel(OptionsWindow window, string name, string button) : base(window, name, button)
        {
            InitializeCollection();
            
            Resolution();
            DisplayMode();
            Vsync();

            InitGraphicsOption();
        }

        public override void Save()
        {
            foreach (Action callback in ChangedCallbacks)
                callback?.Invoke();
            
            ChangedCallbacks.Clear();
        }

        public override void Reset() { }

        private void RegisterGraphics(ref DropdownField field, string name, List<string> choices, Action onChanged)
        {
            field = OptionsWindow.Root.Q<DropdownField>(name);
            field.choices = choices;

            field.RegisterValueChangedCallback(CheckCurrentGraphicsPreset);
            
            SubscribeChangedCallback(field, onChanged);
        }

        private void CheckCurrentGraphicsPreset(ChangeEvent<string> evt)
        {
            if (_isCustomOptions || evt.newValue == _graphicsQuality.value) return;
            
            _isCustomOptions = true;
            _graphicsQuality.value = OptionsWindow.CustomLocalLabel;
        }
        
        private void InitGraphicsOption()
        {
            List<string> qualityLevels = OptionsWindow.QualityLocalize.Keys.ToList();
            
            RegisterGraphics(ref _models, "models", qualityLevels, ChangeModels);
            RegisterGraphics(ref _shaders, "shaders", qualityLevels, ChangeShaders);
            RegisterGraphics(ref _shadows, "shadows", qualityLevels, ChangeShadows);
            RegisterGraphics(ref _terrain, "terrain", qualityLevels, ChangeTerrain);
            RegisterGraphics(ref _physics, "physics", qualityLevels, ChangePhysics);
            RegisterGraphics(ref _textures, "textures", qualityLevels, ChangeTexture);
            RegisterGraphics(ref _lightning, "lightning", qualityLevels, ChangeLightning);
            RegisterGraphics(ref _postProcessing, "post-processing", qualityLevels, ChangePostprocessing);
            
            _graphicsQuality = OptionsWindow.Root.Q<DropdownField>("graphics-quality");
            _graphicsQuality.choices = qualityLevels;

            _graphicsQuality.RegisterCallback<ChangeEvent<string>>(ChangeGraphicsQuality);

            string currentQuality = OptionsWindow.QualityLocalize.First(x => x.Value == QualityLevel.High).Key;
            _graphicsQuality.value = currentQuality;
        }
        
        private void Resolution()
        {
            _resolutionField = OptionsWindow.Root.Q<DropdownField>("resolution");
            _refreshRateField = OptionsWindow.Root.Q<DropdownField>("refresh-rate");
            
            Resolution[] resolutions = Screen.resolutions;
            Resolution current = Screen.currentResolution;
            
            _resolutionField.value = $"{current.width} x {current.height}";
            _refreshRateField.value = $"{current.refreshRateRatio}Hz";
            
            _resolutionField.choices = new List<string>(resolutions.Length);
            _refreshRateField.choices = new List<string>(4);
            
            for (var i = resolutions.Length - 1; i >= 0; i--)
                _resolutionField.choices.Add($"{resolutions[i].width} x {resolutions[i].height}");

            SetValidRates(current.width, current.height);
            
            SubscribeChangedCallback(_resolutionField, SetResolution);
            SubscribeChangedCallback(_refreshRateField, SetResolution);
            
            _resolutionField.RegisterCallback<ChangeEvent<string>>(UpdateRefreshRates);
        }

        private void UpdateRefreshRates(ChangeEvent<string> changed)
        {
            (int Width, int Height) resolution = GetResolution(changed.newValue);
            SetValidRates(resolution.Width, resolution.Height);
        }
        
        private void DisplayMode()
        {
            _screenModeField = OptionsWindow.Root.Q<DropdownField>("display-mode");
            _screenModeField.choices = new List<string>(3);

            _screenModeField.value = OptionsWindow.ScreenModeLocalize.First(x => x.Value == Screen.fullScreenMode).Key;

            foreach (string localString in OptionsWindow.ScreenModeLocalize.Keys)
            {
                _screenModeField.choices.Add(localString);
            }
            
            SubscribeChangedCallback(_screenModeField, SetResolution);
        }

        private void Vsync()
        {
            _vsync = OptionsWindow.Root.Q<Toggle>("vsync");
            _vsync.value = false;
            
            SubscribeCallback(_vsync, SetVsync);
        }

        private void SetValidRates(int width, int height)
        {
            _refreshRateField.choices.Clear();
            
            (int left, int right) = BinarySearchRefreshRates(width, height);

            if (left == -1) left = right;

            if (right == -1) right = left;

            if (right == -1 && left == -1)
            {
                Debug.LogError("Not found refresh rates");
                return;
            }
            
            for (int i = left; i <= right; i++)
            {
                Resolution res = Screen.resolutions[i];
                _refreshRateField.choices.Add($"{res.refreshRateRatio.value:F0}Hz");
            }

            if(_refreshRateField.enabledSelf)
                _refreshRateField.value = _refreshRateField.choices.Last();
        }
        
        private void SetVsync()
        {
            Application.targetFrameRate = -1;
            QualitySettings.vSyncCount = _vsync.value ? 1 : 0;
            
            _refreshRateField.SetEnabled(!_vsync.value);
            _refreshRateField.value = $"{Screen.currentResolution.refreshRateRatio.value:F0}Hz";
        }
        
        private void SetResolution()
        {
            (int Width, int Height) res = GetResolution(_resolutionField.value);

            FullScreenMode mode =OptionsWindow.ScreenModeLocalize[_screenModeField.value];
            
            string rateValue = _refreshRateField.value;
            ReadOnlySpan<char> span = rateValue.AsSpan();
            int i = 0;
            int value = 0;

            while (i < span.Length && char.IsDigit(span[i]))
            {
                value = value * 10 + (span[i] - '0');
                i++;
            }
            
            RefreshRate refreshRate = new RefreshRate()
            {
                numerator = (uint)value,
                denominator = 1
            };
            
            Screen.SetResolution(res.Width, res.Height, mode, refreshRate);
        }
        
        private void ChangeModels()
        {
            Debug.Log("ChangeModels");
        }
        
        private void ChangePhysics()
        {
            Debug.Log("ChangePhysics");
        }
        
        private void ChangeTexture()
        {
            Debug.Log("ChangeTexture");
        }
        
        private void ChangeShaders()
        {
            Debug.Log("ChangeShaders");
        }
        
        private void ChangeShadows()
        {
            Debug.Log("ChangeShadows");
        }
        
        private void ChangeTerrain()
        {
            Debug.Log("ChangeTerrain");
        }
        
        private void ChangeLightning()
        {
            Debug.Log("ChangeLightning");
        }
        
        private void ChangePostprocessing()
        {
            Debug.Log("ChangePostprocessing");
        }

        private void ChangeGraphicsQuality(ChangeEvent<string> evt)
        {
            if(evt.newValue == OptionsWindow.CustomLocalLabel) return;

            _isCustomOptions = false;
            
            _models.value = evt.newValue;
            _shaders.value = evt.newValue;
            _shadows.value = evt.newValue;
            _terrain.value = evt.newValue;
            _physics.value = evt.newValue;
            _textures.value = evt.newValue;
            _lightning.value = evt.newValue;
            _postProcessing.value = evt.newValue;
        }

        private (int Width, int Height) GetResolution(string resString)
        {
            ReadOnlySpan<char> span = resString.AsSpan();

            int i = 0;
            int width = 0;
            int height = 0;
            int current = 0;
            
            while (i < span.Length)
            {
                if (char.IsDigit(span[i]))
                {
                    current = current * 10 + (span[i] - '0');
                }
                else if(width == 0)
                {
                    width = current;
                    current = 0;
                }

                i++;
            }
            height = current;
            
            return (width, height);
        }
        
        private (int left, int right) BinarySearchRefreshRates(int width, int height)
        {
            int lastIndex = -1;
            int startIndex = -1;
            
            // left search
            {
                int left = 0;
                int right = Screen.resolutions.Length - 1;

                while (left <= right)
                {
                    int middle = left + (right - left) / 2;
                    Resolution res = Screen.resolutions[middle];

                    if (res.width == width && res.height == height)
                    {
                        startIndex = middle;
                        right = middle - 1;
                    }
                    else if (res.width > width || (res.width == width && res.height > height))
                    {
                        right = middle - 1;
                    }
                    else
                    {
                        left = middle + 1;
                    }
                }
            }
            
            // right search
            {
                int left = startIndex;
                int right = Screen.resolutions.Length - 1;
                
                while (left <= right)
                {
                    int middle = left + (right - left) / 2;
                    Resolution res = Screen.resolutions[middle];

                    if (res.width == width && res.height == height)
                    {
                        lastIndex = middle;
                        left = middle + 1;
                    }
                    else if (res.width > width || (res.width == width && res.height > height))
                    {
                        left = middle + 1;
                    }
                    else
                    {
                        right = middle - 1;
                    }
                }
            }
            
            return (startIndex, lastIndex);
        }
    }
}
