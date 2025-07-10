using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace Game.Scripts.Editor.Custom.Tools
{
    public static class InspectorDebugShortcut
    {
        [MenuItem("Tools/Toggle Inspector Debug Mode %#&i")]
        private static void Toggle()
        {
            EditorWindow wnd = EditorWindow.focusedWindow;
            if (wnd == null) return;

            var inspectorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            if (wnd.GetType() != inspectorType)
            {
                Debug.LogWarning("Сначала выделите любое окно Inspector.");
                return;
            }

            var prop = inspectorType.GetProperty(
                "inspectorMode",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            );
            if (prop == null) return;

            var cur = (InspectorMode)prop.GetValue(wnd);
            prop.SetValue(wnd, cur == InspectorMode.Normal ? InspectorMode.Debug : InspectorMode.Normal);
            wnd.Repaint();
        }
    }
}