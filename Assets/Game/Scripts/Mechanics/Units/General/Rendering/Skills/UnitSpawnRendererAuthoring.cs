using UnityEngine;
using Unity.Entities;

namespace Game.Scripts.Mechanics.Units.General.Rendering.Skills
{
    public class UnitSpawnRendererAuthoring : MonoBehaviour
    {
        [SerializeField] private Material _material;
        
        public class UnitSpawnRendererAuthoringBaker : Baker<UnitSpawnRendererAuthoring>
        {
            public override void Bake(UnitSpawnRendererAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitSpawnRendererData { Material = authoring._material });
            }
        }
    }

    public struct UnitSpawnRendererData : IComponentData
    {
        public UnityObjectRef<Material> Material;
    }
}