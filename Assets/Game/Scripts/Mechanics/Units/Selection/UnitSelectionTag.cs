using Unity.Entities;

namespace Game.Scripts.Mechanics.Units.Selection
{
    public struct UnitSelectionTag : IComponentData
    {
        public int Group;
    }

    public struct UnitsSelectedEvent : IComponentData
    {
        public bool HasSelected;
    }
    
    public partial class SelectionSystemGroup : ComponentSystemGroup { }
}