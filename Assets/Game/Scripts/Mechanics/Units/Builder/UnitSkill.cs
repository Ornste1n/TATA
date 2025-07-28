using System;
using UnityEngine;
using CustomInspector;
using Game.Scripts.Extension.Mono;

namespace Game.Scripts.Mechanics.Units.Builder
{
    [CreateAssetMenu(menuName = "Units/Create Unit Skill")]
    public class UnitSkill : ScriptableObject
    {
        [Preview]
        [SerializeField] private Sprite _icon;

        [SerializeField] private SerializableMonoScript<SkillBase> script;
        
        public Sprite Icon => _icon;
        public Type SkillType => script.Type;
    }
    
}