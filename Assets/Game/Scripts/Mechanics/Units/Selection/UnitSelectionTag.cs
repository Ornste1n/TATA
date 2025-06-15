using Unity.Entities;

namespace Game.Scripts.Mechanics.Units.Selection
{
    public struct LastSelectedUnit : IComponentData
    {
        public Entity Value;
    }
    
    public struct UnitsHoverTag : IComponentData { }
    
    public struct UnitSelectionTag : IComponentData
    {
        public int Group;
    }

    public struct UnitsSelectedEvent : IComponentData
    {
        public bool HasSelected;
    }
}