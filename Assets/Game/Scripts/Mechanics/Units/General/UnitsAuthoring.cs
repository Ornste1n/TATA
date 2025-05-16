using UnityEngine;
using Unity.Entities;
using Game.Scripts.Mechanics.Units.General.Components;
using Game.Scripts.Mechanics.Units.Selection;

namespace Game.Scripts.Mechanics.Units.General
{
    public class UnitsAuthoring : MonoBehaviour
    {
        [SerializeField] private int m_Health;
        
        public class UnitsAuthoringBaker : Baker<UnitsAuthoring>
        {
            public override void Bake(UnitsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent(entity, new Damageable()
                {
                    Health = authoring.m_Health,
                    MaxHealth = authoring.m_Health
                });

                AddComponent(entity, new UnitSelectionTag());
            }
        }
    }
}