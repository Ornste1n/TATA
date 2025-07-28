using UnityEngine;
using Unity.Entities;
using Game.Scripts.SaveSystems.Components;

namespace Game.Scripts.SaveSystems
{
    public partial class SaveSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<SaveEvent>();
        }

        protected override void OnUpdate()
        {
            Entity eventEntity = SystemAPI.GetSingletonEntity<SaveEvent>();
            
            Debug.Log("Save");
            
            EntityManager.RemoveComponent<SaveEvent>(eventEntity);
        }
    }
}