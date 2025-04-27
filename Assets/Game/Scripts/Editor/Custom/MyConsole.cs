using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Custom
{
    public class MyConsole : EditorWindow
    {
        private Texture2D _backgroundTexture;
        
        [MenuItem("Console/Console")]
        private static void OpenWindow()
        {
            GetWindow<MyConsole>("Console").Show();
        }
        
        private void OnEnable()
        {
            _backgroundTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Game/Source/Develop/nebula.jpg");
            
            Vector2 fixedSize = new Vector2(600, 600);
            minSize = fixedSize;
            maxSize = fixedSize;
        }

        private void OnGUI()
        {
            if (_backgroundTexture != null)
            {
                GUI.DrawTexture(new Rect(0, 0, position.width, position.height), _backgroundTexture, ScaleMode.StretchToFill);
            }
        }
    }
}