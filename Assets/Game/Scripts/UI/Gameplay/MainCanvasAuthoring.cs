using Unity.Entities;
using UnityEngine;

namespace Game.Scripts.UI.Gameplay
{
    public class MainCanvasAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _uiDocument;

        public class MainCanvasAuthoringBaker : Baker<MainCanvasAuthoring>
        {
            public override void Bake(MainCanvasAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MainCanvasRawData()
                {
                    DocumentGo = authoring._uiDocument
                });
            }
        }
    }

    public struct MainCanvasRawData : IComponentData
    {
        public UnityObjectRef<GameObject> DocumentGo;
    }
}