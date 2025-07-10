using Unity.Entities;

namespace Game.Scripts.Mechanics.Units.General.Components
{
    public struct Damageable : IComponentData
    {
        public readonly float Health;
        public readonly float MaxHealth;

        public Damageable(float health)
        {
            Health = health;
            MaxHealth = health;
        }
    }

    public struct UnitId : IComponentData
    {
        public readonly uint Id;

        public UnitId(uint id) => Id = id;
    }
}