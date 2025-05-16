using UnityEngine;
using Unity.Entities;
using Game.Scripts.Inputs;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
using Game.Scripts.Extension.Meshes;

namespace Game.Scripts.Mechanics.MouseSelection
{
    public class SelectionAreaDrawer
    {
        private readonly Mesh _mesh;
        private readonly Material _shaderMaterial;
        private readonly PlayerInputSystem _inputSystem;
        private static readonly int QuadSize = Shader.PropertyToID("_QuadSize");
        
        private Vector2 _startMousePosition;

        public SelectionAreaDrawer(Material shaderMaterial)
        {
            _shaderMaterial = shaderMaterial;
            _mesh = MeshUtility.CreateQuadMesh();
            _inputSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerInputSystem>();
            
            _inputSystem.OnMouseEndDrag += Stop;
            _inputSystem.OnMouseBeginDrag += Start;
        }
        
        private void Start(Vector2 position)
        {
            _startMousePosition = Camera.main.ScreenToViewportPoint(position);
            RenderPipelineManager.endCameraRendering += Draw;
        }

        private void Draw(ScriptableRenderContext context, Camera camera)
        {
            Vector2 viewportPosition = camera.ScreenToViewportPoint(Mouse.current.position.value);

            GL.PushMatrix();
            GL.LoadOrtho();
            
            float minX = Mathf.Min(_startMousePosition.x, viewportPosition.x);
            float maxX = Mathf.Max(_startMousePosition.x, viewportPosition.x);
            
            float minY = Mathf.Min(_startMousePosition.y, viewportPosition.y);
            float maxY = Mathf.Max(_startMousePosition.y, viewportPosition.y);

            Vector3 position = new Vector3(minX, minY, 0);
            Vector3 scale = new Vector3(maxX - minX, maxY - minY, 1);
            
            Matrix4x4 matrix4X4 = Matrix4x4.TRS(position, Quaternion.identity, scale);

            float quadWidth = scale.x * Screen.width;
            float quadHeight = scale.y * Screen.height;
            
            _shaderMaterial.SetVector(QuadSize, new Vector2(quadWidth, quadHeight));
            _shaderMaterial.SetPass(0);
            
            Graphics.DrawMeshNow(_mesh, matrix4X4);
            GL.PopMatrix();
        }

        private void Stop()
        {
            RenderPipelineManager.endCameraRendering -= Draw;
        }
        
        public void Dispose()
        {
            Stop();
            _inputSystem.OnMouseEndDrag -= Stop;
            _inputSystem.OnMouseBeginDrag -= Start;
        }
    }
}