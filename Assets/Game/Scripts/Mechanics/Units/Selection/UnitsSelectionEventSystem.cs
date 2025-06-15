using Unity.Jobs;
using Unity.Burst;
using Unity.Physics;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using IJob = Unity.Jobs.IJob;
using Game.Scripts.Inputs.Components;
using Collider = Unity.Physics.Collider;
using RaycastHit = Unity.Physics.RaycastHit;
using ConvexCollider = Unity.Physics.ConvexCollider;

namespace Game.Scripts.Mechanics.Units.Selection
{
    [UpdateInGroup(typeof(UpdateSelectionSystem))]
    public partial struct UnitsSelectionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Entity singleton = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(singleton, new LastSelectedUnit { Value = Entity.Null });
            
            state.RequireForUpdate<LastSelectedUnit>();
            state.RequireForUpdate<CameraInputComponent>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            CameraInputComponent input = SystemAPI.GetSingleton<CameraInputComponent>();
            CollisionWorld physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            float3 startPos = input.WorldMousePosition + new float3(0, 10, 0);
            float3 endPos = input.WorldMousePosition;

            RaycastInput rayInput = new()
            {
                End = endPos,
                Start = startPos,
                Filter = CollisionFilter.Default
            };

            Entity selSingleton = SystemAPI.GetSingletonEntity<LastSelectedUnit>();
            Entity lastSelected = SystemAPI.GetComponent<LastSelectedUnit>(selSingleton).Value;

            EndSimulationEntityCommandBufferSystem ecbSystem = state.World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
            EntityCommandBuffer.ParallelWriter ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

            JobHandle jobHandle = new RaycastSelectJob()
            {
                PhysicsWorld = physicsWorld,
                Ray = rayInput,
                Ecb = ecb,
                LastSelected = lastSelected,
                SingletonEntity = selSingleton
            }.Schedule(state.Dependency);

