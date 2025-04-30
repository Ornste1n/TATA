using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Scripts.Settings;
using Game.Scripts.Data.Saver;

namespace Game.Scripts.UI.Options.Models
{
    [Serializable]
    public class GraphicModel : IJsonSerializable
    {
        #region Display

        public int Width;
        public int Height;
        public uint RefreshRate;
        public FullScreenMode ScreenMode;
        public bool VerticalSync;

        #endregion

        public Dictionary<GraphicsQuality, Quality> Qualities;
    }
}