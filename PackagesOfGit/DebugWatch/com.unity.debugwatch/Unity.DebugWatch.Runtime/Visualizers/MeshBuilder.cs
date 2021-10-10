
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.DebugWatch.Visualizers.InternalDetails
{

    public class MeshBuilder
    {
        //public List<Mesh> meshes = new List<Mesh>();
        public List<Triangle> triangle = new List<Triangle>();
        public List<Line> line = new List<Line>();
        public class MeshData
        {
            public const int kVertexPerTriangle = 3;
            public const int kIndexPerTriangle = 3;
            public const int kVertexPerLine = 2;
            public const int kIndexPerLine = 2;
            public Vector3[] vertex;
            public Color[] color;
            public int[] index;
            public struct SubMesh
            {
                public int IndexBase;
                public int IndexCount;
                public UnityEngine.MeshTopology Topology;
            }
            public List<SubMesh> SubMeshes = new List<SubMesh>();
            public int cur_Vertex;
            public int cur_Index;
            public MeshData(int lineCount, int triangleCount)
            {
                int vertexCount = triangleCount * 3 + lineCount * 2;
                int indexCount = triangleCount * 3 + lineCount * 2;
                vertex = new Vector3[vertexCount];
                color = new Color[vertexCount];
                index = new int[indexCount];
                index = new int[indexCount];
                cur_Vertex = 0;
                cur_Index = 0;
            }

            public void Add(List<Triangle> elems, int iFirst, int iLast)
            {
                var sm = new SubMesh() { IndexBase = cur_Index, Topology = MeshTopology.Triangles };
                int count = iLast - iFirst;
                for (int i = 0; i != count; ++i)
                {
                    var e = elems[iFirst + i];
                    int i3 = i * 3;
                    vertex[cur_Vertex + i3 + 0] = e.pos0;
                    vertex[cur_Vertex + i3 + 1] = e.pos1;
                    vertex[cur_Vertex + i3 + 2] = e.pos2;

                    color[cur_Vertex + i3 + 0] = e.color0;
                    color[cur_Vertex + i3 + 1] = e.color1;
                    color[cur_Vertex + i3 + 2] = e.color2;

                    index[cur_Index + i3 + 0] = cur_Vertex + i3 + 0;
                    index[cur_Index + i3 + 1] = cur_Vertex + i3 + 1;
                    index[cur_Index + i3 + 2] = cur_Vertex + i3 + 2;
                }
                cur_Vertex += count * 3;
                cur_Index += count * 3;
                sm.IndexCount = cur_Index - sm.IndexBase;
                SubMeshes.Add(sm);
            }
            public void Add(List<Line> elems, int iFirst, int iLast)
            {
                var sm = new SubMesh() { IndexBase = cur_Index, Topology = MeshTopology.Lines };
                int count = iLast - iFirst;
                for (int i = 0; i != count; ++i)
                {
                    var e = elems[iFirst + i];
                    int i2 = i * 2;
                    vertex[cur_Vertex + i2 + 0] = e.pos0;
                    vertex[cur_Vertex + i2 + 1] = e.pos1;

                    color[cur_Vertex + i2 + 0] = e.color0;
                    color[cur_Vertex + i2 + 1] = e.color1;

                    index[cur_Index + i2 + 0] = cur_Vertex + i2 + 0;
                    index[cur_Index + i2 + 1] = cur_Vertex + i2 + 1;
                }
                cur_Vertex += count * 2;
                cur_Index += count * 2;
                sm.IndexCount = cur_Index - sm.IndexBase;
                SubMeshes.Add(sm);
            }


            public Mesh CreateMesh(Mesh meshToUse = null)
            {
                Mesh mesh = meshToUse;
                if (mesh == null)
                {
                    mesh = new Mesh();
                    mesh.hideFlags = HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy | HideFlags.NotEditable;
                }
                mesh.vertices = vertex;
                mesh.colors = color;
                mesh.subMeshCount = SubMeshes.Count;
                int iSubMesh = 0;
                foreach (var sm in SubMeshes)
                {
                    mesh.SetIndices(index, sm.IndexBase, sm.IndexCount, sm.Topology, iSubMesh);
                    ++iSubMesh;
                }

                return mesh;
            }
        }

        const int kMaxVertex = 30000;
        const int kMaxIndex = 30000;
        private int ProcessShapes(ref int curAvailableShape, ref int curAvailableVertex, ref int curAvailableIndex, int VertexPerShape, int IndexPerShape)
        {
            var maxForVertex = curAvailableVertex / VertexPerShape;
            var maxForIndex = curAvailableIndex / IndexPerShape;
            var numToProcess = Math.Min(Math.Min(maxForIndex, maxForVertex), curAvailableShape);
            if (numToProcess > 0)
            {
                curAvailableVertex -= numToProcess * VertexPerShape;
                curAvailableIndex -= numToProcess * IndexPerShape;
                curAvailableShape -= numToProcess;
            }
            return numToProcess;
        }

        public List<Mesh> BuildMesh(List<Mesh> meshesToUse = null)
        {
            var meshes = new List<Mesh>();
            int curTriangle = 0;
            int curAvailableTriangle = triangle.Count;
            int curAvailableVertex = kMaxVertex;
            int curAvailableIndex = kMaxIndex;
            int curLine = 0;
            int curAvailableLine = line.Count;
            int curMesh = 0;
            while (true)
            {
                int numTriangle = ProcessShapes(ref curAvailableTriangle, ref curAvailableVertex, ref curAvailableIndex, MeshData.kVertexPerTriangle, MeshData.kIndexPerTriangle);
                int numLine = ProcessShapes(ref curAvailableLine, ref curAvailableVertex, ref curAvailableIndex, MeshData.kVertexPerLine, MeshData.kIndexPerLine);

                if (numTriangle > 0 || numLine > 0)
                {
                    MeshData md = new MeshData(numLine, numTriangle);
                    if (numTriangle > 0)
                    {
                        md.Add(triangle, curTriangle, curTriangle + numTriangle);
                        curTriangle += numTriangle;
                    }
                    if (numLine > 0)
                    {
                        md.Add(line, curLine, curLine + numLine);
                        curLine += numLine;
                    }
                    meshes.Add(md.CreateMesh(
                        meshesToUse != null && meshesToUse.Count > curMesh
                        ? meshesToUse[curMesh]
                        : null
                    ));
                    curAvailableVertex = kMaxVertex;
                    curAvailableIndex = kMaxIndex;
                    ++curMesh;
                }
                else
                {
                    break;
                }
            }
            if (meshesToUse != null)
            {
                for (int i = curMesh; i != meshesToUse.Count; ++i)
                {
                    UnityEngine.Object.DestroyImmediate(meshesToUse[i]);
                    meshesToUse[i] = null;
                }
            }
            return meshes;
        }
        public void Clear()
        {
            triangle.Clear();
            line.Clear();
        }
        public struct Triangle
        {
            public Triangle(Vector3 a, Vector3 b, Vector3 c, Color color)
            {
                pos0 = a;
                pos1 = b;
                pos2 = c;
                color0 = color;
                color1 = color;
                color2 = color;
            }

            public Triangle(Vector3 a, Vector3 b, Vector3 c, Color colorA, Color colorB, Color colorC)
            {
                pos0 = a;
                pos1 = b;
                pos2 = c;
                color0 = colorA;
                color1 = colorB;
                color2 = colorC;
            }

            public Vector3 pos0;
            public Vector3 pos1;
            public Vector3 pos2;
            public Color color0;
            public Color color1;
            public Color color2;
        }
        public struct Line
        {
            public Line(Vector3 p0, Vector3 p1, Color c)
            {
                pos0 = p0;
                pos1 = p1;
                color0 = c;
                color1 = c;
            }

            public Line(Vector3 p0, Vector3 p1, Color c0, Color c1)
            {
                pos0 = p0;
                pos1 = p1;
                color0 = c0;
                color1 = c1;
            }

            public Vector3 pos0;
            public Vector3 pos1;
            public Color color0;
            public Color color1;
        }

        public void Add(Triangle a)
        {
            triangle.Add(a);
        }

        public void Add(Line a)
        {
            line.Add(a);
        }


    }

}