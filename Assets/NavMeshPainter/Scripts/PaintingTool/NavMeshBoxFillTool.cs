using UnityEngine;
using System.Collections;

namespace ASL.NavMeshPainter
{
    [System.Serializable]
    public class NavMeshBoxFillTool : IPaintingTool
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