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

        private enum State
        {
            None,
            Drag,
        }

        public float size;
        public Vector3 position;
        public float maxHeight;
        public BrushType brushType;

        private State m_CurrentState;

        public Bounds Bounds
        {
            get { return new Bounds(position, new Vector3(size*2, maxHeight*2, size*2)); }
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

        public bool OnMouseDown(Vector3 position)
        {
            return false;
        }

        public bool OnMouseUp()
        {
            return false;
        }

        public void OnMouseMove(Vector3 position)
        {
            this.position = position;
        }

        public bool OnMouseDrag(Vector3 position)
        {
            this.position = position;
            return true;
        }

        public void ResetState()
        {
            m_CurrentState = State.None;
        }
    }
}