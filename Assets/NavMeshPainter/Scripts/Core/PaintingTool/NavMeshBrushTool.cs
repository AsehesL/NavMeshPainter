using UnityEngine;
using System.Collections;

namespace ASL.NavMesh
{
    /// <summary>
    /// 笔刷工具
    /// </summary>
    [System.Serializable]
    public class NavMeshBrushTool : IPaintingTool
    {
        /// <summary>
        /// 笔刷类型
        /// </summary>
        public enum NavMeshBrushType
        {
            /// <summary>
            /// 圆柱形
            /// </summary>
            Cylinder = 0,
            /// <summary>
            /// 球形
            /// </summary>
            Sphere = 1,
            /// <summary>
            /// 立方体
            /// </summary>
            Box = 2,
        }

        public float length;
        public float width;
        public Vector3 position;
        public float height;
        public NavMeshBrushType brushType;

        public Bounds Bounds
        {
            get
            {
                if (brushType == NavMeshBrushType.Box)
                    return new Bounds(position, new Vector3(length, height, width));
                else if (brushType == NavMeshBrushType.Cylinder)
                    return new Bounds(position, new Vector3(length*2, height, length*2));
                else return new Bounds(position, new Vector3(length*2, length*2, length*2));
            }
        }

        public bool IntersectsBounds(Bounds bounds)
        {
            return this.Bounds.Intersects(bounds);
        }

        public bool IntersectsTriangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
        {
            Vector3 max = Vector3.Max(vertex0, Vector3.Max(vertex1, vertex2));
            Vector3 min = Vector3.Min(vertex0, Vector3.Min(vertex1, vertex2));
            if (brushType == NavMeshBrushType.Box)
                return InteresectsTriangleByBoxBrush(max, min);
            else if (brushType == NavMeshBrushType.Cylinder)
                return InteresectsTriangleByCylinder(max, min);
            else
                return InteresectsTriangleBySphere(max, min);
        }

        private bool InteresectsTriangleByBoxBrush(Vector3 max, Vector3 min)
        {
            
            Vector3 size = max - min;
            Vector3 center = min + size*0.5f;
            Bounds bd = new Bounds(center, size);
            return bd.Intersects(this.Bounds);
        }

        private bool InteresectsTriangleByCylinder(Vector3 max, Vector3 min)
        {
            if (max.y < position.y - height*0.5f || min.y > position.y + height*0.5f)
                return false;
            Vector2 nearestpos = default(Vector2);
            nearestpos.x = Mathf.Clamp(position.x, min.x, max.x);
            nearestpos.y = Mathf.Clamp(position.z, min.z, max.z);
            float dis = Vector2.Distance(nearestpos, new Vector2(position.x, position.z));

            if (dis > length)
                return false;
            return true;
        }

        private bool InteresectsTriangleBySphere(Vector3 max, Vector3 min)
        {
            Vector3 nearestpos = default(Vector3);
            nearestpos.x = Mathf.Clamp(position.x, min.x, max.x);
            nearestpos.y = Mathf.Clamp(position.y, min.y, max.y);
            nearestpos.z = Mathf.Clamp(position.z, min.z, max.z);
            float dis = Vector3.Distance(nearestpos, position);

            if (dis > length)
                return false;
            return true;
        }
    }
}