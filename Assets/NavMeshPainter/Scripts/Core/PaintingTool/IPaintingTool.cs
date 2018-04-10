using UnityEngine;
using System.Collections;

namespace ASL.NavMesh
{
    /// <summary>
    /// 绘画工具接口
    /// </summary>
    public interface IPaintingTool
    {
        /// <summary>
        /// 和包围盒相交判断
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        bool IntersectsBounds(Bounds bounds);

        /// <summary>
        /// 和三角形相交判断
        /// </summary>
        /// <param name="vertex0"></param>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <returns></returns>
        bool IntersectsTriangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2);
       
    }
}