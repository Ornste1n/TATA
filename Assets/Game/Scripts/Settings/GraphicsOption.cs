using UnityEngine;
using System.Threading.Tasks;
using Game.Scripts.Data.Saver;
using Game.Scripts.UI.Options.Models;
using Paths = Game.Scripts.Data.Paths;

namespace Game.Scripts.Settings
{
    public enum Quality
    {
        Low,
        Medium,
        High,
    }
    
    public static class GraphicsOption
    {
        private static GraphicModel _model;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static async void PresetGraphics()
        {
            return;
            
            _model = await JsonSaverAsync.Instance.LoadAsync<GraphicModel>(Paths.Graphics);

            RefreshRate rate = new RefreshRate
            {
                denominator = 1,
                numerator = _model.RefreshRate
            };
            
            Screen.SetResolution(_model.Width, _model.Height, _model.ScreenMode, rate);
        }

        public static async Task SaveGraphics()
        {
            return;
            
            await JsonSaverAsync.Instance.SaveAsync(Paths.Graphics, _model);
        }

        public static void UpdateVsync(bool active)
        {
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
            Debug.Log("ChangeModels " + level);
        }
        
        public static void UpdatePhysics(Quality level)
        {
            Debug.Log("ChangePhysics " + level);
        }
        
        public static void UpdateTexture(Quality level)
        {
            Debug.Log("ChangeTexture " + level);
        }
        
        public static void UpdateShaders(Quality level)
        {
            Debug.Log("ChangeShaders " + level);
        }
        
        public static void UpdateShadows(Quality level)
        {
            Debug.Log("ChangeShadows " + level);
        }
        
        public static void UpdateTerrain(Quality level)
        {
            Debug.Log("ChangeTerrain " + level);
        }
        
        public static void UpdateLightning(Quality level)
        {
            Debug.Log("ChangeLightning " + level);
        }
        
        public static void UpdatePostprocessing(Quality level)
        {
            Debug.Log("ChangePostprocessing " + level);
        }
    }
}