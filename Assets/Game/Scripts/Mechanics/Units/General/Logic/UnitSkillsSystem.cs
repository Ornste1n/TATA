using Unity.Entities;
using Game.Scripts.Mechanics.Units.General.Components;
using Game.Scripts.Mechanics.Units.Selection.UnitsHud;

namespace Game.Scripts.Mechanics.Units.General.Logic
{
    [UpdateInGroup(typeof(InitializeUnitsSystemGroup))]
    public partial class UnitSkillsSystem : SystemBase
    {
        private SkillsArray _skillsArray;
        private UnitsPanelSystem _panelSystem;
        
        protected override void OnUpdate()
        {
            _panelSystem = World.GetExistingSystemManaged<UnitsPanelSystem>();
            _panelSystem.TriedUseSkill += TryUseSkill;
            Enabled = false;
        }
        
        private void TryUseSkill(int index)
        {
            SkillsArray array = SystemAPI.ManagedAPI.GetSingleton<SkillsArray>();
            array.Skills[index].Execute();
        }

        protected override void OnDestroy()
        {
            _panelSystem.TriedUseSkill -= TryUseSkill;
        }
    }
}