using System.Threading;
using Unity.Entities;
using Game.Scripts.Inputs;
using Game.Scripts.Scenes;
using Game.Scripts.UI.Options;
using UnityEngine.UIElements;

namespace Game.Scripts.UI.Gameplay
{
    public class GameMenu
    {
        private VisualElement _options;
        private VisualElement _gameMenu;
        private VisualElement _pauseMenu;

        private OptionsWindow _optionsWindow;
        private PlayerInputSystem _inputSystem;

        private const string Hidden = "hidden";
        
        #region Buttons
        private Button _saveButton;
        private Button _loadButton;
        private Button _quitButton;
        private Button _continueButton;
        
        #endregion

        public GameMenu(UIDocument document)
        {
            InitializeUI(document);
            
            _inputSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerInputSystem>();
            _inputSystem.OnPaused += MenuEnable;
        }

        private void InitializeUI(UIDocument document)
        {
            VisualElement root = document.rootVisualElement;
            _options = root.Q<VisualElement>("options");
            _gameMenu = root.Q<VisualElement>("game-menu");
            _pauseMenu = root.Q<VisualElement>("pause-menu");
            
            RegisterMenuButtons();
            
            _optionsWindow = document.gameObject.GetComponent<OptionsWindow>();
            _optionsWindow.OnChangeActive += SetActiveOptions;
        }

        private void SetActiveOptions(bool active)
        {
            if (active)
                _pauseMenu.AddToClassList(Hidden);
            else
                _pauseMenu.RemoveFromClassList(Hidden);
        }

        private void RegisterMenuButtons()
        {
            VisualElement root = _pauseMenu;
            _saveButton = root.Q<Button>("save");
            _loadButton = root.Q<Button>("load");
            _quitButton = root.Q<Button>("quit");
            _continueButton = root.Q<Button>("continue");

            _quitButton.clicked += Quit;
            _continueButton.clicked += MenuDisable;
        }

        private async void Quit()
        {
            _quitButton.clicked -= Quit;
            await SceneSwitcher.LoadMenuSceneAsync(new CancellationToken());
        }

        private void MenuDisable() => _gameMenu.AddToClassList(Hidden);
        private void MenuEnable() => _gameMenu.RemoveFromClassList(Hidden);

        public void Disable()
        {
            _quitButton.clicked -= Quit;
            _inputSystem.OnPaused -= MenuEnable;
            _continueButton.clicked -= MenuDisable;
            _optionsWindow.OnChangeActive -= SetActiveOptions;
        }
    }
}