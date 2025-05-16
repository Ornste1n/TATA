using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Scripts.Mechanics.Units.General.Rendering
{
    public class HealthBarAuthoring : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private Material m_Material;
        [SerializeField] private float m_Height;
        [SerializeField] private float m_Width;
        [SerializeField] private float m_ScaleFactor;

        [Header("Position")]
        [SerializeField] private float m_YOffset;
        [SerializeField] private float m_XOffset;

        public class HealthBarAuthoringBaker : Baker<HealthBarAuthoring>
        {
            public override void Bake(HealthBarAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new HealthBarComponent()
                {
                    ScaleFactor = authoring.m_ScaleFactor,
                    HealthMaterial = authoring.m_Material,
                    Size = new float2(authoring.m_Width,authoring.m_Height),
                    Offset = new float2(authoring.m_XOffset, authoring.m_YOffset),
                });
            }
        }
    }

    public struct HealthBarComponent : IComponentData
    {
        public float2 Size;
        public float2 Offset;
        public float ScaleFactor;
        public UnityObjectRef<Material> HealthMaterial;
    }
}