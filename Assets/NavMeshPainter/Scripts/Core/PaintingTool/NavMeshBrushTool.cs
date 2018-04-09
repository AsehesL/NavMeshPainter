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

        public bool IntersectsTriangle(NavMeshTriangleNode node)
        {
            if (brushType == NavMeshBrushType.Box)
                return InteresectsTriangleByBoxBrush(node);
            else if (brushType == NavMeshBrushType.Cylinder)
                return InteresectsTriangleByCylinder(node);
            else
                return InteresectsTriangleBySphere(node);
        }

        private bool InteresectsTriangleByBoxBrush(NavMeshTriangleNode node)
        {
            
            Vector3 size = node.max - node.min;
            Vector3 center = node.min + size*0.5f;
            Bounds bd = new Bounds(center, size);
            return bd.Intersects(this.Bounds);
        }

        private bool InteresectsTriangleByCylinder(NavMeshTriangleNode node)
        {
            if (node.max.y < position.y - maxHeight || node.min.y > position.y + maxHeight)
                return false;
            Vector2 nearestpos = default(Vector2);
            nearestpos.x = Mathf.Clamp(position.x, node.min.x, node.max.x);
            nearestpos.y = Mathf.Clamp(position.z, node.min.z, node.max.z);
            float dis = Vector2.Distance(nearestpos, new Vector2(position.x, position.z));

            if (dis > xSize)
                return false;
            return true;
        }

        private bool InteresectsTriangleBySphere(NavMeshTriangleNode node)
        {
            Vector3 nearestpos = default(Vector3);
            nearestpos.x = Mathf.Clamp(position.x, node.min.x, node.max.x);
            nearestpos.y = Mathf.Clamp(position.y, node.min.y, node.max.y);
            nearestpos.z = Mathf.Clamp(position.z, node.min.z, node.max.z);
            float dis = Vector3.Distance(nearestpos, position);

            if (dis > xSize)
                return false;
            return true;
        }
    }
}