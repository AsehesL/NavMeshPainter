using UnityEngine;
using System.Collections;

namespace ASL.NavMeshPainter
{
    [System.Serializable]
    public class NavMeshLineTool : IPaintingTool
    {
        public float width;
        public Vector3 beginPos;
        public Vector3 endPos;
        public float maxHeight;

        public bool IntersectsBounds(Bounds bounds)
        {
            return false;
        }

        public bool IntersectsTriangle(NavMeshTriangleNode node)
        {
            return false;
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
        }

        public bool OnMouseDrag(Vector3 position)
        {
            return false;
        }

        public void ResetState()
        {
        }
    }
}