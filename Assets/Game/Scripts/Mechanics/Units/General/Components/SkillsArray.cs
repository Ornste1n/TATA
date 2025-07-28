using System;
using Unity.Entities;
using System.Collections.Generic;
using Game.Scripts.Mechanics.Units.Builder;

namespace Game.Scripts.Mechanics.Units.General.Components
{
    public class SkillsArray : IComponentData
    {
        public IReadOnlyList<SkillBase> Skills;
    }
}