            ecbSystem.AddJobHandleForProducer(jobHandle);
            state.Dependency = jobHandle;
        }
    }
    
    [UpdateInGroup(typeof(UpdateSelectionSystem))]
    [BurstCompile]
    public partial struct UnitsSelectionEventSystem : ISystem
    {
        private Entity _selectedEvent;
        
        public void OnCreate(ref SystemState state)
        {
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAny<EndDragEvent, ClickMouseEvent>();
            
            EntityQuery query = state.GetEntityQuery(builder);

            _selectedEvent = state.EntityManager.CreateEntity();
            
            state.RequireAnyForUpdate(query);
            state.RequireForUpdate<InputData>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Entity entity = SystemAPI.GetSingletonEntity<InputData>();
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
            
            CollisionWorld collisions = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            NativeList<Entity> entitiesToDeselect = new NativeList<Entity>(16, Allocator.TempJob);
            
            NativeArray<Entity> unitsSelected = SystemAPI.QueryBuilder()
                .WithAll<UnitSelectionTag>()
                .Build().ToEntityArray(Allocator.TempJob);

            if (SystemAPI.HasComponent<EndDragEvent>(entity))
            {
                NativeList<Entity> entitiesToSelect = default;
                
                EndDragEvent data = SystemAPI.GetComponentRO<EndDragEvent>(entity).ValueRO;
                
                NativeArray<float3> points = new NativeArray<float3>(4, Allocator.TempJob)
                {
                    [0] = data.LeftBottom,
                    [1] = data.LeftTop,
                    [2] = data.RightBottom,
                    [3] = data.RightTop,
                };
                
                entitiesToSelect = new NativeList<Entity>(16, Allocator.TempJob);
                
                UnitBoxCast castJob = new UnitBoxCast()
                {
                    Points = points,
                    Collisions = collisions,
                    SelectedEntities = unitsSelected,
                    EntitiesToSelect = entitiesToSelect,
                    EntitiesToDeselect = entitiesToDeselect,
                    SelectionComponent = SystemAPI.GetComponentLookup<UnitSelectionTag>(true)
                };

                castJob.Schedule().Complete();

                if (entitiesToSelect.Length == 0)
                {
                    foreach (Entity selected in unitsSelected)
                        ecb.RemoveComponent<UnitSelectionTag>(selected);
                }
                else
                {
                    foreach (Entity deselect in entitiesToDeselect)
                        ecb.RemoveComponent<UnitSelectionTag>(deselect);
            
                    foreach (Entity select in entitiesToSelect)
                        ecb.AddComponent<UnitSelectionTag>(select);
                }
                
                state.EntityManager.RemoveComponent<EndDragEvent>(entity);
                
                points.Dispose();
                entitiesToSelect.Dispose();
            }
            
            if (SystemAPI.HasComponent<ClickMouseEvent>(entity))
            {
                ClickMouseEvent data = SystemAPI.GetComponentRO<ClickMouseEvent>(entity).ValueRO;
                NativeReference<Entity> castEntity = new NativeReference<Entity>(Entity.Null, Allocator.TempJob);

                float3 startPosition = data.WorldPosition;
                float3 endPosition = startPosition;
                startPosition.y += 10;
                
                UnitRayCast unitRayCast = new UnitRayCast()
                {
                    End = endPosition,
                    Start = startPosition,
                    Collisions = collisions,
                    ClosestEntity = castEntity,
                    SelectedEntities = unitsSelected,
                    EntitiesToDeselect = entitiesToDeselect,
                    SelectionComponent = SystemAPI.GetComponentLookup<UnitSelectionTag>(true)
                };
                
                unitRayCast.Schedule().Complete();

                if (castEntity.Value == Entity.Null)
                {
                    foreach (Entity selected in unitsSelected)
                        ecb.RemoveComponent<UnitSelectionTag>(selected);
                }
                else
                {
                    if (entitiesToDeselect.Length == 0)
                    {
                        foreach (Entity selected in unitsSelected)
                            ecb.RemoveComponent<UnitSelectionTag>(selected);
                        
                        ecb.AddComponent<UnitSelectionTag>(castEntity.Value);
                    }
                    else
                    {
                        foreach (Entity selected in entitiesToDeselect)
                            ecb.RemoveComponent<UnitSelectionTag>(selected);
                    }
                }

                state.EntityManager.RemoveComponent<ClickMouseEvent>(entity);

                castEntity.Dispose();
            }
            
            ecb.AddComponent<UnitsSelectedEvent>(_selectedEvent);
            ecb.Playback(state.EntityManager);
            
            ecb.Dispose();
            unitsSelected.Dispose();
            entitiesToDeselect.Dispose();
        }
    }

    [BurstCompile]
    public struct UnitBoxCast : IJob
    {
        [ReadOnly] public CollisionWorld Collisions;
        [ReadOnly] public NativeArray<float3> Points;
        [ReadOnly] public NativeArray<Entity> SelectedEntities;
        [ReadOnly] public ComponentLookup<UnitSelectionTag> SelectionComponent;
        
        [WriteOnly] public NativeList<Entity> EntitiesToSelect;
        [WriteOnly] public NativeList<Entity> EntitiesToDeselect;
        
        [BurstCompile]
        public unsafe void Execute()
        {
            BlobAssetReference<Collider> collider = ConvexCollider.Create
            (
                Points,
                new ConvexHullGenerationParameters(),
                CollisionFilter.Default
            );

            ColliderCastInput castInput = new ColliderCastInput()
            {
                Collider = (Collider*) collider.GetUnsafePtr(),
                Start = new float3(0, 10f, 0),
                End =  float3.zero,
                QueryColliderScale = 0,
                Orientation = quaternion.identity,
            };

            OverlapCollector collector = new OverlapCollector(16, Allocator.TempJob);

            if (!Collisions.CastCollider(castInput, ref collector))
            {
                collider.Dispose();
                collector.Dispose();
                return;
            }
            
            NativeList<Entity> newSelection = new NativeList<Entity>(collector.Hits.Length, Allocator.TempJob);

            for (int i = 0; i < collector.Hits.Length; i++)
            {
                Entity entity = collector.Hits[i].Entity;
                
                newSelection.Add(entity);
                
                if(SelectionComponent.HasComponent(entity)) continue;
                
                EntitiesToSelect.Add(entity);
            }
            
            foreach (Entity selected in SelectedEntities)
            {
                if (newSelection.Contains(selected)) continue;
                    
                EntitiesToDeselect.Add(selected);
            }
            
            collider.Dispose();
            collector.Dispose();
            newSelection.Dispose();
        }
    }
    
    [BurstCompile]
    public struct UnitRayCast : IJob
    {
        [ReadOnly] public float3 Start;
        [ReadOnly] public float3 End;
        
        [ReadOnly] public CollisionWorld Collisions;
        [ReadOnly] public NativeArray<Entity> SelectedEntities;
        [ReadOnly] public ComponentLookup<UnitSelectionTag> SelectionComponent;
        
        [WriteOnly] public NativeReference<Entity> ClosestEntity;
        [WriteOnly] public NativeList<Entity> EntitiesToDeselect;
        
        [BurstCompile]
        public void Execute()
        {
            RaycastInput raycastInput = new RaycastInput()
            {
                End = End,
                Start = Start,
                Filter = CollisionFilter.Default,
            };
            
            if(!Collisions.CastRay(raycastInput, out RaycastHit closestHit)) return;

            Entity entity = closestHit.Entity;

            ClosestEntity.Value = entity;
            
            if (!SelectionComponent.HasComponent(closestHit.Entity)) return;
            
            foreach (Entity selected in SelectedEntities)
            {
                if(selected == entity) continue;
                    
                EntitiesToDeselect.Add(selected);
            }
        }
    }
    
    [BurstCompile]
    public struct RaycastSelectJob : IJob
    {
        [ReadOnly] public CollisionWorld PhysicsWorld;
        public RaycastInput Ray;

        public EntityCommandBuffer.ParallelWriter Ecb;
        public Entity LastSelected;
        public Entity SingletonEntity;

        [BurstCompile]
        public void Execute()
        {
            PhysicsWorld.CastRay(Ray, out RaycastHit hit);

            Entity newSelected = Entity.Null;

            if (hit.Entity != Entity.Null)
            {
                newSelected = hit.Entity;

                if (LastSelected != Entity.Null && LastSelected != newSelected)
                    Ecb.RemoveComponent<UnitsHoverTag>(0, LastSelected);

                if (newSelected != LastSelected)
                    Ecb.AddComponent<UnitsHoverTag>(0, newSelected);
            }
            else
            {
                if (LastSelected != Entity.Null)
                    Ecb.RemoveComponent<UnitsHoverTag>(0, LastSelected);
            }

            Ecb.SetComponent(0, SingletonEntity, new LastSelectedUnit { Value = newSelected });
        }
    }
}