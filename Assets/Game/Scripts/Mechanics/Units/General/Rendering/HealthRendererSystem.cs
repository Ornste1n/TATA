using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Game.Scripts.Extension.Meshes;
using Game.Scripts.Mechanics.Units.General.Components;

namespace Game.Scripts.Mechanics.Units.General.Rendering
{
    public partial class UnitsHealthRendererSystem : SystemBase
    {
        private Mesh _mesh;
        private Material _healthBarMaterial;
        private MaterialPropertyBlock _materialProperty;

        private float2 _offset;
        private float3 _minSize;
        private float _scaleFactor;
        private Camera _mainCamera;

        private const int MaxInstancesPerBatch = 1023;
        private static readonly int Fill = Shader.PropertyToID("_Fill");
        private static readonly int MeshSize = Shader.PropertyToID("_MeshSize");

        protected override void OnCreate()
        {
            _materialProperty = new MaterialPropertyBlock();
        }

        protected override void OnStartRunning()
        {
            Entity barEntity = SystemAPI.GetSingletonEntity<HealthBarComponent>();
            HealthBarComponent barComponent = SystemAPI.GetComponent<HealthBarComponent>(barEntity);

            _mainCamera = Camera.main;
            _offset = barComponent.Offset;
            _minSize = barComponent.MinSize;
            _scaleFactor = barComponent.ScaleFactor;
            _healthBarMaterial = barComponent.HealthMaterial.Value;
            _mesh = MeshUtility.CreateQuadMesh(barComponent.Size.x, barComponent.Size.y);
            
            _healthBarMaterial.SetVector(MeshSize, new Vector2(barComponent.Size.x, barComponent.Size.y));
            
            EntityManager.DestroyEntity(barEntity);
        }

        protected override void OnUpdate()
        {
            NativeList<Matrix4x4> matrices = new NativeList<Matrix4x4>(64, Allocator.Temp);
            NativeList<float> fills = new NativeList<float>(64, Allocator.Temp);

            float2 offset = _offset;
            Camera camera = _mainCamera;
            float3 minSize = _minSize;
            float scaleFactor = _scaleFactor;

            Vector3 cameraPosition = camera.transform.position;
            Vector3 cameraRotation = camera.transform.rotation.eulerAngles;
            Quaternion barRotation = Quaternion.Euler(cameraRotation);

            float barScaleFactor = Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad * 0.5f);
            
            foreach (UnitAspect unit in SystemAPI.Query<UnitAspect>())
            {
                Damageable damageable = unit.Damageable;
                float fill = damageable.Health / damageable.MaxHealth;

                Vector3 barPosition = unit.Transform.Position;
                barPosition.x += offset.x;
                barPosition.y += offset.y;

                float distance = Vector3.Distance(cameraPosition, barPosition);
                float3 scale = distance * Vector3.one * barScaleFactor * scaleFactor;

                if (scale.x < minSize.x || scale.y < minSize.y)
                    scale = minSize;
                
                Matrix4x4 matrix4X4 = Matrix4x4.TRS(barPosition, barRotation, scale);
                
                fills.Add(fill);
                matrices.Add(matrix4X4);
                
                if (matrices.Length == MaxInstancesPerBatch)
                    DrawHealthBatch(matrices, fills);
            }

            if(matrices.Length > 0)
                DrawHealthBatch(matrices, fills);

            fills.Dispose();
            matrices.Dispose();
        }

        private void DrawHealthBatch(NativeList<Matrix4x4> matrices, NativeList<float> fills)
        {
            _materialProperty.Clear();
            _materialProperty.SetFloatArray(Fill, fills.AsArray().ToArray());

            RenderParams rp = new RenderParams(_healthBarMaterial) { matProps = _materialProperty };

            Graphics.RenderMeshInstanced(rp, _mesh, 0, matrices.AsArray().ToArray(), matrices.Length);
            
            matrices.Clear();
            fills.Clear();
        }
    }
}