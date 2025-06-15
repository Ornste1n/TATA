using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts.Mechanics.CameraMovement
{
    public struct CameraMoveSettingsData : IComponentData
    {
        public float DragScrollSpeed;
        public float MouseScrollSpeed;
        public float KeyboardScrollSpeed;
    }
    
    public struct CameraPositionComponent : IComponentData
    {
        public float3 Position;

        public CameraPositionComponent(float3 position) => Position = position;
    }
}