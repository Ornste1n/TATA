using System;
using UnityEngine;
using NaughtyAttributes;
using Game.Scripts.Extension.Mono;

namespace Game.Scripts.Mechanics.Units.Builder
{
    [CreateAssetMenu(menuName = "Units/Create Unit Skill")]
    public class UnitSkill : ScriptableObject
    {
        [SerializeField] private SerializableMonoScript<SkillBase> _skill;
        
        [ShowAssetPreview]
        [SerializeField] private Sprite _icon;
    }
}