using Unity.Entities;

namespace Game.Scripts.Extension.ECS
{
    public static class EcsMultiWorld
    {
        public static void CreateSingletonInAllWorlds<T>(T data) where T : unmanaged, IComponentData
        {
            foreach (var world in World.All)
            {
                EntityManager em = world.EntityManager;
                EntityQuery query = em.CreateEntityQuery(typeof(T));
                
                if (query.IsEmpty)
                {
                    Entity entity = em.CreateEntity();
                    em.AddComponentData(entity, data);
                }
            }
        }
    }
}