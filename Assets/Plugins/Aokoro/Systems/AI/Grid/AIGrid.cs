using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using NaughtyAttributes;
using UnityEngine.AI;

namespace Aokoro.AI.Grid
{
    public class AIGrid : MonoBehaviour
    {
        public AIGridCell[] Cells => _cells;

        [SerializeField]
        private AIGridCell[] _cells;
        [SerializeField]
        int[] navMeshMask;

        [ShowNativeProperty]
        public float Area { get; private set; }

        public virtual bool IsDirty => isDirty;

        private bool isDirty;


        public void UpdateGrid()
        {
            if(IsDirty)
                BuildData();
        }

        public virtual void BuildData()
        {
            _cells = BuildGridCells();
            Area = CalculateArea();

            isDirty = false;
        }

        protected virtual AIGridCell[] BuildGridCells()
        {
            NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

            int[] areas = triangulation.areas;
            int[] indices = triangulation.indices;
            Vector3[] vertices = triangulation.vertices;

            List<AIGridCell> cells = new List<AIGridCell>();
            for (int i = 0; i < areas.Length; i++)
            {
                int area = areas[i];
                if (navMeshMask.Contains(area))
                {
                    Vector3 a = vertices[indices[3 * i]];
                    Vector3 b = vertices[indices[3 * i + 1]];
                    Vector3 c = vertices[indices[3 * i + 2]];

                    cells.Add(new AIGridCell(area, a, b, c));
                }
            }

            return cells.ToArray();
        }

        protected virtual float CalculateArea()
        {
            float area = 0;
            for (int i = 0; i < Cells.Length; i++)
                area += Cells[i].TotalArea;

            return area;
        }

        public virtual AIGridCell GetRandomCell()
        {
            var rng = Random.Range(0, Area);

            foreach (AIGridCell cell in Cells)
            {
                float area = cell.TotalArea;
                if (rng < area)
                    return cell;

                rng -= area;
            }

            return Cells.Last();
        }

        public virtual Vector3 GetRandomPosition() => GetRandomCell().GetRandomPointInCell();

#if UNITY_EDITOR
        [SerializeField] bool ShowGrid = true;
        private void OnDrawGizmosSelected()
        {
            if (!ShowGrid)
                return;

            BuildData();

            Gizmos.color = Color.green;

            //AIGridCell cell = Cells.ElementAt(showTriangle);
            foreach(AIGridCell cell in Cells)
            {
                var triangles = cell.Triangles;
                var points = cell.Points;

                for (int i = 0; i < cell.TriangleCount; i++)
                {
                    int a_i = triangles[i * 3];
                    int b_i = triangles[i * 3 + 1];
                    int c_i = triangles[i * 3 + 2];

                    Vector3 a = points[a_i];
                    Vector3 b = points[b_i];
                    Vector3 c = points[c_i];

                    Gizmos.DrawLine(a, b);
                    Gizmos.DrawLine(a, c);
                    Gizmos.DrawLine(b, c);
                }
            }
        }
#endif
    }
}
