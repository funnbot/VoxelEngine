using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {

    public class MeshData {
        List<Vector3> verts;
        List<int> tris;
        List<Vector3> uvs;

        public MeshData() {
            verts = new List<Vector3>();
            tris = new List<int>();
            uvs = new List<Vector3>();
        }

        public void AddQuad(int direction, Coord3 position, int textureIndex) {
            AddQuadVerts(direction, position);
            AddQuadTris();
            AddQuadUV(textureIndex);
        }

        public void AddDecal(Coord3 position, int textureIndex) {
            AddDecalVerts(position);
            AddDecalTris();
            for (int i = 0; i < 4; i++)
                AddQuadUV(textureIndex);
        }

        public Mesh ToMesh() {
            var mesh = new Mesh();
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);

            mesh.RecalculateNormals();
            mesh.UploadMeshData(false);
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
            uvs = new List<Vector3>(uvs.Count);
        }

        private const float F = 0.5f,
            T = 0.3535f;

        private void AddQuadUV(int ind) {
            uvs.Add(new Vector3(0, 1, ind));
            uvs.Add(new Vector3(1, 1, ind));
            uvs.Add(new Vector3(1, 0, ind));
            uvs.Add(new Vector3(0, 0, ind));
        }

        private void AddQuadVerts(int dir, Coord3 pos) {
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

        private void AddDecalVerts(Coord3 pos) {
            float x = pos.x, y = pos.y, z = pos.z;
            for (int i = 0; i < 2; i++) {
                verts.Add(new Vector3(x - T, y + F, z + T));
                verts.Add(new Vector3(x + T, y + F, z - T));
                verts.Add(new Vector3(x + T, y - F, z - T));
                verts.Add(new Vector3(x - T, y - F, z + T));
            }
            for (int i = 0; i < 2; i++) {
                verts.Add(new Vector3(x - T, y + F, z - T));
                verts.Add(new Vector3(x + T, y + F, z + T));
                verts.Add(new Vector3(x + T, y - F, z + T));
                verts.Add(new Vector3(x - T, y - F, z - T));
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