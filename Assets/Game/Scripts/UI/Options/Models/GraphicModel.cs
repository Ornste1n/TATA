using UnityEngine;
using MessagePack;

namespace Game.Scripts.UI.Options.Models
{
    [MessagePackObject(true)]
    public class GraphicModel
    {
        #region Display
        
        public int VerticalSync;
        public Resolution Resolution;
        public RefreshRate RefreshRate;
        public FullScreenMode ScreenMode;
        
        #endregion

        // #region Quality
        //
        //
        //
        // #endregion
    }
}