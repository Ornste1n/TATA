using UnityEngine;

namespace Game.Scripts.Extension.Meshes
{
    public static class MeshUtility
    {
        public static Mesh CreateQuadMesh(float width = 1f, float height = 1f)
        {
            Mesh mesh = new Mesh();
            
            Vector3[] vertices =
            {
                new (0, 0 ,0),
                new (0, height ,0),
                new (width, 0 ,0),
                new (width, height, 0),
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
    }
}