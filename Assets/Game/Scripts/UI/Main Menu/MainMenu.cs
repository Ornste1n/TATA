using UnityEngine;
using UnityEngine.UIElements;
using Game.Scripts.UI.Options;

namespace Game.Scripts.UI.Frontend.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private OptionsWindow _optionsWindow;

        private VisualElement _navigationBar;
        
        private void Awake()
        {
            VisualElement root = _uiDocument.rootVisualElement;
            _navigationBar = root.Q<VisualElement>("navigation-bar");
            
            Button continueButton = root.Q<Button>("continue");
            Button campaignButton = root.Q<Button>("campaign");
            Button quitButton = root.Q<Button>("quit");

            _optionsWindow.OnChangeActive += SwitchOption;
        }

        private void SwitchOption(bool otherActiveState)
        {
            if (otherActiveState)
                _navigationBar.AddToClassList("hidden");
            else
                _navigationBar.RemoveFromClassList("hidden");
        }

        private void OnDestroy()
        {
            _optionsWindow.OnChangeActive -= SwitchOption;

            _optionsWindow.Dispose();
        }
    }
}
