using UnityEngine;
using Unity.Entities;
using UnityEngine.UIElements;

namespace Game.Scripts.UI.Gameplay
{
    public struct MainCanvasData : IComponentData
    {
        public UnityObjectRef<UIDocument> Document;
    }

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class MainCanvasInitializeSystem : SystemBase
    {
        private GameMenu m_GameMenu;
        
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

            m_GameMenu = new GameMenu(document);
            
            Enabled = false;
        }

        protected override void OnDestroy()
        {
            m_GameMenu.Disable();
        }
    }
}