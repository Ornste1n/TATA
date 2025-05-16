using System;
using UnityEditor;
using UnityEngine;
using Game.Scripts.Data;
using System.Diagnostics;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Game.Scripts.Extension.Toolkit;

namespace Game.Scripts.Editor.Custom
{
    public class MyConsole : EditorWindow
    {
        [SerializeField] private VisualTreeAsset _tree;

        private readonly Dictionary<IEventHandler, Action> _handlers = new(4);
        private readonly ProcessStartInfo _startInfo = new() { UseShellExecute = true };
        
        [MenuItem("Console/Console")]
        private static void OpenWindow()
        {
            GetWindow<MyConsole>("Console").Show();
        }

        private void CreateGUI()
        {
            _tree.CloneTree(rootVisualElement);
            
            maxSize = new Vector2(400, 600);
            minSize = maxSize;

            InitFields();
        }

        private void InitFields()
        {
            VisualElement root = rootVisualElement;
            
            root.Q<Button>("persistent-data-open").RegisterEvent<ClickEvent>(CheckCallback, _handlers, OpenPersistent);
            root.Q<Button>("logs-open").RegisterEvent<ClickEvent>(CheckCallback, _handlers, Config);
            root.Q<Button>("config-open").RegisterEvent<ClickEvent>(CheckCallback, _handlers, Config);
        }

        private void CheckCallback(ClickEvent clickEvent)
        {
            _handlers[clickEvent.target].Invoke();
        }
        
        private void OpenLogs() => OpenFile(Paths.Options);
        private void Config() => OpenFile(Paths.Options);
        private void OpenPersistent() => EditorUtility.RevealInFinder(Application.persistentDataPath);
    
        private void OpenFile(string path)
        {
            _startInfo.FileName = path;
            Process.Start(_startInfo);
        }
    }
}