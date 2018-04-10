using UnityEngine;
using System.Collections;

namespace ASL.NavMesh
{
    public enum NavMeshBrushType
    {
        Cylinder = 0,
        Sphere =  1,
        Box = 2,
    }
    [System.Serializable]
    public class NavMeshBrushTool : IPaintingTool
    {
        public float xSize;
        public float zSize;
        public Vector3 position;
        public float maxHeight;
        public NavMeshBrushType brushType;

        public Bounds Bounds
        {
            get
            {
                if (brushType == NavMeshBrushType.Box)
                    return new Bounds(position, new Vector3(xSize*2, maxHeight*2, zSize*2));
                else if (brushType == NavMeshBrushType.Cylinder)
                    return new Bounds(position, new Vector3(xSize*2, maxHeight*2, xSize*2));
                else return new Bounds(position, new Vector3(xSize*2, xSize*2, xSize*2));
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
            if (max.y < position.y - maxHeight || min.y > position.y + maxHeight)
                return false;
            Vector2 nearestpos = default(Vector2);
            nearestpos.x = Mathf.Clamp(position.x, min.x, max.x);
            nearestpos.y = Mathf.Clamp(position.z, min.z, max.z);
            float dis = Vector2.Distance(nearestpos, new Vector2(position.x, position.z));

            if (dis > xSize)
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

            if (dis > xSize)
                return false;
            return true;
        }
    }
}