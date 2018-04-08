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

            Vector3 toEnd = endPos - beginPos;

            Vector3 hVector = Vector3.Cross(toEnd, Vector3.up).normalized;

            Vector3 p0 = beginPos - hVector*width*0.5f - Vector3.up*maxHeight;
            Vector3 p1 = beginPos - hVector*width*0.5f + Vector3.up*maxHeight;
            Vector3 p2 = beginPos + hVector*width*0.5f + Vector3.up*maxHeight;
            Vector3 p3 = beginPos + hVector*width*0.5f - Vector3.up*maxHeight;

            Vector3 p4 = endPos - hVector*width*0.5f - Vector3.up*maxHeight;
            Vector3 p5 = endPos - hVector*width*0.5f + Vector3.up*maxHeight;
            Vector3 p6 = endPos + hVector*width*0.5f + Vector3.up*maxHeight;
            Vector3 p7 = endPos + hVector*width*0.5f - Vector3.up*maxHeight;

            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p0);

            Gizmos.DrawLine(p4, p5);
            Gizmos.DrawLine(p5, p6);
            Gizmos.DrawLine(p6, p7);
            Gizmos.DrawLine(p7, p4);

            Gizmos.DrawLine(p0, p4);
            Gizmos.DrawLine(p1, p5);
            Gizmos.DrawLine(p2, p6);
            Gizmos.DrawLine(p3, p7);
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