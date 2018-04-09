using UnityEngine;
using System.Collections;

namespace ASL.NavMeshPainter
{
    /// <summary>
    /// 绘画工具接口
    /// </summary>
    public interface IPaintingTool
    {

        bool IntersectsBounds(Bounds bounds);

        bool IntersectsTriangle(NavMeshTriangleNode node);

        bool OnMouseDown(Vector3 position);
        bool OnMouseUp();
        void OnMouseMove(Vector3 position);
        bool OnMouseDrag(Vector3 position);
        void ResetState();
    }
}