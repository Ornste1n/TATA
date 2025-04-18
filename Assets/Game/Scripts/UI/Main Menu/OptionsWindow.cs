using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Scripts.UI.Frontend.MainMenu
{
    public class OptionsWindow : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        
        private VisualElement Root => _uiDocument.rootVisualElement;

        private void Start()
        {
            ResolutionDropdown();
        }

        private void ResolutionDropdown()
        {
            Resolution[] resolutions = Screen.resolutions;
            Resolution current = Screen.currentResolution;
            
            DropdownField dropdown = Root.Q<DropdownField>("resolution");
            dropdown.value = $"{current.width} x {current.height} ({current.refreshRateRatio})Hz";
            dropdown.choices = new List<string>(resolutions.Length);
            
            for (var i = resolutions.Length - 1; i >= 0; i--)
            {
                Resolution res = resolutions[i];
                dropdown.choices.Add($"{res.width} x {res.height} ({res.refreshRateRatio}Hz)");
            }
        }
    }
}
