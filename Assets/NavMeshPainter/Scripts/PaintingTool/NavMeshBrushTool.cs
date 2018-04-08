using UnityEngine;
using System.Collections;

namespace ASL.NavMeshPainter
{
    [System.Serializable]
    public class NavMeshBrushTool : IPaintingTool
    {
        public enum BrushType
        {
            Circular = 0,
            Square = 1,
        }

        public float size;
        public Vector3 position;
        public float maxHeight;
        public BrushType brushType;

        public Bounds Bounds
        {
            get { return new Bounds(position, new Vector3(size*2, maxHeight*2, size*2)); }
        }

        public void DrawToolGizmos()
        {
            Bounds.DrawBounds(Color.blue);
        }

        public void DrawTool(Material renderMaterial)
        {
            renderMaterial.SetVector("_BrushPos", position);
            renderMaterial.SetVector("_BrushSize", new Vector3(size, maxHeight, (float)brushType));
        }

        public bool IntersectsBounds(Bounds bounds)
        {
            return this.Bounds.Intersects(bounds);
        }

        public bool IntersectsTriangle(NavMeshTriangleNode node)
        {
            if (brushType == BrushType.Circular)
                return InteresectsTriangleByCircular(node);
            else
                return InteresectsTriangleBySquareBrush(node);
        }

        private bool InteresectsTriangleBySquareBrush(NavMeshTriangleNode node)
        {
            Vector3 size = node.max - node.min;
            Vector3 center = node.min + size*0.5f;
            Bounds bd = new Bounds(center, size);
            return bd.Intersects(this.Bounds);
        }

        private bool InteresectsTriangleByCircular(NavMeshTriangleNode node)
        {
            if (node.max.y < position.y - maxHeight || node.min.y > position.y + maxHeight)
                return false;
            Vector2 nearestpos = default(Vector2);
            nearestpos.x = Mathf.Clamp(position.x, node.min.x, node.max.x);
            nearestpos.y = Mathf.Clamp(position.z, node.min.z, node.max.z);
            float dis = Vector2.Distance(nearestpos, new Vector2(position.x, position.z));

            if (dis > size)
                return false;
            return true;
        }
    }
}