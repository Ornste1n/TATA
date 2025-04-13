using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Options
{
    [Serializable]
    public abstract class OptionPanel
    {
        [SerializeField] private Button _navigationButton;
        
        public Button Button => _navigationButton;
    }
}
