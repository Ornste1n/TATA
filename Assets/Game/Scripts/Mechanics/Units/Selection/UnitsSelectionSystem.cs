using Unity.Entities;
using Unity.Collections;
using Game.Scripts.Inputs.Components;
using UnityEngine;

namespace Game.Scripts.Mechanics.Units.Selection
{
    public partial struct UnitsSelectionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAny<EndDragEvent, ClickMouseEvent>();
            
            EntityQuery query = state.GetEntityQuery(builder);
            
            state.RequireAnyForUpdate(query);
            state.RequireForUpdate<InputData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            Entity entity = SystemAPI.GetSingletonEntity<InputData>();

            if (SystemAPI.HasComponent<EndDragEvent>(entity))
            {
                UnityEngine.Debug.Log($"Check with boxcast");
                state.EntityManager.RemoveComponent<EndDragEvent>(entity);
            }
            
            if (SystemAPI.HasComponent<ClickMouseEvent>(entity))
            {
                UnityEngine.Debug.Log($"Check with raycast");
                state.EntityManager.RemoveComponent<ClickMouseEvent>(entity);
            }
        }
    }
}