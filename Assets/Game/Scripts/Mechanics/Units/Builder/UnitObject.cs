using UnityEngine;
using CustomInspector;
using System.Collections.Generic;
using UnityEngine.Localization.Tables;

namespace Game.Scripts.Mechanics.Units.Builder
{
    [CreateAssetMenu(menuName = "Units/Create Unit")]
    public class UnitObject : ScriptableObject
    {
        [Preview, Tab("Main"), SerializeField] 
        private Sprite _icon;
        
        [Tab("Main"), GUIColor(FixedColor.Cyan), LabelSettings(LabelStyle.NoSpacing), SerializeField] 
        private uint _id;
        
        [Tab("Main"), SerializeField] 
        private TableEntryReference _nameReference;

        [Tab("Main"), SerializeField]
        private GameObject _prefab;
        
        [Tab("Stats"), BackgroundColor(FixedColor.CherryRed), SerializeField]
        private int _health;
        
        [Tab("Skills"), ListContainer, SerializeField]
        private ListContainer<UnitSkill> _skills = new();
        
        public uint Id => _id;
        public Sprite Icon => _icon;
        public GameObject Prefab => _prefab;
        public TableEntryReference NameRef => _nameReference;
        
        
        #region Main

        public int Health => _health;
        public IReadOnlyList<UnitSkill> Skills => _skills;

        #endregion
    }
}