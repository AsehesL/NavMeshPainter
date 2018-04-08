using UnityEngine;
using System.Collections;

namespace ASL.NavMeshPainter
{
    /// <summary>
    /// 绘画工具接口
    /// </summary>
    public interface IPaintingTool
    {

        void DrawToolGizmos();

        bool IntersectsBounds(Bounds bounds);

        bool IntersectsTriangle(NavMeshTriangleNode node);
    }
}