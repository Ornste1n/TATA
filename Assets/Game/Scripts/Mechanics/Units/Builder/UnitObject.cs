using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Localization.Tables;

namespace Game.Scripts.Mechanics.Units.Builder
{
    [CreateAssetMenu(menuName = "Units/Create Unit")]
    public class UnitObject : ScriptableObject
    {
        [SerializeField] private TableEntryReference _nameReference;
        [SerializeField] private uint _id;
        
        [ShowAssetPreview]
        [SerializeField] private Sprite _icon;

        [BoxGroup("Main")] 
        [ProgressBar("Health", 1000, EColor.Red)]
        [SerializeField, Range(0, 1000)] private int _health;

        [BoxGroup("Skills")]
        [SerializeField] private List<UnitSkill> _skills = new();
        
        public uint Id => _id;
        public Sprite Icon => _icon;
        public TableEntryReference NameRef => _nameReference;
        
        
        #region Main

        public int Health => _health;
        

        #endregion
    }
}