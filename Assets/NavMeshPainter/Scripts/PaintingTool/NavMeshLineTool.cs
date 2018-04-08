using UnityEngine;
using System.Collections;
using System;

namespace ASL.NavMeshPainter
{
    [System.Serializable]
    public class NavMeshLineTool : IPaintingTool
    {
        public float width;
        public Vector3 beginPos;
        public Vector3 endPos;
        public float maxHeight;

        public void DrawToolGizmos()
        {
            Gizmos.color = Color.blue;
        }

        public bool IntersectsBounds(Bounds bounds)
        {
            return false;
        }

        public bool IntersectsTriangle(NavMeshTriangleNode node)
        {
            return false;
        }
    }
}