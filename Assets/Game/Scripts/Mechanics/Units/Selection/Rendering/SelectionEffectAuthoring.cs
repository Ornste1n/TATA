using Game.Scripts.Extension.Meshes;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts.Mechanics.Units.Selection.Rendering
{
    public class SelectionEffectAuthoring : MonoBehaviour
    {
        [Header("Selection")]
        [SerializeField] private Material _selectionMaterial;
        [SerializeField] private float _selectionHeight;
        [SerializeField] private float _selectionWidth;

        [Header("Hover")]
        [SerializeField] private Material _hoverMaterial;
        [SerializeField] private float _hoverHeight;
        [SerializeField] private float _hoverWidth;
        
        [Header("Position")]
        [SerializeField] private float m_YOffset;
        [SerializeField] private float m_XOffset;
        
        public class SelectionEffectAuthoringBaker : Baker<SelectionEffectAuthoring>
        {
            public override void Bake(SelectionEffectAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SelectionEffectData()
                {
                    Material = authoring._selectionMaterial,
                    Offset = new float2(authoring.m_XOffset, authoring.m_YOffset),
                    Size = new float2(authoring._selectionWidth, authoring._selectionHeight),
                    Mesh = MeshUtility.CreateQuadMesh(authoring._selectionWidth, authoring._selectionHeight),
                });
                AddComponent(entity, new HoverEffectData()
                {
                    Material = authoring._hoverMaterial,
                    Offset = new float2(authoring.m_XOffset, authoring.m_YOffset),
                    Size = new float2(authoring._hoverWidth, authoring._hoverHeight),
                    Mesh = MeshUtility.CreateQuadMesh(authoring._hoverWidth, authoring._hoverHeight),
                });
            }
        }
    }
    
    public struct HoverEffectData : IComponentData
    {
        public float2 Size;
        public float2 Offset;
        public UnityObjectRef<Mesh> Mesh;
        public UnityObjectRef<Material> Material;
    }
    
    public struct SelectionEffectData : IComponentData
    {
        public float2 Size;
        public float2 Offset;
        public UnityObjectRef<Mesh> Mesh;
        public UnityObjectRef<Material> Material;
    }
}