using Unity.Entities;
using Unity.Transforms;
using Game.Scripts.Mechanics.Units.General.Components;

namespace Game.Scripts.Mechanics.Units.General
{
    public readonly partial struct UnitAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRO<UnitId> m_UnitId;
        private readonly RefRW<Damageable> m_Damageable;
        private readonly RefRW<LocalTransform> m_Transform;

        public uint Id => m_UnitId.ValueRO.Id;
        public Damageable Damageable => m_Damageable.ValueRO;
        public LocalTransform Transform => m_Transform.ValueRO;
    }
}