using UnityEngine;
using Unity.Entities;
using Game.Scripts.Mechanics.Units.Builder;
using Game.Scripts.Mechanics.Units.General.Components;

namespace Game.Scripts.Mechanics.Units.General
{
    public class UnitAuthoring : MonoBehaviour
    {
        [SerializeField] private UnitObject _unitObject;
        
        public class UnitsAuthoringBaker : Baker<UnitAuthoring>
        {
            public override void Bake(UnitAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                UnitObject unitObject = authoring._unitObject;
                
                AddComponent(entity, new UnitId(unitObject.Id));
                AddComponent(entity, new Damageable(unitObject.Health));
            }
        }
    }
}