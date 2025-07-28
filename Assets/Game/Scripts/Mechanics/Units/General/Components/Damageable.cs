using Unity.Entities;
using Game.Scripts.SaveSystems.Components;

namespace Game.Scripts.Mechanics.Units.General.Components
{
    [Save]
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

    public struct UnitPrefabReference : IBufferElementData
    {
        public Entity Prefab;
    }
}