using UnityEngine;
using System.Collections;

namespace ASL.NavMesh
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class NavMeshBoxTool : IPaintingTool
    {
        public float topHeight = 0.001f;
        public float bottomHeight = 0.001f;
        public Vector3 beginPos;
        public Vector3 endPos;

        public Bounds bounds
        {
            get
            {
                Vector3 toEnd = endPos - beginPos;
                Vector3 center = beginPos + (toEnd) *0.5f;
                center.y += -bottomHeight + (topHeight + bottomHeight) * 0.5f;
                Vector3 size = new Vector3(Mathf.Abs(toEnd.x), topHeight + bottomHeight, Mathf.Abs(toEnd.z));
                return new Bounds(center, size);
            }
        }

        public bool IntersectsBounds(Bounds bounds)
        {
            return this.bounds.Intersects(bounds);
        }

        public bool IntersectsTriangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
        {
            Vector3 max = Vector3.Max(vertex0, Vector3.Max(vertex1, vertex2));
            Vector3 min = Vector3.Min(vertex0, Vector3.Min(vertex1, vertex2));
            Vector3 size = max - min;
            Vector3 center = min + size * 0.5f;
            Bounds bd = new Bounds(center, size);
            return bd.Intersects(this.bounds);
        }
    }
}