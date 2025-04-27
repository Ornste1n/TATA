using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using UnityEngine.Localization;
using System.Collections.Generic;
using Game.Scripts.UI.Options.Base;
using UnityEngine.Localization.Tables;
using AYellowpaper.SerializedCollections;

namespace Game.Scripts.UI.Options
{
    public class OptionsWindow : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private LocalizedStringTable _optionTable;
        [Space]
        [SerializedDictionary("Screen Mode", "Localization Name")]
        public SerializedDictionary<FullScreenMode, TableEntryReference> _screenModesNames;
        [Space]
        [SerializedDictionary("Quality", "Localization Name")]
        public SerializedDictionary<QualityLevel, TableEntryReference> _qualityLevel;
        [Space] 
        [SerializeField] private TableEntryReference _customQualityReference;

        private Button _backButton;
        private Button _applyButton;
        private Button _resetButton;

        private StringTable _options;
        private VisualElement m_Panel;
        private Button _openOptionButton;

        private Option _current;
        private Option _generalOption;
        
        private readonly Dictionary<IEventHandler, Option> _panels = new(5);
        
        #region const
        private const string Hidden = "hidden";
        private const string ActiveStyle = "activeOptionButton";
        #endregion
        
        public event Action<bool> OnChangeActive;
        public VisualElement Root => _uiDocument.rootVisualElement;

        #region Localize
        public string CustomLocalLabel { get; private set; }
        public Dictionary<string, QualityLevel> QualityLocalize { get; private set; }
        public Dictionary<string, FullScreenMode> ScreenModeLocalize { get; private set; }
        #endregion

        private void Awake()
        {
            Initialize();
        }

        private void Open(ClickEvent _)
        {
            OnChangeActive?.Invoke(true);
            m_Panel?.RemoveFromClassList(Hidden);
        }
        
        private async void Initialize()
        {
            m_Panel = Root.Q<VisualElement>("OptionsTemplate");
                
            await LoadLocalizeTable();
            
            InitNavigation();
            InitOptionButtons();
        }

        private void InitOptionButtons()
        {
            _generalOption = new GeneralPanel(this, "general-content", "general");
            
            RegisterPanel(_generalOption);
            RegisterPanel(new GraphicsPanel(this, "graphics-content", "graphics"));
            RegisterPanel(new AudioPanel(this, "audio-content", "audio"));
            
            _generalOption.Button.AddToClassList(ActiveStyle);
            _current = _generalOption;
        }

        private void RegisterPanel(Option option)
        {
            _panels.Add(option.Button, option);
            option.Button.RegisterCallback<ClickEvent>(ChangeOptionPanel);
        }
        
        private void ChangeOptionPanel(ClickEvent clickEvent)
        {
            _current.Panel.AddToClassList(Hidden);
            _current.Button.RemoveFromClassList(ActiveStyle);
            
            _current = _panels[clickEvent.currentTarget];
            
            _current.Panel.RemoveFromClassList(Hidden);
            _current.Button.AddToClassList(ActiveStyle);
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

            QualityLocalize = new Dictionary<string, QualityLevel>(3);
            
            foreach (var (quality, reference) in _qualityLevel)
            {
                string local = _options.GetEntry(reference.Key)?.GetLocalizedString();
                
                if (local == null) continue;
                
                QualityLocalize.Add(local, quality);
            }
            
            _qualityLevel.Clear();
            _qualityLevel = null;
            
            CustomLocalLabel = _options.GetEntry(_customQualityReference.Key)?.GetLocalizedString();
        }

        private void SaveChanges(ClickEvent _)
        {
            foreach (Option panel in _panels.Values)
            {
                if(!panel.HasChanged()) continue;
                
                panel.Save();
            }
        }

        private void ResetPanel(ClickEvent _)
        {
            _current?.Reset();
        }
        
        private void InitNavigation()
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
            m_Panel?.AddToClassList(Hidden);
        }
        
        public void Dispose()
        {
            foreach (var option in _panels.Values)
                option.Button.UnregisterCallback<ClickEvent>(ChangeOptionPanel);
            
            _openOptionButton.UnregisterCallback<ClickEvent>(Open);
            _resetButton.UnregisterCallback<ClickEvent>(ResetPanel);
            _backButton.UnregisterCallback<ClickEvent>(CloseWindow);
            _applyButton.UnregisterCallback<ClickEvent>(SaveChanges);
        }
    }
}
