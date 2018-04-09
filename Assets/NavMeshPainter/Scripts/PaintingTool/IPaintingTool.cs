using UnityEngine;
using System.Collections;

namespace ASL.NavMesh
{
    /// <summary>
    /// 绘画工具接口
    /// </summary>
    public interface IPaintingTool
    {

        bool IntersectsBounds(Bounds bounds);

        bool IntersectsTriangle(NavMeshTriangleNode node);
       
    }
}