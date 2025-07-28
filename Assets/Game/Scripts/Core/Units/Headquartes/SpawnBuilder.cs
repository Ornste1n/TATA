using UnityEngine;
using Unity.Physics;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;
using Game.Scripts.Inputs;
using System.Collections.Generic;
using Game.Scripts.Mechanics.Units.Builder;
using Game.Scripts.Mechanics.Units.General.Components;
using Game.Scripts.Mechanics.Units.General.Rendering.Skills;

namespace Game.Scripts.Core.Units.Headquartes
{
    public class SpawnBuilder : SkillBase
    {
        private const uint UnitId = 1;
        
        private Entity _entity;
        private PhysicsCollider _physicsCollider;
        private List<(Mesh Mesh, Matrix4x4 Matrix)> _meshes = new(16);

        private PlayerInputSystem _inputSystem;
        private UnitSpawnRendererSystem _spawnSystem;
        
        public override void Initialize()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            
            _inputSystem = world.GetExistingSystemManaged<PlayerInputSystem>();
            _spawnSystem = world.GetExistingSystemManaged<UnitSpawnRendererSystem>();
            
            FoundNeededEntity(world);
        }

        public override void Execute()
        {
            _inputSystem.OnMouseClick += TrySpawnEnemy;
            _spawnSystem.Start(_meshes, _physicsCollider);
        }

        private void TrySpawnEnemy(float3 worldMouse)
        {
            if(_spawnSystem.Obstacle) return;
            
            _spawnSystem.Enabled = false;
            _inputSystem.OnMouseClick -= TrySpawnEnemy;
            
            EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
            Entity newBuild = em.Instantiate(_entity);

            if (em.HasComponent<LocalTransform>(newBuild))
            {
                LocalTransform transform = em.GetComponentData<LocalTransform>(newBuild);
                em.SetComponentData(newBuild, transform.WithPosition(worldMouse));
            }
            else
            {
                em.AddComponentData(newBuild, LocalTransform.FromPosition(worldMouse));
            }
        }

        private void FoundNeededEntity(World world)
        {
            Entity entity = default;
            EntityManager em = world.EntityManager;
            EntityQuery bufferQuery = em.CreateEntityQuery(typeof(UnitPrefabReference));
            Entity bufferEntity = bufferQuery.GetSingletonEntity();

            EntityQuery blobQuery = em.CreateEntityQuery(typeof(UnitsCatalogBlobRef));
            Entity blobEntity = blobQuery.GetSingletonEntity();            
            
            UnitsCatalogBlobRef blobs = em.GetComponentData<UnitsCatalogBlobRef>(blobEntity);
            DynamicBuffer<UnitPrefabReference> unitsPrefabs = em.GetBuffer<UnitPrefabReference>(bufferEntity);

            ref BlobArray<UnitBlob> units = ref blobs.Catalog.Value.Units;
            
            int i = 0;
            
            while (i < units.Length)
            {
                if (units[i].Id == UnitId)
                {
                    entity = unitsPrefabs[units[i].BuffersIndex].Prefab;
                    break;
                }

                i++;
            }

            _entity = entity;

            DynamicBuffer<LinkedEntityGroup> linkedGroup = em.GetBuffer<LinkedEntityGroup>(entity);

            List<(Mesh Mesh, Matrix4x4 Matrix)> meshes = _meshes;
            
            if(meshes.Count != 0)
                meshes.Clear();
            
            foreach (LinkedEntityGroup entityGroup in linkedGroup)
            {
                Entity child = entityGroup.Value;
                if (!em.HasComponent<RenderMeshArray>(child) || !em.HasComponent<MaterialMeshInfo>(child)) continue;
                
                RenderMeshArray meshArray = em.GetSharedComponentManaged<RenderMeshArray>(child);
                MaterialMeshInfo meshInfo = em.GetComponentData<MaterialMeshInfo>(child);
                
                Mesh mesh = meshArray.GetMesh(meshInfo);
                
                LocalTransform transform = em.GetComponentData<LocalTransform>(child);
                Matrix4x4 matrix4X4 = Matrix4x4.TRS(transform.Position, transform.Rotation, Vector3.one * transform.Scale);
                meshes.Add((mesh, matrix4X4));
            }

            _meshes = meshes;
            _physicsCollider = em.GetComponentData<PhysicsCollider>(entity);
        }
        
        public override void Dispose()
        {
            _inputSystem.OnMouseClick -= TrySpawnEnemy;
            _meshes.Clear();
        }
    }
}