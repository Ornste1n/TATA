using Unity.Entities;

namespace Game.Scripts.Mechanics.Units.General.Components
{
    public struct Damageable : IComponentData
    {
        public float Health;
        public float MaxHealth;
    }
}