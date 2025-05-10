using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Game.Scripts.UI.Options;
using UnityEngine.Localization.Settings;

namespace Game.Scripts.UI.Frontend.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private OptionsWindow _optionsWindow;

        private Button _quitButton;
        private Button _continueButton;
        private Button _campaignButton;
        private VisualElement _navigationBar;

        private void Awake()
        {
            VisualElement root = _uiDocument.rootVisualElement;
            _navigationBar = root.Q<VisualElement>("navigation-bar");

            _continueButton = root.Q<Button>("continue");
            _campaignButton = root.Q<Button>("campaign");
            _quitButton = root.Q<Button>("quit");
            
            _quitButton.clicked += Quit;
            _optionsWindow.OnChangeActive += SwitchOption;
        }

        private void SwitchOption(bool otherActiveState)
        {
            if (otherActiveState)
                _navigationBar.AddToClassList("hidden");
            else
                _navigationBar.RemoveFromClassList("hidden");
        }

        private void Quit() => Application.Quit();

        private void OnDestroy()
        {
            _quitButton.clicked -= Quit;
            _optionsWindow.OnChangeActive -= SwitchOption;

            _optionsWindow.Dispose();
        }
    }
}
