using Game.Scripts.Extension.Meshes;
using Game.Scripts.Mechanics.Units.General.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Scripts.Mechanics.Units.General.Rendering.Health
{
    [UpdateInGroup(typeof(RenderingUnitsEffectsSystem))]
    public partial class UnitsHealthRendererSystem : SystemBase
    {
        private Mesh _mesh;
        private Material _healthBarMaterial;
        private MaterialPropertyBlock _materialProperty;

        private float2 _offset;
        private float3 _minSize;
        private float _scaleFactor;
        private Camera _mainCamera;

        private NativeArray<float> _fills;
        private NativeArray<Matrix4x4> _matrices;
        
        private const int MaxInstancesPerBatch = 1023;
        private static readonly int Fill = Shader.PropertyToID("_Fill");
        private static readonly int MeshSize = Shader.PropertyToID("_MeshSize");

        protected override void OnCreate()
        {
            _materialProperty = new MaterialPropertyBlock();
            _fills = new NativeArray<float>(MaxInstancesPerBatch, Allocator.Persistent);
            _matrices = new NativeArray<Matrix4x4>(MaxInstancesPerBatch, Allocator.Persistent);
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
            float2 offset = _offset;
            float3 minSize = _minSize;
            Camera camera = _mainCamera;
            float scaleFactor = _scaleFactor;

            Vector3 cameraPosition = camera.transform.position;
            Vector3 cameraRotation = camera.transform.rotation.eulerAngles;
            Quaternion barRotation = Quaternion.Euler(cameraRotation);

            float barScaleFactor = math.tan(camera.fieldOfView * Mathf.Deg2Rad * 0.5f);

            int count = 0;
            NativeArray<float> fills = _fills;
            NativeArray<Matrix4x4> matrices = _matrices;
            
            foreach (UnitAspect unit in SystemAPI.Query<UnitAspect>())
            {
                Damageable damageable = unit.Damageable;

                Vector3 barPosition = unit.Transform.Position;
                barPosition.x += offset.x;
                barPosition.y += offset.y;

                float distance = math.distance(cameraPosition, barPosition);
                float3 scale = math.max(distance * Vector3.one * barScaleFactor * scaleFactor, minSize);
                
                fills[count] = damageable.Health / damageable.MaxHealth;
                matrices[count++] = Matrix4x4.TRS(barPosition, barRotation, scale);

                if (count != MaxInstancesPerBatch) continue;
                
                DrawHealthBatch(matrices, fills);
                count = 0;
            }

            if(matrices.Length > 0)
                DrawHealthBatch(matrices, fills);
        }

        private void DrawHealthBatch(NativeArray<Matrix4x4> matrices, NativeArray<float> fills)
        {
            _materialProperty.Clear();
            _materialProperty.SetFloatArray(Fill, fills.ToArray());

            RenderParams render = new RenderParams(_healthBarMaterial) { matProps = _materialProperty };
            Graphics.RenderMeshInstanced(render, _mesh, 0, matrices, matrices.Length);
        }

        protected override void OnDestroy()
        {
            _fills.Dispose();
            _matrices.Dispose();
        }
    }
}