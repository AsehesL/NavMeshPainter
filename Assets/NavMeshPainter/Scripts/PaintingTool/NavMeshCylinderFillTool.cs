using UnityEngine;
using System.Collections;
using System;

namespace ASL.NavMeshPainter
{
    [System.Serializable]
    public class NavMeshCylinderFillTool : IPaintingTool
    {

        public void DrawToolGizmos()
        {
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