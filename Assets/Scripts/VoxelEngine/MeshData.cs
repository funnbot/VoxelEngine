﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {

    public class MeshData {
        private const int MaxVertexCount = 65000;
        private const int SubMeshCount = 2;

        List<Vector3> verts;
        List<Vector3> normals;
        List<Vector3> uvs;
        List<int>[] tris;

        public MeshData() {
            verts = new List<Vector3>();
            normals = new List<Vector3>();
            uvs = new List<Vector3>();

            tris = new List<int>[SubMeshCount];
            for (int i = 0; i < SubMeshCount; i++)
                tris[i] = new List<int>();
        }

        public void AddQuad(int direction, Coord3 position, int faceRotation, int textureIndex, SubMesh subMesh) {
            AddQuadVerts(direction, position);
            AddQuadTris((int) subMesh);
            AddQuadUV(textureIndex, faceRotation);
        }

        public void AddQuad(int direction, Coord3 position) {
            AddQuadVerts(direction, position);
            AddQuadTris(0);
        }

        public void AddDecalCross(Coord3 position, int textureIndex, SubMesh subMesh) {
            AddDecalCrossVerts(position);
            AddDecalCrossTris((int) subMesh);
            for (int i = 0; i < 4; i++)
                AddQuadUV(textureIndex, 0);
        }

        public void AddBoundingBox(Coord3 position, float size, int down = 3, bool cullDown = false) {
            for (int i = 0; i < 6; i++) {
                if (cullDown && i == down) continue;
                AddQuadVerts(i, position, size, down);
            }
        }

        public Mesh ToMesh() {
            var mesh = new Mesh();
            mesh.subMeshCount = SubMeshCount;
            mesh.SetVertices(verts);

            for (int i = 0; i < SubMeshCount; i++)
                mesh.SetTriangles(tris[i], i);

            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();
            mesh.UploadMeshData(true);
            return mesh;
        }

        public Mesh ToColMesh() {
            var mesh = new Mesh();
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris[0], 0);
            mesh.UploadMeshData(true);
            return mesh;
        }

        public void Clear() {
            verts = new List<Vector3>(verts.Count);
            normals = new List<Vector3>(verts.Count);
            uvs = new List<Vector3>(uvs.Count);
            for (int i = 0; i < SubMeshCount; i++)
                tris[i] = new List<int>(tris[i].Count);
        }

        private const float F = 0.5f,
            T = 0.3535f,
            O = 0.1f;

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

        private void AddQuadVerts(int dir, Coord3 pos, float s, int down) {
            float x = pos.x, y = pos.y, z = pos.z;
            if (down != 3) Debug.LogError("Directional Bounding Boxes Not Implemented");
            switch (dir) {
                case BlockFace.front:
                    verts.Add(new Vector3((x - F) * s, (y + F) * s, (z + F) * s));
                    verts.Add(new Vector3((x + F) * s, (y + F) * s, (z + F) * s));
                    verts.Add(new Vector3((x + F) * s, (y - F), (z + F) * s));
                    verts.Add(new Vector3((x - F) * s, (y - F), (z + F) * s));
                    break;
                case BlockFace.back:
                    verts.Add(new Vector3((x + F) * s, (y + F) * s, (z - F) * s));
                    verts.Add(new Vector3((x - F) * s, (y + F) * s, (z - F) * s));
                    verts.Add(new Vector3((x - F) * s, (y - F), (z - F) * s));
                    verts.Add(new Vector3((x + F) * s, (y - F), (z - F) * s));
                    break;
                case BlockFace.top:
                    verts.Add(new Vector3((x - F) * s, (y + F) * s, (z - F) * s));
                    verts.Add(new Vector3((x + F) * s, (y + F) * s, (z - F) * s));
                    verts.Add(new Vector3((x + F) * s, (y + F) * s, (z + F) * s));
                    verts.Add(new Vector3((x - F) * s, (y + F) * s, (z + F) * s));
                    break;
                case BlockFace.bottom:
                    verts.Add(new Vector3((x - F) * s, (y - F), (z + F) * s));
                    verts.Add(new Vector3((x + F) * s, (y - F), (z + F) * s));
                    verts.Add(new Vector3((x + F) * s, (y - F), (z - F) * s));
                    verts.Add(new Vector3((x - F) * s, (y - F), (z - F) * s));
                    break;
                case BlockFace.right:
                    verts.Add(new Vector3((x + F) * s, (y + F) * s, (z + F) * s));
                    verts.Add(new Vector3((x + F) * s, (y + F) * s, (z - F) * s));
                    verts.Add(new Vector3((x + F) * s, (y - F), (z - F) * s));
                    verts.Add(new Vector3((x + F) * s, (y - F), (z + F) * s));
                    break;
                case BlockFace.left:
                    verts.Add(new Vector3((x - F) * s, (y + F) * s, (z - F) * s));
                    verts.Add(new Vector3((x - F) * s, (y + F) * s, (z + F) * s));
                    verts.Add(new Vector3((x - F) * s, (y - F), (z + F) * s));
                    verts.Add(new Vector3((x - F) * s, (y - F), (z - F) * s));
                    break;
            }
        }

        private void AddDecalCrossVerts(Coord3 pos) {
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

        private void AddDecalHashVerts(Coord3 pos) {
            float x = pos.x, y = pos.y, z = pos.z;
            for (int i = 0; i < 2; i++) {
                verts.Add(new Vector3(x - F, y + F, z + T));
                verts.Add(new Vector3(x + F, y + F, z + T));
                verts.Add(new Vector3(x + F, y - F, z + T));
                verts.Add(new Vector3(x - F, y - F, z + T));
            }
            for (int i = 0; i < 2; i++) {
                verts.Add(new Vector3(x - F, y + F, z - T));
                verts.Add(new Vector3(x + F, y + F, z - T));
                verts.Add(new Vector3(x + F, y - F, z - T));
                verts.Add(new Vector3(x - F, y - F, z - T));
            }
            for (int i = 0; i < 2; i++) {
                verts.Add(new Vector3(x + T, y + F, z + F));
                verts.Add(new Vector3(x + T, y + F, z - F));
                verts.Add(new Vector3(x + T, y - F, z - F));
                verts.Add(new Vector3(x + T, y - F, z + F));
            }
            for (int i = 0; i < 2; i++) {
                verts.Add(new Vector3(x - T, y + F, z - F));
                verts.Add(new Vector3(x - T, y + F, z + F));
                verts.Add(new Vector3(x - T, y - F, z + F));
                verts.Add(new Vector3(x - T, y - F, z - F));
            }
        }

        private void AddDecalCrossTris(int sub) {
            int c = verts.Count;
            AddQuadTris(c - 12, sub);
            AddInverseQuadTris(c - 8, sub);
            AddQuadTris(c - 4, sub);
            AddInverseQuadTris(c, sub);
        }

        private void AddDecalHashTris(int sub) {
            int c = verts.Count;
            AddQuadTris(c - 28, sub);
            AddInverseQuadTris(c - 24, sub);
            AddQuadTris(c - 20, sub);
            AddInverseQuadTris(c - 16, sub);
            AddQuadTris(c - 12, sub);
            AddInverseQuadTris(c - 8, sub);
            AddQuadTris(c - 4, sub);
            AddInverseQuadTris(c, sub);
        }

        private void AddQuadTris(int c, int s) {
            tris[s].Add(c - 4);
            tris[s].Add(c - 2);
            tris[s].Add(c - 3);
            tris[s].Add(c - 4);
            tris[s].Add(c - 1);
            tris[s].Add(c - 2);
        }
        private void AddQuadTris(int s) =>
            AddQuadTris(verts.Count, s);

        private void AddInverseQuadTris(int c, int s) {
            tris[s].Add(c - 4);
            tris[s].Add(c - 3);
            tris[s].Add(c - 2);
            tris[s].Add(c - 4);
            tris[s].Add(c - 2);
            tris[s].Add(c - 1);
        }
    }

}