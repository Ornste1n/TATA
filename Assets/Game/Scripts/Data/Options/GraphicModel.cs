using System;
using UnityEngine;
using Game.Scripts.Data.Saver;

namespace Game.Scripts.UI.Options.Models
{
    [Serializable]
    public class GraphicModel : IJsonSerializable
    {
        #region Display

        public int VerticalSync;
        public Resolution Resolution;
        public RefreshRate RefreshRate;
        public FullScreenMode ScreenMode;

        #endregion

        #region Quality

        public QualityLevel Models;
        public QualityLevel Shaders;
        public QualityLevel Shadows;
        public QualityLevel Terrain;
        public QualityLevel Physics;
        public QualityLevel Textures;
        public QualityLevel Lightning;
        public QualityLevel PostProcessing;
        public QualityLevel GraphicsQuality;

        #endregion
    }
}