using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {

    public class MeshData {
        List<Vector3> verts;
        List<int> tris;
        List<Vector2> uvs;

        public MeshData() {
            verts = new List<Vector3>();
            tris = new List<int>();
            uvs = new List<Vector2>();
        }

        public void AddQuad(int direction, Vector3Int position) {
            AddQuadVerts(direction, position);
            AddQuadTris();
        }

        public void AddDecal(Vector3Int position) {
            AddDecalVerts(position);
            AddDecalTris();
        }

        private void AddQuadTri() {
            int count = verts.Count;
            int[] t = new int[] {
                count - 4, count - 2, count - 3,
                count - 4, count - 1, count - 2
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

        public Mesh ToColMesh() {
            var mesh = new Mesh();
            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();
            return mesh;
        }

        public void Clear() {
            verts = new List<Vector3>(verts.Count);
            tris = new List<int>(tris.Count);
            uvs = new List<Vector2>(uvs.Count);
        }

        private const float F = 0.5f,
            T = 0.3535f;

        private void AddQuadVerts(int dir, Vector3Int pos) {
            float x = pos.x, y = pos.y, z = pos.z;
            switch (dir) {
                case 0:
                    verts.Add(new Vector3(x - F, y + F, z - F));
                    verts.Add(new Vector3(x + F, y + F, z - F));
                    verts.Add(new Vector3(x + F, y + F, z + F));
                    verts.Add(new Vector3(x - F, y + F, z + F));
                    break;
                case 1:
                    verts.Add(new Vector3(x - F, y - F, z + F));
                    verts.Add(new Vector3(x + F, y - F, z + F));
                    verts.Add(new Vector3(x + F, y - F, z - F));
                    verts.Add(new Vector3(x - F, y - F, z - F));
                    break;
                case 2:
                    verts.Add(new Vector3(x + F, y + F, z + F));
                    verts.Add(new Vector3(x + F, y + F, z - F));
                    verts.Add(new Vector3(x + F, y - F, z - F));
                    verts.Add(new Vector3(x + F, y - F, z + F));
                    break;
                case 3:
                    verts.Add(new Vector3(x - F, y + F, z - F));
                    verts.Add(new Vector3(x - F, y + F, z + F));
                    verts.Add(new Vector3(x - F, y - F, z + F));
                    verts.Add(new Vector3(x - F, y - F, z - F));
                    break;
                case 4:
                    verts.Add(new Vector3(x - F, y + F, z + F));
                    verts.Add(new Vector3(x + F, y + F, z + F));
                    verts.Add(new Vector3(x + F, y - F, z + F));
                    verts.Add(new Vector3(x - F, y - F, z + F));
                    break;
                case 5:
                    verts.Add(new Vector3(x + F, y + F, z - F));
                    verts.Add(new Vector3(x - F, y + F, z - F));
                    verts.Add(new Vector3(x - F, y - F, z - F));
                    verts.Add(new Vector3(x + F, y - F, z - F));
                    break;
            }
        }

        private void AddDecalVerts(Vector3Int pos) {
            float x = pos.x, y = pos.y, z = pos.z;
            for (int i = 0; i < 2; i++) {
                verts.Add(new Vector3(x - T, y + F, x + T));
                verts.Add(new Vector3(x + T, y + F, x - T));
                verts.Add(new Vector3(x + T, y - F, x - T));
                verts.Add(new Vector3(x - T, y - F, x + T));
            }
            for (int i = 0; i < 2; i++) {
                verts.Add(new Vector3(x - T, y + F, x - T));
                verts.Add(new Vector3(x + T, y + F, x + T));
                verts.Add(new Vector3(x + T, y - F, x + T));
                verts.Add(new Vector3(x - T, y - F, x - T));
            }
        }

        private void AddDecalTris() {
            int c = verts.Count;
            AddQuadTris(c - 12);
            AddInverseQuadTris(c - 8);
            AddQuadTris(c - 4);
            AddInverseQuadTris(c);
        }

        private void AddQuadTris(int c) {
            tris.Add(c - 4);
            tris.Add(c - 2);
            tris.Add(c - 3);
            tris.Add(c - 4);
            tris.Add(c - 1);
            tris.Add(c - 2);
        }
        private void AddQuadTris() =>
            AddQuadTris(verts.Count);

        private void AddInverseQuadTris(int c) {
            tris.Add(c - 4);
            tris.Add(c - 3);
            tris.Add(c - 2);
            tris.Add(c - 4);
            tris.Add(c - 2);
            tris.Add(c - 1);
        }
    }

}