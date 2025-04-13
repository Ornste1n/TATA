using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Scripts.UI.Frontend.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        private const string ContinueButton = "continue";
        private const string CampaignButton = "campaign";
        private const string OptionsButton = "options";
        private const string QuitButton = "quit";
        
        [SerializeField] private UIDocument _uiDocument;

        private VisualElement _optionsPanel;
        private VisualElement _navigationBar;
        
        private void Awake()
        {
            VisualElement root = _uiDocument.rootVisualElement;
            
            _optionsPanel = root.Q<VisualElement>("options-window");
            _navigationBar = root.Q<VisualElement>("navigation-bar");
            
            Button continueButton = root.Q<Button>(ContinueButton);
            Button campaignButton = root.Q<Button>(CampaignButton);
            Button optionsButton = root.Q<Button>(OptionsButton);
            Button quitButton = root.Q<Button>(QuitButton);
            
            optionsButton.RegisterCallback<ClickEvent>(ShowOptions);
        }

        private void ShowOptions(ClickEvent evt)
        {
            _navigationBar.AddToClassList("hidden");
            _optionsPanel.RemoveFromClassList("hidden");
        }
    }
}
