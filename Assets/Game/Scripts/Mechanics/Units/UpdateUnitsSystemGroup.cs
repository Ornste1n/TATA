using Unity.Entities;

namespace Game.Scripts.Mechanics.Units
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class InitializeUnitsSystemGroup : ComponentSystemGroup { }
    
    public partial class UpdateUnitsSystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(UpdateUnitsSystemGroup))]
    public partial class UpdateSelectionSystem : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class RenderingUnitsEffectsSystem : ComponentSystemGroup { }
}