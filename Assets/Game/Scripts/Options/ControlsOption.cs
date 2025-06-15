using UnityEngine;
using Unity.Entities;
using Game.Scripts.Options.Models;
using Game.Scripts.Mechanics.CameraMovement;

namespace Game.Scripts.Options
{
    public class ControlsOption : Option<ControlsModel>
    {
#if !UNITY_EDITOR
        protected override void PresetOptions()
        {
            EntityManager em = Bootstrap.MenuWorld.EntityManager;
            Entity controlsSettings = em.CreateEntity();
            em.AddComponentData(controlsSettings, new CameraMoveSettingsData()
            {
                DragScrollSpeed = Model.DragScrollSpeed,
                MouseScrollSpeed = Model.MouseScrollSpeed,
                KeyboardScrollSpeed = Model.KeyboardScrollSpeed,
            });
        }
#endif

        public void UpdateSensitivity(ControlsModelRaw modelRaw)
        {
            EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery query = em.CreateEntityQuery(typeof(CameraMoveSettingsData));
            
            if(modelRaw.DragScrollSpeed != -1f)
                Model.DragScrollSpeed = modelRaw.DragScrollSpeed;
            
            if(modelRaw.MouseScrollSpeed != -1f)
                Model.MouseScrollSpeed = modelRaw.MouseScrollSpeed;
            
            if(modelRaw.KeyboardScrollSpeed != -1f)
                Model.KeyboardScrollSpeed = modelRaw.KeyboardScrollSpeed;
            
            if (!query.IsEmpty)
            {
                Entity singleton = query.GetSingletonEntity();
                em.SetComponentData(singleton, new CameraMoveSettingsData()
                {
                    DragScrollSpeed = Model.DragScrollSpeed,
                    MouseScrollSpeed = Model.MouseScrollSpeed,
                    KeyboardScrollSpeed = Model.KeyboardScrollSpeed
                });
            }
            else
            {
                Debug.LogWarning("CameraMoveSettingsData is not found");
            }
        }
        
        public float GetValue(ControlsSensitivity type)
        {
            return type switch
            {
                ControlsSensitivity.DragScroll => Model.DragScrollSpeed,
                ControlsSensitivity.MouseScroll => Model.MouseScrollSpeed,
                ControlsSensitivity.KeyboardScroll => Model.KeyboardScrollSpeed,
                _ => 0
            };
        }
        
        public override ControlsModel CreateDefaultModel()
        {
            return new ControlsModel()
            {
                DragScrollSpeed = 100f,
                MouseScrollSpeed = 100f,
                KeyboardScrollSpeed = 100f,
            };
        }
    }

    public enum ControlsSensitivity
    {
        DragScroll,
        MouseScroll,
        KeyboardScroll,
    }
}