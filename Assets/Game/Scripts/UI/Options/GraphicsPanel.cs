using System;
using System.Linq;
using UnityEngine;
using Game.Scripts.Options;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using Game.Scripts.UI.Options.Base;
using Game.Scripts.Extension.Toolkit;
using System.Diagnostics.CodeAnalysis;

namespace Game.Scripts.UI.Options
{
    public sealed class GraphicsPanel : OptionPanel
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
        
        private readonly List<(Quality Value, Action<Quality>)> _changedGraphics = new(4);
        private readonly Dictionary<IEventHandler, Action<Quality>> _graphicsCallback = new(8);

        private readonly HashSet<Action> _changedCallbacks = new(4);
        private readonly Dictionary<IEventHandler, Action> _elementsCallbacks = new(4);
        
        public GraphicsPanel(OptionsWindow window, string name, string button) : base(window, name, button)
        {
            Resolution();
            DisplayMode();
            Vsync();

            InitGraphicsOption();
        }

        public override void Save()
        {
            foreach (Action callback in _changedCallbacks)
                callback?.Invoke();

            foreach (var (quality, action) in _changedGraphics)
                action?.Invoke(quality);
            
            _changedGraphics.Clear();
            _changedCallbacks.Clear();
        }

        public override void Reset() { }

        public override bool HasChanged() => _changedCallbacks?.Count != 0 || _changedGraphics.Count != 0;

        private void InitGraphicsOption()
        {
            List<string> qualityLevels = OptionsWindow.QualityLocalize.Keys.ToList();
            
            RegisterGraphics(ref _models, "models", qualityLevels, GraphicsQuality.Models, OptionsManager.Graphics.UpdateModels);
            RegisterGraphics(ref _shaders, "shaders", qualityLevels, GraphicsQuality.Shaders, OptionsManager.Graphics.UpdateShaders);
            RegisterGraphics(ref _shadows, "shadows", qualityLevels, GraphicsQuality.Shadows, OptionsManager.Graphics.UpdateShadows);
            RegisterGraphics(ref _terrain, "terrain", qualityLevels, GraphicsQuality.Terrain, OptionsManager.Graphics.UpdateTerrain);
            RegisterGraphics(ref _physics, "physics", qualityLevels, GraphicsQuality.Physics, OptionsManager.Graphics.UpdatePhysics);
            RegisterGraphics(ref _textures, "textures", qualityLevels, GraphicsQuality.Textures, OptionsManager.Graphics.UpdateTexture);
            RegisterGraphics(ref _lightning, "lightning", qualityLevels, GraphicsQuality.Lightning, OptionsManager.Graphics.UpdateLightning);
            RegisterGraphics(ref _postProcessing, "post-processing", qualityLevels, GraphicsQuality.PostProcessing, OptionsManager.Graphics.UpdatePostprocessing);
            
            _graphicsQuality = OptionsWindow.Root.Q<DropdownField>("graphics-quality");

            _graphicsQuality.choices = qualityLevels;
            _isCustomOptions = !OptionsManager.Graphics.HasOneQualityLevel();
            _graphicsQuality.value = !_isCustomOptions ? _models.value : OptionsWindow.CustomLocalLabel;
            
            _graphicsQuality.RegisterCallback<ChangeEvent<string>>(ChangeGraphicsQuality);
        }
        
        private void RegisterGraphics
        (
            [AllowNull] ref DropdownField field, 
            string name, List<string> choices,
            GraphicsQuality graphic, 
            Action<Quality> graphicsMethod
        )
        {
            field = OptionsWindow.Root.Q<DropdownField>(name);

            if (field == null) throw new ArgumentException("Field is null");
            
            field.choices = choices;
            field.value = field.choices[(int)OptionsManager.Graphics.GetValue(graphic)];
            
            field.RegisterValueChangedCallback(CheckCurrentGraphicsPreset);
            _graphicsCallback.Add(field, graphicsMethod);
        }

        private void CheckCurrentGraphicsPreset(ChangeEvent<string> evt)
        {
            if (!_isCustomOptions && evt.newValue != _graphicsQuality.value)
            {
                _isCustomOptions = true;
                _graphicsQuality.value = OptionsWindow.CustomLocalLabel;
            }

            Quality quality = OptionsWindow.QualityLocalize[evt.newValue];
            _changedGraphics.Add((quality, _graphicsCallback[evt.currentTarget]));
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

            _resolutionField.RegisterEvent<ChangeEvent<string>>(HandleCallback, _elementsCallbacks, SetResolution);
            _refreshRateField.RegisterEvent<ChangeEvent<string>>(HandleCallback, _elementsCallbacks, SetResolution);
            
            _resolutionField.RegisterCallback<ChangeEvent<string>>(UpdateRefreshRates);
        }
        
        private void HandleCallback(EventBase eventBase)
        {
            Action callback = _elementsCallbacks[eventBase.currentTarget];
            _changedCallbacks.Add(callback);
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
                _screenModeField.choices.Add(localString);

            _screenModeField.RegisterEvent<ChangeEvent<string>>(HandleCallback, _elementsCallbacks, SetResolution);
        }

        private void Vsync()
        {
            _vsync = OptionsWindow.Root.Q<Toggle>("vsync");
            _vsync.value = QualitySettings.vSyncCount != 0;

            _vsync.RegisterEvent<ClickEvent>(HandleCallback, _elementsCallbacks, SetVsync);
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

            if (_refreshRateField.enabledSelf)
                _refreshRateField.value = _refreshRateField.choices.Last();
        }

        private void SetVsync()
        {
            _refreshRateField.SetEnabled(!_vsync.value);
            _refreshRateField.value = $"{Screen.currentResolution.refreshRateRatio.value:F0}Hz";
            OptionsManager.Graphics.UpdateVsync(_vsync.value);
        }

        private void SetResolution()
        {
            (int Width, int Height) res = GetResolution(_resolutionField.value);

            FullScreenMode mode = OptionsWindow.ScreenModeLocalize[_screenModeField.value];

            string rateValue = _refreshRateField.value;
            ReadOnlySpan<char> span = rateValue.AsSpan();
            int i = 0;
            int value = 0;

            while (i < span.Length && char.IsDigit(span[i]))
            {
                value = value * 10 + (span[i] - '0');
                i++;
            }

            OptionsManager.Graphics.UpdateResolution(res.Width, res.Height, value, mode);
        }

        private void ChangeGraphicsQuality(ChangeEvent<string> evt)
        {
            if (evt.newValue == OptionsWindow.CustomLocalLabel) return;

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
                else if (width == 0)
                {
                    width = current;
                    current = 0;
                }

                i++;
            }

            height = current;

            return (width, height);
        }

        private static (int left, int right) BinarySearchRefreshRates(int width, int height)
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
        
        public override void Dispose()
        {
            
        }
    }
}
