using UnityEngine;
using System.Collections;

namespace ASL.NavMeshPainter
{
    [System.Serializable]
    public class NavMeshBrush : IPaintingTool
    {

        public float radius;
        public Vector3 position;
        public float maxHeight;

        public Bounds Bounds
        {
            get { return new Bounds(position, new Vector3(radius*2, maxHeight*2, radius*2)); }
        }

        public void DrawToolGizmos()
        {
            Bounds.DrawBounds(Color.blue);
        }

        public void DrawTool(Material renderMaterial)
        {
            renderMaterial.SetVector("_BrushPos", position);
            renderMaterial.SetVector("_BrushSize", new Vector2(radius, maxHeight));
        }

        public void DrawGUI()
        {
            
        }

        public bool IntersectsBounds(Bounds bounds)
        {
            return this.Bounds.Intersects(bounds);
        }

        public bool IntersectsTriangle(NavMeshTriangleNode node)
        {
            if (node.max.y < position.y - maxHeight || node.min.y > position.y + maxHeight)
                return false;
            Vector2 nearestpos = default(Vector2);
            nearestpos.x = Mathf.Clamp(position.x, node.min.x, node.max.x);
            nearestpos.y = Mathf.Clamp(position.z, node.min.z, node.max.z);
            float dis = Vector2.Distance(nearestpos, new Vector2(position.x, position.z));

            if (dis > radius)
                return false;
            return true;
        }
    }
}