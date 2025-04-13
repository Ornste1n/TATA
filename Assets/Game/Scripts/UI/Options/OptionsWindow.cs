using UnityEngine;
using System.Collections.Generic;

namespace Game.Scripts.UI.Options
{
    public class OptionsWindow : MonoBehaviour
    {
        [SerializeField] private List<OptionPanel> _optionPanels = new();

        private OptionPanel _activePanel;
        
        private void Awake()
        {
        }

        private void SwitchPanel(OptionPanel panel)
        {
            
        }
        
        private void OnDestroy()
        {
            
        }
    }
}