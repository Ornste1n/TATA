using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Game.Scripts.Inputs.Components;

namespace Game.Scripts.Mechanics.CameraMovement
{
    [UpdateInGroup(typeof(CameraSystemsGroup)), UpdateAfter(typeof(CameraMovementSystem))]
    public partial class SyncCameraObjectSystem : SystemBase
    {
        private Transform _targetTransform;
        
        protected override void OnCreate()
        {
            RequireForUpdate<CameraMoveSettingsData>();
            RequireForUpdate<CameraPositionComponent>();
        }

        protected override void OnStartRunning()
        {
            _targetTransform = Camera.main!.transform;
            ref CameraPositionComponent positionComponent = ref SystemAPI.GetSingletonRW<CameraPositionComponent>().ValueRW;
            positionComponent.Position = _targetTransform.position;
        }

        protected override void OnUpdate()
        {
            _targetTransform.position = SystemAPI.GetSingleton<CameraPositionComponent>().Position;
        }
    }
    
    [UpdateInGroup(typeof(CameraSystemsGroup))]
    [BurstCompile]
    public partial struct CameraMovementSystem : ISystem
    {
        private const float MinEdge = 0.005f;
        private const float MaxEdge = 0.995f;

        private bool _onDragScrolling;
        private float3 _startDragPosition;
        private float3 _centerCameraPosition;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CameraMoveSettingsData>();
            state.RequireForUpdate<CameraPositionComponent>();
            state.RequireForUpdate<MouseInputComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            MouseInputComponent input = SystemAPI.GetSingleton<MouseInputComponent>();
            CameraMoveSettingsData settings = SystemAPI.GetSingleton<CameraMoveSettingsData>();
            ref CameraPositionComponent cameraPosition = ref SystemAPI.GetSingletonRW<CameraPositionComponent>().ValueRW;

            if (!input.OnDragScroll && !_onDragScrolling)
            {
                float scrollSpeed = settings.KeyboardScrollSpeed;

                if (settings.MouseScrollSpeed != 0)
                {
                    float3 direction = float3.zero;
                    float minEdge = MinEdge; float maxEdge = MaxEdge;
                    
                    if (input.ViewMousePosition.x >= maxEdge)
                        direction.x = 1;
                    else if (input.ViewMousePosition.x <= minEdge)
                        direction.x = -1;

                    if (input.ViewMousePosition.y >= maxEdge)
                        direction.z = 1;
                    else if (input.ViewMousePosition.y <= minEdge)
                        direction.z = -1;

                    if (direction.x != 0 || direction.z != 0)
                    {
                        scrollSpeed = settings.MouseScrollSpeed;

                        if (direction.x != 0) input.Direction.x = direction.x;
                        
                        if (direction.z != 0) input.Direction.z = direction.z;
                    }
                }
            
                float3 normalized = math.lengthsq(input.Direction) > 0f ? math.normalize(input.Direction) : float3.zero;
                cameraPosition.Position += scrollSpeed * normalized * SystemAPI.Time.DeltaTime;
            }
            else if(!input.OnDragScroll && _onDragScrolling)
            {
                _onDragScrolling = false;
            }
            else if(!_onDragScrolling)
            {
                _onDragScrolling = true;
                _startDragPosition = input.ViewMousePosition;
                _centerCameraPosition = cameraPosition.Position;
            }
            else if(_onDragScrolling)
            {
                float3 offset = _startDragPosition - input.ViewMousePosition;
                float3 direction = new float3(offset.x, 0, offset.y);
                cameraPosition.Position = _centerCameraPosition + (settings.DragScrollSpeed / 2f * direction);
            }
        }
    }

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class CameraMovementInitializeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            EntityManager gameManager = EntityManager;
            Entity entity = gameManager.CreateEntity();
            
            gameManager.AddComponent<MouseInputComponent>(entity);
            gameManager.AddComponent<CameraPositionComponent>(entity);

#if !UNITY_EDITOR
            EntityManager menuManager = Bootstrap.MenuWorld.EntityManager;
            EntityQuery query = menuManager.CreateEntityQuery(typeof(CameraMoveSettingsData));

            CameraMoveSettingsData data = query.GetSingleton<CameraMoveSettingsData>();

            gameManager.AddComponentData(entity, data);
#else
            gameManager.AddComponentData(entity, new CameraMoveSettingsData()
            {
                DragScrollSpeed = 50f,
                MouseScrollSpeed = 0f,
                KeyboardScrollSpeed = 50f,
            });
#endif
            
            Enabled = false;
        }
    }
    
    public partial class CameraSystemsGroup : ComponentSystemGroup { }
}