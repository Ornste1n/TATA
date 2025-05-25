using UnityEngine;
using Unity.Entities;
using UnityEngine.UIElements;

namespace Game.Scripts.UI.Gameplay
{
    public struct MainCanvasData : IComponentData
    {
        public UnityObjectRef<UIDocument> Document;
    }

    public partial class MainCanvasInitializeSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<MainCanvasRawData>();
        }

        protected override void OnUpdate()
        {
            Entity singleton = SystemAPI.GetSingletonEntity<MainCanvasRawData>();
            MainCanvasRawData data = SystemAPI.GetComponent<MainCanvasRawData>(singleton);

            UIDocument document = Object.Instantiate(data.DocumentGo.Value).GetComponent<UIDocument>();
            document.gameObject.name = $"[UI document] {data.DocumentGo.Value.name}";

            EntityManager.AddComponentData(singleton, new MainCanvasData()
            {
                Document = document
            });

            Enabled = false;
        }
    }
}