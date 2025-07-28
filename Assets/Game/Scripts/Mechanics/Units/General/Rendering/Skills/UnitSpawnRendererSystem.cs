using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using Unity.Physics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using System.Collections.Generic;
using Game.Scripts.WorldSettings;
using Game.Scripts.Inputs.Components;
using Material = UnityEngine.Material;

namespace Game.Scripts.Mechanics.Units.General.Rendering.Skills
{
    [UpdateInGroup(typeof(RenderingUnitsEffectsSystem))]
    public partial class UnitSpawnRendererSystem : SystemBase
    {
        private PhysicsCollider _physicsCollider;
        private List<(Mesh Mesh, Matrix4x4 Matrix)> _meshes;
        
        private Color _default;
        private Color _invalid;
        private RenderParams _renderParams;
        private Material _rendererMaterial;
        private MaterialPropertyBlock _propertyBlock;
        private static readonly int s_color = Shader.PropertyToID("_Color");

        public bool Obstacle { get; private set; }
        
        protected override void OnCreate()
        {
            Enabled = false;
            RequireForUpdate<MouseInputComponent>();
            RequireForUpdate<UnitSpawnRendererData>();
        }

        public void Start(List<(Mesh Mesh, Matrix4x4 Matrix)> meshes, PhysicsCollider collider)
        {
            UnitSpawnRendererData data = SystemAPI.GetSingleton<UnitSpawnRendererData>();
            _rendererMaterial = data.Material;
            _physicsCollider = collider;

            _default = _rendererMaterial.color;
            _invalid = Color.red;
            _invalid.a = _default.a;
            
            _meshes = meshes;
            _propertyBlock = new MaterialPropertyBlock();
            _renderParams = new RenderParams(_rendererMaterial) { matProps = _propertyBlock};
            
            Enabled = true;
        }
        
        protected override unsafe void OnUpdate()
        {
            MouseInputComponent mouse = SystemAPI.GetSingleton<MouseInputComponent>();
            CollisionWorld world = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            float3 end = mouse.WorldMousePosition;
            float3 start = end;
            start.y += 10;
            end.y += 0.5f;
            
            ColliderCastInput input = new (_physicsCollider.Value, start, end);
            NativeReference<bool> canSpawn = new (Allocator.TempJob);

            Aabb bounds = _physicsCollider.ColliderPtr -> CalculateAabb(new RigidTransform(quaternion.identity,end));

            CheckSpaceAvailable job = new()
            {
                OffsetY = 2f,
                Aabb = bounds,
                CanSpawn = canSpawn,
                CollisionWorld = world,
                ColliderCastInput = input,
                CollisionFilter = new CollisionFilter()
                {
                    GroupIndex = 0,
                    BelongsTo = ~0u,
                    CollidesWith = 1u << (int)WorldLayers.Ground
                }
            };
            
            job.Schedule().Complete();

            _propertyBlock.SetColor(s_color, !canSpawn.Value ? _invalid : _default);
            
            Matrix4x4 matrix4X4 = Matrix4x4.TRS(mouse.WorldMousePosition, Quaternion.identity, Vector3.one);
            
            foreach ((UnityObjectRef<Mesh> mesh, Matrix4x4 localMatrix) in _meshes)
            {
                Matrix4x4 finalMatrix = matrix4X4 * localMatrix;
                Graphics.RenderMesh(_renderParams, mesh, 0, finalMatrix);
            }

            Obstacle = !canSpawn.Value;
            canSpawn.Dispose();
        }
    }

    [BurstCompile]
    public struct CheckSpaceAvailable : IJob
    {
        public CollisionWorld CollisionWorld;
        public NativeReference<bool> CanSpawn;

        public Aabb Aabb;
        public float OffsetY;
        public CollisionFilter CollisionFilter;
        public ColliderCastInput ColliderCastInput;
        
        [BurstCompile]
        public void Execute()
        {
            float3 leftBottom = Aabb.Min;
            float3 rightTop = new (Aabb.Max.x, leftBottom.y, Aabb.Max.z);

            float3 rightBottom = new (rightTop.x, leftBottom.y, leftBottom.z);
            float3 leftTop = new (leftBottom.x, rightTop.y, rightTop.z);
            
            if (HasHit(leftBottom) && HasHit(rightTop) &&
                HasHit(rightBottom) && HasHit(leftTop) &&
                !CollisionWorld.CastCollider(ColliderCastInput))
            {
                CanSpawn.Value = true;
                return;
            }

            CanSpawn.Value = false;
        }
        
        [BurstCompile]
        private bool HasHit(float3 start)
        {
            RaycastInput input = new()
            {
                Start = start,
                End = start - new float3(0, OffsetY, 0),
                Filter = CollisionFilter
            };
            return CollisionWorld.CastRay(input);
        }
    }
}