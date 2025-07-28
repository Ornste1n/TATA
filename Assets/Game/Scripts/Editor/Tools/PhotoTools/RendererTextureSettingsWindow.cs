using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UIElements;

namespace Game.Scripts.Editor.Tools.PhotoTools
{
    public class RendererTextureSettingsWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset _treeAsset;

        private Button _updateButton;
        
        private TextField _width;
        private TextField _height;
        private TextField _depth;
        private TextField _mipCount;
        private EnumField _enumField;
        
        [MenuItem("Tools/Generate Texture/Texture Settings")]
        private static void OpenSettings()
        {
            GetWindow<RendererTextureSettingsWindow>("Texture Settings").Show();
        }

        private void CreateGUI()
        {
            _treeAsset.CloneTree(rootVisualElement);
            maxSize = new Vector2(350, 150);
            minSize = maxSize;

            VisualElement root = rootVisualElement;

            _width = root.Q<TextField>("width");
            _height = root.Q<TextField>("height");
            _depth = root.Q<TextField>("depth");
            _mipCount = root.Q<TextField>("mip-count");
            _enumField = root.Q<EnumField>("format");
             
            _width.value = CameraTextureRenderer.Settings.Width.ToString();
            _height.value = CameraTextureRenderer.Settings.Height.ToString();
            _depth.value = CameraTextureRenderer.Settings.Depth.ToString();
            _mipCount.value = CameraTextureRenderer.Settings.MipCount.ToString();
            _enumField.value = CameraTextureRenderer.Settings.Format;

            _updateButton = root.Q<Button>();
            _updateButton.RegisterCallback<ClickEvent>(OnUpdated);
        }

        private void OnUpdated(ClickEvent clickEvent)
        {
            VisualElement root = rootVisualElement;
            TextureRenderSettings settings = new()
            {
                Width = Convert.ToInt32(_width.value),
                Height = Convert.ToInt32(_height.value),
                Depth = Convert.ToInt32(_depth.value),
                MipCount = Convert.ToInt32(_mipCount.value),
                Format = (RenderTextureFormat)_enumField.value
            };
            
            CameraTextureRenderer.Settings = settings;
        }

        private void OnDestroy()
        {
            _updateButton.UnregisterCallback<ClickEvent>(OnUpdated);
        }
        
        public struct TextureRenderSettings
        {
            public int Width;
            public int Height;
            public int Depth;
            public int MipCount;
            public RenderTextureFormat Format;

            public TextureRenderSettings(int width, int height, int depth, int mipCount, RenderTextureFormat format)
            {
                Width = width;
                Height = height;
                Depth = depth;
                MipCount = mipCount;
                Format = format;
            }
        }
    }
}