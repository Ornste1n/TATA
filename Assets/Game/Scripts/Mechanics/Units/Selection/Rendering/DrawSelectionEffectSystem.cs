using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Game.Scripts.Mechanics.Units.General;

namespace Game.Scripts.Mechanics.Units.Selection.Rendering
{
    [RequireMatchingQueriesForUpdate] 
    [UpdateInGroup(typeof(RenderingUnitsEffectsSystem))]
    public partial class DrawSelectionEffectSystem : SystemBase
    {
        private const int KMaxBatch = 1023;

        private bool _isInitialized;
        private SelectionEffectData _data;
        
        private RenderParams _renderParams;
        private NativeArray<float4x4> _matricesBuffer;
        
        protected override void OnCreate()
        {
            _matricesBuffer = new NativeArray<float4x4>(KMaxBatch, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            
            RequireForUpdate<UnitSelectionTag>();
            RequireForUpdate<SelectionEffectData>();
        }

        protected override void OnStartRunning()
        {
            if(_isInitialized) return;

            _isInitialized = true;
            
            _data = SystemAPI.GetSingleton<SelectionEffectData>();
            _renderParams = new RenderParams(_data.Material);
        }

        protected override void OnUpdate()
        {
            SelectionEffectData data = _data;
            RenderParams renderParams = _renderParams;
            NativeArray<float4x4> matrices = _matricesBuffer;

            int count = 0;
            foreach (UnitAspect unit in SystemAPI.Query<UnitAspect>().WithAll<UnitSelectionTag>())
            {
                matrices[count++] = BuildMatrix(unit.Transform.Position, data.Size, data.Offset);

                if (count == KMaxBatch)
                    DrawBatch(matrices, renderParams, data.Mesh, count);
            }
            
            DrawBatch(matrices, renderParams, data.Mesh, count);
        }
        
        private void DrawBatch(NativeArray<float4x4> buffer, RenderParams renderParams, Mesh mesh, int count)
        {
            NativeArray<Matrix4x4> matrices = buffer.Reinterpret<Matrix4x4>();
            Graphics.RenderMeshInstanced(renderParams, mesh, 0, matrices, count);
        }
        
        public static Matrix4x4 BuildMatrix(float3 unitPosition, float2 size, float2 offset)
        {
            float3 pos = unitPosition;
            
            pos.y += offset.y;
            pos.x -= size.x * 0.5f;
            pos.z -= size.y * 0.5f;

            return Matrix4x4.TRS(pos, Quaternion.Euler(90f, 0f, 0f), Vector3.one);
        }

        protected override void OnDestroy()
        {
            _matricesBuffer.Dispose();
        }
    }
    
    [UpdateInGroup(typeof(RenderingUnitsEffectsSystem))]
    public partial class DrawHoverEffectSystem : SystemBase
    {
        private bool _isInitialized;
        private HoverEffectData _data;
        private RenderParams _renderParams;
        private NativeArray<float4x4> _oneMatrix;
        
        protected override void OnCreate()
        {
            _oneMatrix = new NativeArray<float4x4>(1, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            
            EntityQuery needEnabled = SystemAPI.QueryBuilder()
                .WithAll<UnitsHoverTag>()
                .Build();

            RequireForUpdate(needEnabled);  
            RequireForUpdate<HoverEffectData>();
        }
        
        protected override void OnStartRunning()
        {
            if(_isInitialized) return;

            _isInitialized = true;
            _data = SystemAPI.GetSingleton<HoverEffectData>();
            _renderParams = new RenderParams(_data.Material);
        }
        
        protected override void OnUpdate()
        {
            HoverEffectData data = _data;
            Entity entity = SystemAPI.GetSingletonEntity<UnitsHoverTag>();
            
            UnitAspect unit = SystemAPI.GetAspect<UnitAspect>(entity);

            NativeArray<float4x4> matrix = _oneMatrix;
            matrix[0] = DrawSelectionEffectSystem.BuildMatrix(unit.Transform.Position, data.Size, data.Offset);
                
            NativeArray<Matrix4x4> matrices = matrix.Reinterpret<Matrix4x4>();
            Graphics.RenderMeshInstanced(_renderParams, data.Mesh, 0, matrices, 1);
        }

        protected override void OnDestroy()
        {
            _oneMatrix.Dispose();
        }
    }
}