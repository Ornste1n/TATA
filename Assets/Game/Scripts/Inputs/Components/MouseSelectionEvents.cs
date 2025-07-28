using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts.Inputs.Components
{
    public struct ClickMouseEvent : IComponentData
    {
        /*public readonly float3 WorldPosition;

        public ClickMouseEvent(float3 worldPosition)
        {
            WorldPosition = worldPosition;
        }*/
    }

    public struct EndDragEvent : IComponentData
    {
        public readonly float3 LeftTop;
        public readonly float3 LeftBottom;

        public readonly float3 RightTop;
        public readonly float3 RightBottom;

        public EndDragEvent(float3 leftTop, float3 leftBottom, float3 rightTop, float3 rightBottom)
        {
            LeftTop = leftTop;
            LeftBottom = leftBottom;
            RightTop = rightTop;
            RightBottom = rightBottom;
        }
    }
}