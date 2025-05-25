using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Game.Scripts.Extension.Meshes;
using Game.Scripts.Mechanics.Units.General;

namespace Game.Scripts.Mechanics.Units.Selection.Rendering
{
    [UpdateInGroup(typeof(SelectionSystemGroup))]
    public partial class DrawSelectionEffectSystem : SystemBase
    {
        private Mesh _mesh;
        private Material _material;

        private float2 _size;
        private float2 _offset;
        private bool _initialized;
        
        protected override void OnCreate()
        {
            _initialized = false;
            RequireForUpdate<UnitSelectionTag>();
        }

        protected override void OnStartRunning()
        {
            if(_initialized) return;
            
            Entity entity = SystemAPI.GetSingletonEntity<SelectionEffectData>();
            SelectionEffectData data = SystemAPI.GetComponent<SelectionEffectData>(entity);
            
            _size = data.Size;
            _offset = data.Offset;
            _material = data.HealthMaterial.Value;
            _mesh = MeshUtility.CreateQuadMesh(data.Size.x, data.Size.y);
            
            EntityManager.DestroyEntity(entity);
            _initialized = true;
        }

        protected override void OnUpdate()
        {
            NativeList<Matrix4x4> matrix4X4s = new NativeList<Matrix4x4>(16, Allocator.Temp);

            float2 size = _size;
            float offsetY = _offset.y;
            
            foreach (UnitAspect unit in SystemAPI.Query<UnitAspect>().WithAll<UnitSelectionTag>())
            {
                float3 effectPosition = unit.Transform.Position;

                effectPosition.y += offsetY;
                effectPosition.x -= size.x / 2f;
                effectPosition.z -= size.y / 2f;
                
                Quaternion quaternion = Quaternion.Euler(90f, 0f, 0f);
                Matrix4x4 matrix = Matrix4x4.TRS(effectPosition, quaternion, Vector3.one);
                
                matrix4X4s.Add(matrix);
                
                if(matrix4X4s.Length == 1023)
                    DrawBatch(matrix4X4s);
            }
            
            DrawBatch(matrix4X4s);
            
            matrix4X4s.Dispose();
        }

        private void DrawBatch(NativeList<Matrix4x4> matrix4X4S)
        {
            RenderParams renderParams = new RenderParams(_material);
            Graphics.RenderMeshInstanced(renderParams, _mesh, 0, matrix4X4S.AsArray().ToArray(), matrix4X4S.Length);
            matrix4X4S.Clear();
        }
    }
}