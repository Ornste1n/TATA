using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts.Inputs.Components
{
    public struct MouseInputComponent : IComponentData
    {
        public bool OnDragScroll;
        
        public float3 Direction;
        public float3 ViewMousePosition;
        public float3 WorldMousePosition;

        public Ray ScreenToWorldRay;
    }
}