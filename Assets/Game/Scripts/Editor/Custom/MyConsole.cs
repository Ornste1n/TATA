using UnityEditor;

namespace Game.Scripts.Custom
{
    public class MyConsole : EditorWindow
    {
        [MenuItem("Console/Console")]
        private static void OpenWindow()
        {
            GetWindow<MyConsole>("Console").Show();
        }
    }
}