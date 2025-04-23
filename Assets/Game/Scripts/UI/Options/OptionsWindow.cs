using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Localization.Tables;
using AYellowpaper.SerializedCollections;
using Game.Scripts.UI.Options.Base;

namespace Game.Scripts.UI.Options
{
    public class OptionsWindow : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private LocalizedStringTable _optionTable;
        [Space]
        [SerializedDictionary("Screen Mode", "Localization Name")]
        public SerializedDictionary<FullScreenMode, TableEntryReference> _screenModesNames;

        private Button _backButton;
        private Button _applyButton;
        private Button _resetButton;

        private StringTable _options;
        private VisualElement m_Panel;
        private Button _openOptionButton;

        private Option _current;
        private List<Option> _panels;
        
        public event Action<bool> OnChangeActive;
        public VisualElement Root => _uiDocument.rootVisualElement;
        public Dictionary<string, FullScreenMode> ScreenModeLocalize { get; private set; }

        private void Awake()
        {
            Initialize();
        }

        private void Open(ClickEvent _)
        {
            OnChangeActive?.Invoke(true);
            m_Panel?.RemoveFromClassList("hidden");
        }
        
        private async void Initialize()
        {
            m_Panel = Root.Q<VisualElement>("OptionsTemplate");
                
            RegisterButtons();
            await LoadLocalizeTable();

            _panels = new List<Option>(5)
            {
                new GraphicsPanel(this)
            };
        }

        private async Task LoadLocalizeTable()
        {
            _options = await _optionTable.GetTableAsync().Task;

            ScreenModeLocalize = new Dictionary<string, FullScreenMode>(3);
            
            foreach (var (fullScreenMode, reference) in _screenModesNames)
            {
                string local = _options.GetEntry(reference.Key)?.GetLocalizedString();
                
                if (local == null) continue;
                
                ScreenModeLocalize.Add(local, fullScreenMode);
            }
            
            _screenModesNames.Clear();
            _screenModesNames = null;
        }

        private void SaveChanges(ClickEvent _)
        {
            foreach (Option panel in _panels)
            {
                if(!panel.HasChanged()) continue;
                
                panel.Save();
            }
        }

        private void ResetPanel(ClickEvent _)
        {
            _current?.Reset();
        }
        
        private void RegisterButtons()
        {
            _openOptionButton = Root.Q<Button>("options");
            
            _backButton = Root.Q<Button>("back");
            _applyButton = Root.Q<Button>("apply");
            _resetButton = Root.Q<Button>("reset");
            
            _openOptionButton.RegisterCallback<ClickEvent>(Open);
            
            _resetButton.RegisterCallback<ClickEvent>(ResetPanel);
            _backButton.RegisterCallback<ClickEvent>(CloseWindow);
            _applyButton.RegisterCallback<ClickEvent>(SaveChanges);
        }
        

        private void CloseWindow(ClickEvent _)
        {
            OnChangeActive?.Invoke(false);
            m_Panel?.AddToClassList("hidden");
        }
        
        public void Dispose()
        {
            _openOptionButton.UnregisterCallback<ClickEvent>(Open);
            _resetButton.UnregisterCallback<ClickEvent>(ResetPanel);
            _backButton.UnregisterCallback<ClickEvent>(CloseWindow);
            _applyButton.UnregisterCallback<ClickEvent>(SaveChanges);
        }
    }
}
