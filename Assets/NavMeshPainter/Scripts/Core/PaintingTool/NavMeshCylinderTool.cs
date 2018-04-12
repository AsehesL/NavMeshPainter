using UnityEngine;
using System.Collections;

namespace ASL.NavMesh
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class NavMeshCylinderTool : IPaintingTool
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
                Vector3 center = beginPos;
                center.y += -bottomHeight + (topHeight + bottomHeight)*0.5f;
                float rad = Mathf.Sqrt(toEnd.x*toEnd.x + toEnd.z*toEnd.z);
                Vector3 size = new Vector3(rad*2, topHeight + bottomHeight, rad*2);
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
            Vector3 toEnd = endPos - beginPos;
            Vector3 center = beginPos;
            center.y = -bottomHeight + (topHeight + bottomHeight) * 0.5f;
            float rad = Mathf.Sqrt(toEnd.x * toEnd.x + toEnd.z * toEnd.z);
            if (max.y < beginPos.y - bottomHeight || min.y > beginPos.y + topHeight)
                return false;
            Vector2 nearestpos = default(Vector2);
            nearestpos.x = Mathf.Clamp(center.x, min.x, max.x);
            nearestpos.y = Mathf.Clamp(center.z, min.z, max.z);
            float dis = Vector2.Distance(nearestpos, new Vector2(center.x, center.z));

            if (dis > rad)
                return false;
            return true;
        }
    }
}