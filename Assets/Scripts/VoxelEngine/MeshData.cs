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

        public void AddQuad(int direction, Coord3 position, int faceRotation, int textureIndex) {
            AddQuadVerts(direction, position);
            AddQuadTris();
            AddQuadUV(textureIndex, faceRotation);
        }

        public void AddQuad(int direction, Coord3 position) {
            AddQuadVerts(direction, position);
            AddQuadTris();
        }

        public void AddDecal(Coord3 position, int textureIndex) {
            AddDecalVerts(position);
            AddDecalTris();
            for (int i = 0; i < 4; i++)
                AddQuadUV(textureIndex, 0);
        }

        public Mesh ToMesh() {
            var mesh = new Mesh();
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);

            mesh.RecalculateNormals();
            mesh.UploadMeshData(true);
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

        private void AddQuadUV(int ind, int rot) {
            Coord2 a = Coord2.Corners[(rot + 4) % 4],
                b = Coord2.Corners[(rot + 5) % 4],
                c = Coord2.Corners[(rot + 6) % 4],
                d = Coord2.Corners[(rot + 7) % 4];
            uvs.Add(new Vector3(a.x, a.y, ind));
            uvs.Add(new Vector3(b.x, b.y, ind));
            uvs.Add(new Vector3(c.x, c.y, ind));
            uvs.Add(new Vector3(d.x, d.y, ind));
        }

        private void AddQuadVerts(int dir, Coord3 pos) {
            float x = pos.x, y = pos.y, z = pos.z;
            switch (dir) {
                case BlockFace.front:
                    verts.Add(new Vector3(x - F, y + F, z + F));
                    verts.Add(new Vector3(x + F, y + F, z + F));
                    verts.Add(new Vector3(x + F, y - F, z + F));
                    verts.Add(new Vector3(x - F, y - F, z + F));
                    break;
                case BlockFace.back:
                    verts.Add(new Vector3(x + F, y + F, z - F));
                    verts.Add(new Vector3(x - F, y + F, z - F));
                    verts.Add(new Vector3(x - F, y - F, z - F));
                    verts.Add(new Vector3(x + F, y - F, z - F));
                    break;
                case BlockFace.top:
                    verts.Add(new Vector3(x - F, y + F, z - F));
                    verts.Add(new Vector3(x + F, y + F, z - F));
                    verts.Add(new Vector3(x + F, y + F, z + F));
                    verts.Add(new Vector3(x - F, y + F, z + F));
                    break;
                case BlockFace.bottom:
                    verts.Add(new Vector3(x - F, y - F, z + F));
                    verts.Add(new Vector3(x + F, y - F, z + F));
                    verts.Add(new Vector3(x + F, y - F, z - F));
                    verts.Add(new Vector3(x - F, y - F, z - F));
                    break;
                case BlockFace.right:
                    verts.Add(new Vector3(x + F, y + F, z + F));
                    verts.Add(new Vector3(x + F, y + F, z - F));
                    verts.Add(new Vector3(x + F, y - F, z - F));
                    verts.Add(new Vector3(x + F, y - F, z + F));
                    break;
                case BlockFace.left:
                    verts.Add(new Vector3(x - F, y + F, z - F));
                    verts.Add(new Vector3(x - F, y + F, z + F));
                    verts.Add(new Vector3(x - F, y - F, z + F));
                    verts.Add(new Vector3(x - F, y - F, z - F));
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