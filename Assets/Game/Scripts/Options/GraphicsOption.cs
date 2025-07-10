using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Game.Scripts.Options.Models;

namespace Game.Scripts.Options
{
    public class GraphicsOption : Option<GraphicModel>
    {
        protected override void PresetOptions()
        {
            RefreshRate rate = new RefreshRate
            {
                denominator = 1,
                numerator = Model.RefreshRate
            };
            
            Screen.SetResolution(Model.Width, Model.Height, Model.ScreenMode, rate);
        }

        public Quality GetValue(GraphicsQuality graphicsQuality) =>  Model.Qualities[graphicsQuality];

        public bool HasOneQualityLevel()
        {
            Quality first = Model.Qualities.Values.First();
            return Model.Qualities.Values.All(q => q == first);
        }

        public void UpdateVsync(bool active)
        {
            Model.VerticalSync = active;
            Application.targetFrameRate = -1;
            QualitySettings.vSyncCount = active ? 1 : 0;
        }
        
        public void UpdateResolution(int width, int height, int rate, FullScreenMode mode)
        {
            uint numerator = (uint)rate;
            RefreshRate refreshRate = new RefreshRate { numerator = numerator, denominator = 1 };
            
            Model.Width = width;
            Model.Height = height;
            Model.ScreenMode = mode;
            Model.RefreshRate = numerator;
            
            Screen.SetResolution(width, height, mode, refreshRate);
        }
        
        public void UpdateModels(Quality level)
        {
            Model.Qualities[GraphicsQuality.Models] = level;
            Debug.Log("ChangeModels " + level);
        }
        
        public void UpdatePhysics(Quality level)
        {
            Model.Qualities[GraphicsQuality.Physics] = level;
            Debug.Log("ChangePhysics " + level);
        }
        
        public void UpdateTexture(Quality level)
        {
            Model.Qualities[GraphicsQuality.Textures] = level;
            Debug.Log("ChangeTexture " + level);
        }
        
        public void UpdateShaders(Quality level)
        {
            Model.Qualities[GraphicsQuality.Shaders] = level;
            Debug.Log("ChangeShaders " + level);
        }
        
        public void UpdateShadows(Quality level)
        {
            Model.Qualities[GraphicsQuality.Shadows] = level;
            Debug.Log("ChangeShadows " + level);
        }
        
        public void UpdateTerrain(Quality level)
        {
            Model.Qualities[GraphicsQuality.Terrain] = level;
            Debug.Log("ChangeTerrain " + level);
        }
        
        public void UpdateLightning(Quality level)
        {
            Model.Qualities[GraphicsQuality.Lightning] = level;
            Debug.Log("ChangeLightning " + level);
        }
        
        public void UpdatePostprocessing(Quality level)
        {
            Model.Qualities[GraphicsQuality.PostProcessing] = level;
            Debug.Log("ChangePostprocessing " + level);
        }

        public override GraphicModel CreateDefaultModel()
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
    }
}