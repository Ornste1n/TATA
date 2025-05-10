using UnityEngine;
using Unity.Entities;
using Game.Scripts.Inputs;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

namespace Game.Scripts.Mechanics.MouseSelection
{
    public class SelectionAreaDrawer
    {
        private readonly Mesh _mesh;
        private readonly Material _shaderMaterial;
        private static readonly int QuadSize = Shader.PropertyToID("_QuadSize");
        
        private Vector2 _startMousePosition;
        private PlayerInputSystem _inputSystem;

        public SelectionAreaDrawer(Material shaderMaterial)
        {
            _mesh = CreateQuadMesh();
            _shaderMaterial = shaderMaterial;
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

        private Mesh CreateQuadMesh()
        {
            Mesh mesh = new Mesh();
            
            Vector3[] vertices =
            {
                new (0, 0 ,0),
                new (0, 1 ,0),
                new (1, 0 ,0),
                new (1, 1 ,0),
            };
            
            Vector2[] uv =
            {
                new (0, 0),
                new (0, 1),
                new (1, 0),
                new (1, 1),
            };

            int[] triangles =
            {
                0, 1, 2,
                2, 1, 3
            };

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();

            return mesh;
        }
        
        public void Dispose()
        {
            _inputSystem.OnMouseEndDrag -= Stop;
            _inputSystem.OnMouseBeginDrag -= Start;
        }
    }
}