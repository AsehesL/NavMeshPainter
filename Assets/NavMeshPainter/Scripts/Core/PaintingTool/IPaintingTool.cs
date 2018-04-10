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

        bool IntersectsTriangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2);
       
    }
}