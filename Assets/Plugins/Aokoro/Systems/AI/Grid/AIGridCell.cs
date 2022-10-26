using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using NaughtyAttributes;

using HullDelaunayVoronoi.Delaunay;
using HullDelaunayVoronoi.Primitives;

namespace Aokoro.AI.Grid
{
    [System.Serializable]
    public struct AIGridCell
    {
        [ShowNonSerializedField]
        public Vector3[] Points;
        [ShowNonSerializedField]
        public int[] Triangles;

        public int TriangleCount => Triangles.Length / 3;

        public float[] Areas { private set; get; }

        public readonly float TotalArea;

        public int layer;


        public AIGridCell(int layer, params Vector3[] points)
        {
            this.layer = layer;

            this.Points = points;
            this.Triangles = BuildTriangles(this.Points);

            this.Areas = CalculateArea(this.Points, this.Triangles);

            TotalArea = 0;
            for (int i = 0; i < Areas.Length; i++)
                TotalArea += Areas[i];

        }

        private int[] GetTriangleIndicesWithIndex(int index) => new int[] { Triangles[index * 3], Triangles[index * 3 + 1], Triangles[index * 3 + 2] };
        private Vector3[] GetTriangleWithIndex(int index)
        {
            int[] indices = GetTriangleIndicesWithIndex(index);
            return new Vector3[] { Points[indices[0]], Points[indices[1]], Points[indices[2]] };
        }

        private static float[] CalculateArea(Vector3[] points, int[] triangles)
        {
            int triangleCount = triangles.Length / 3;
            float[] areas = new float[triangleCount];

            for (int i = 0; i < triangleCount; i++)
            {
                int triangleIndex = i * 3;

                Vector3 p1 = points[triangles[triangleIndex]];
                Vector3 p2 = points[triangles[triangleIndex + 1]];
                Vector3 p3 = points[triangles[triangleIndex + 2]];

                areas[i] = MathUtility.TriangleArea(p1, p2, p3);
            }

            return areas;
        }


        private static int[] BuildTriangles(Vector3[] points)
        {
            if (points.Length > 3)
            {

                List<Vector3> pointsList = new List<Vector3>(points);
                DelaunayTriangulation3 tri = new DelaunayTriangulation3();

                List<Vertex3> vertexList = points.Select((p, i) => new Vertex3(p.x, p.y, p.z, i)).ToList();
                tri.Generate(vertexList);

                IList<DelaunayCell<Vertex3>> cells = tri.Cells;

                int count = cells.Count;
                int[] triangles = new int[count * 3];

                //Loop through the cells
                for (int i = 0; i < count; i++)
                {
                    Vertex3[] vertices = cells[i].Simplex.Vertices;

                    //Loop through the triangle vertices
                    for (int j = 0; j < 3; j++)
                    {
                        Vertex3 vertex = vertices[j];
                        Vector3 v = new Vector3(vertex.X, vertex.Y, vertex.Z);
                        int p_index = pointsList.IndexOf(v);
                        triangles[i * 3 + j] = p_index;
                    }
                }
                return triangles;
            }
            else if (points.Length == 3)
            {
                return new int[] { 0, 1, 2 };
            }
            else
                throw new System.Exception("Grid cells should have at least 3 points");
        }


        public Vector3 GetRandomPointInCell()
        {
            var triangle = GetTriangleWithIndex(GetRandomTriangle());
            return RandomWithinTriangle(triangle[0], triangle[1], triangle[2]);
        }

        private int GetRandomTriangle()
        {
            int triangleCount = TriangleCount;
            var rng = Random.Range(0, TotalArea);

            for (int i = 0; i < triangleCount; i++)
            {
                float area = Areas[i];

                if (rng < area)
                    return i;

                rng -= area;
            }

            return triangleCount - 1;
        }

        private Vector3 RandomWithinTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float r1 = Mathf.Sqrt(Random.Range(0f, 1f));
            float r2 = Random.Range(0f, 1f);
            float m1 = 1 - r1;
            float m2 = r1 * (1 - r2);
            float m3 = r2 * r1;

            return (m1 * p1) + (m2 * p2) + (m3 * p3);
        }
    }
}
