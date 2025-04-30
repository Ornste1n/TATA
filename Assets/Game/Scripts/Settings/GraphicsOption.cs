using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Game.Scripts.Data.Saver;
using Game.Scripts.UI.Options.Models;
using Paths = Game.Scripts.Data.Paths;

namespace Game.Scripts.Settings
{
    public static class GraphicsOption
    {
        private static GraphicModel _model;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static async void PresetGraphics()
        {
            try
            {
                _model = await JsonSaverAsync.Instance.LoadAsync<GraphicModel>(Paths.Graphics);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Graphics Exception: Model is invalid");
                _model = CreateDefaultModel(); // todo изменить настройки учитывая системные параметры компьютера пользователя
                Debug.LogWarning("Create Default Model");
                await SaveGraphics();
            }
            
            RefreshRate rate = new RefreshRate
            {
                denominator = 1,
                numerator = _model.RefreshRate
            };
            
            Debug.Log("Load model and apply config");
            
            Screen.SetResolution(_model.Width, _model.Height, _model.ScreenMode, rate);
        }

        public static Quality GetValue(GraphicsQuality graphicsQuality) =>  _model.Qualities[graphicsQuality];
        
        public static async Task SaveGraphics()
        {
            await JsonSaverAsync.Instance.SaveAsync(Paths.Graphics, _model);
        }

        public static void UpdateVsync(bool active)
        {
            _model.VerticalSync = active;
            
            Application.targetFrameRate = -1;
            QualitySettings.vSyncCount = active ? 1 : 0;
        }
        
        public static void UpdateResolution(int width, int height, int rate, FullScreenMode mode)
        {
            uint numerator = (uint)rate;
            RefreshRate refreshRate = new RefreshRate { numerator = numerator, denominator = 1 };
            
            _model.Width = width;
            _model.Height = height;
            _model.ScreenMode = mode;
            _model.RefreshRate = numerator;
            
            Screen.SetResolution(width, height, mode, refreshRate);
        }
        
        public static void UpdateModels(Quality level)
        {
            _model.Qualities[GraphicsQuality.Models] = level;
            Debug.Log("ChangeModels " + level);
        }
        
        public static void UpdatePhysics(Quality level)
        {
            _model.Qualities[GraphicsQuality.Physics] = level;
            Debug.Log("ChangePhysics " + level);
        }
        
        public static void UpdateTexture(Quality level)
        {
            _model.Qualities[GraphicsQuality.Textures] = level;
            Debug.Log("ChangeTexture " + level);
        }
        
        public static void UpdateShaders(Quality level)
        {
            _model.Qualities[GraphicsQuality.Shaders] = level;
            Debug.Log("ChangeShaders " + level);
        }
        
        public static void UpdateShadows(Quality level)
        {
            _model.Qualities[GraphicsQuality.Shadows] = level;
            Debug.Log("ChangeShadows " + level);
        }
        
        public static void UpdateTerrain(Quality level)
        {
            _model.Qualities[GraphicsQuality.Terrain] = level;
            Debug.Log("ChangeTerrain " + level);
        }
        
        public static void UpdateLightning(Quality level)
        {
            _model.Qualities[GraphicsQuality.Lightning] = level;
            Debug.Log("ChangeLightning " + level);
        }
        
        public static void UpdatePostprocessing(Quality level)
        {
            _model.Qualities[GraphicsQuality.PostProcessing] = level;
            Debug.Log("ChangePostprocessing " + level);
        }

        private static GraphicModel CreateDefaultModel()
        {
            Resolution current = Screen.currentResolution;

            GraphicModel model = new()
            {
                Width = current.width,
                Height = current.height,
                RefreshRate = current.refreshRateRatio.numerator,
                ScreenMode = Screen.fullScreenMode,
                VerticalSync = QualitySettings.vSyncCount == 1,
            };

            GraphicsQuality[] graphics = (GraphicsQuality[])Enum.GetValues(typeof(GraphicsQuality));
            model.Qualities = new Dictionary<GraphicsQuality, Quality>(graphics.Length);
            
           foreach (GraphicsQuality graphic in graphics)
           {
               model.Qualities.Add(graphic, Quality.High);
           }

           return model;
        }
    }
    
    public enum Quality
    {
        Low,
        Medium,
        High,
    }

    public enum GraphicsQuality
    {
        Models,
        Shaders,
        Shadows,
        Terrain,
        Physics,
        Textures,
        Lightning,
        PostProcessing,
        GraphicsQuality
    }
}