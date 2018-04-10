using UnityEngine;
using System.Collections;

namespace ASL.NavMesh
{
    /// <summary>
    /// 线条绘制工具
    /// </summary>
    [System.Serializable]
    public class NavMeshLineTool : IPaintingTool
    {

        public float width;
        public Vector3 beginPos;
        public Vector3 endPos;
        public float height;

        public bool IntersectsBounds(Bounds bounds)
        {
            Vector3 toEnd = endPos - beginPos;

            Vector3 hVector = Vector3.Cross(toEnd, Vector3.up).normalized;

            Vector3 p0 = beginPos - hVector * width * 0.5f - Vector3.up * height;
            Vector3 p1 = beginPos - hVector * width * 0.5f + Vector3.up * height;
            Vector3 p2 = beginPos + hVector * width * 0.5f + Vector3.up * height;
            Vector3 p3 = beginPos + hVector * width * 0.5f - Vector3.up * height;

            Vector3 p4 = endPos - hVector * width * 0.5f - Vector3.up * height;
            Vector3 p5 = endPos - hVector * width * 0.5f + Vector3.up * height;
            Vector3 p6 = endPos + hVector * width * 0.5f + Vector3.up * height;
            Vector3 p7 = endPos + hVector * width * 0.5f - Vector3.up * height;

            Vector3 max = Vector3.Max(p0, p1);
            max = Vector3.Max(max, p2);
            max = Vector3.Max(max, p3);
            max = Vector3.Max(max, p4);
            max = Vector3.Max(max, p5);
            max = Vector3.Max(max, p6);
            max = Vector3.Max(max, p7);

            Vector3 min = Vector3.Min(p0, p1);
            min = Vector3.Min(min, p2);
            min = Vector3.Min(min, p3);
            min = Vector3.Min(min, p4);
            min = Vector3.Min(min, p5);
            min = Vector3.Min(min, p6);
            min = Vector3.Min(min, p7);

            Vector3 size = max - min;
            Vector3 center = min + size*0.5f;
            Bounds bd = new Bounds(center, size);

            return bd.Intersects(bounds);
        }

        public bool IntersectsTriangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
        {
            Vector3 toEnd = (endPos - beginPos);
            Vector3 center = beginPos + toEnd * 0.5f;
            Vector3 zaxis = toEnd.normalized;
            Vector3 xaxis = Vector3.Cross(zaxis, Vector3.up).normalized;
            Vector3 yaxis = Vector3.Cross(xaxis, zaxis).normalized;

            Matrix4x4 m = default(Matrix4x4);
            m.m00 = xaxis.x;
            m.m01 = yaxis.x;
            m.m02 = zaxis.x;
            m.m03 = center.x;

            m.m10 = xaxis.y;
            m.m11 = yaxis.y;
            m.m12 = zaxis.y;
            m.m13 = center.y;

            m.m20 = xaxis.z;
            m.m21 = yaxis.z;
            m.m22 = zaxis.z;
            m.m23 = center.z;

            m.m30 = 0;
            m.m31 = 0;
            m.m32 = 0;
            m.m33 = 1;

            m = m.inverse;

            Vector3 p0 = m.MultiplyPoint(vertex0);
            Vector3 p1 = m.MultiplyPoint(vertex1);
            Vector3 p2 = m.MultiplyPoint(vertex2);

            Vector3 max = Vector3.Max(p2, Vector3.Max(p0, p1));
            Vector3 min = Vector3.Min(p2, Vector3.Min(p0, p1));

            Vector3 tsize = max - min;
            Vector3 tcenter = min + (tsize)*0.5f;

            Bounds tbd = new Bounds(tcenter, tsize);

            Bounds lbd = new Bounds(Vector3.zero, new Vector3(width, height, toEnd.magnitude));

            return lbd.Intersects(tbd);
        }
    }
}