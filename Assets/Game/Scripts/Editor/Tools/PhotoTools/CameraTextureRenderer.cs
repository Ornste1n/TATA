using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Editor.Tools.PhotoTools
{
    public static class CameraTextureRenderer
    {
        private const string FilePath = "Assets/Game/Source/UI/Generated Icon/icon.png";
        
        public static RendererTextureSettingsWindow.TextureRenderSettings Settings { get; set; }
            = new (512, 512, 1, 1, RenderTextureFormat.ARGB32);
        
        [MenuItem("Tools/Generate Texture/Create Texture for the Main Camera")]
        private static async void CreateTexture()
        {
            Camera camera = Camera.main;

            if (camera == null) throw new Exception("Camera is null");

            RenderTexture renderTexture = GetRenderTexture();
            camera.targetTexture = renderTexture;

            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0, 0, 0, 0);
            
            camera.Render();
            RenderTexture.active = renderTexture;
            Texture2D texture2D = GetTexture();
            texture2D.ReadPixels(new Rect(0, 0, Settings.Width, Settings.Height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = null;
            camera.targetTexture = null;

            byte[] bytes = texture2D.EncodeToPNG();
            await File.WriteAllBytesAsync(FilePath, bytes);
            AssetDatabase.ImportAsset(FilePath);
        }

        private static Texture2D GetTexture() =>
            new Texture2D(Settings.Width, Settings.Height,  TextureFormat.ARGB32, Settings.MipCount, false);
        
        private static RenderTexture GetRenderTexture() =>
            new RenderTexture(Settings.Width, Settings.Height, Settings.Depth, Settings.Format, Settings.MipCount);
    }
}
