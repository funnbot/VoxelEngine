using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {

    public class MeshData {
        List<Vector3> verts;
        List<int> tris;
        List<Vector2> uvs;

        public int Count { get { return verts.Count; } }

        public MeshData() {
            verts = new List<Vector3>();
            tris = new List<int>();
            uvs = new List<Vector2>();
        }

        public void AddVert(Vector3 vert) {
            verts.Add(vert);
        }

        public void AddVerts(Vector3[] vert) {
            verts.AddRange(vert);
        }

        public void AddTri(int tri) {
            tris.Add(tri);
        }

        public void AddQuadTri() {
            int count = verts.Count;
            int[] t = new int[] {
                count - 4, count - 2, count - 3,
                count - 4, count - 1, count - 2
            };
            tris.AddRange(t);
        }

        public void AddDecalTri() {
            int count = verts.Count;
            int[] t = new int[] {
                count - 16, count - 14, count - 15,
                count - 16, count - 13, count - 14,

                count - 12, count - 11, count - 10,
                count - 12, count - 10, count - 9,

                count - 8, count - 6, count - 7,
                count - 8, count - 5, count - 6,

                count - 4, count - 3, count - 2,
                count - 4, count - 2, count - 1,
            };
            tris.AddRange(t);
        }

        public void AddUVs(Vector2[] uv) {
            uvs.AddRange(uv);
        }

        public Mesh ToMesh() {
            var mesh = new Mesh();
            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();
            return mesh;
        }

        public void Clear() {
            verts.Clear();
            tris.Clear();
            uvs.Clear();
        }
    }

}