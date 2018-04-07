using UnityEngine;
using System.Collections;

namespace ASL.NavMeshPainter
{
    /// <summary>
    /// Bounds扩展类
    /// </summary>
    public static class BoundsEx
    {

        /// <summary>
        /// 绘制Bounds
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="color"></param>
        public static void DrawBounds(this Bounds bounds, Color color)
        {
            Gizmos.color = color;

            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        /// <summary>
        /// 是否包含另一个Bounds
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="compareTo"></param>
        /// <returns></returns>
        public static bool IsBoundsContainsAnotherBounds(this Bounds bounds, Bounds compareTo)
        {
            if (
                !bounds.Contains(compareTo.center +
                                 new Vector3(-compareTo.size.x/2, compareTo.size.y/2, -compareTo.size.z/2)))
                return false;
            if (
                !bounds.Contains(compareTo.center +
                                 new Vector3(compareTo.size.x/2, compareTo.size.y/2, -compareTo.size.z/2)))
                return false;
            if (
                !bounds.Contains(compareTo.center +
                                 new Vector3(compareTo.size.x/2, compareTo.size.y/2, compareTo.size.z/2)))
                return false;
            if (
                !bounds.Contains(compareTo.center +
                                 new Vector3(-compareTo.size.x/2, compareTo.size.y/2, compareTo.size.z/2)))
                return false;
            if (
                !bounds.Contains(compareTo.center +
                                 new Vector3(-compareTo.size.x/2, -compareTo.size.y/2, -compareTo.size.z/2)))
                return false;
            if (
                !bounds.Contains(compareTo.center +
                                 new Vector3(compareTo.size.x/2, -compareTo.size.y/2, -compareTo.size.z/2)))
                return false;
            if (
                !bounds.Contains(compareTo.center +
                                 new Vector3(compareTo.size.x/2, -compareTo.size.y/2, compareTo.size.z/2)))
                return false;
            if (
                !bounds.Contains(compareTo.center +
                                 new Vector3(-compareTo.size.x/2, -compareTo.size.y/2, compareTo.size.z/2)))
                return false;
            return true;
        }

        /// <summary>
        /// 判断包围盒是否被相机裁剪
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static bool IsBoundsInProjector(this Bounds bounds, Matrix4x4 worldToProjection)
        {

            int code =
                ComputeOutCode(new Vector4(bounds.center.x + bounds.size.x/2, bounds.center.y + bounds.size.y/2,
                    bounds.center.z + bounds.size.z/2, 1), worldToProjection);


            code &=
                ComputeOutCode(new Vector4(bounds.center.x - bounds.size.x/2, bounds.center.y + bounds.size.y/2,
                    bounds.center.z + bounds.size.z/2, 1), worldToProjection);

            code &=
                ComputeOutCode(new Vector4(bounds.center.x + bounds.size.x/2, bounds.center.y - bounds.size.y/2,
                    bounds.center.z + bounds.size.z/2, 1), worldToProjection);

            code &=
                ComputeOutCode(new Vector4(bounds.center.x - bounds.size.x/2, bounds.center.y - bounds.size.y/2,
                    bounds.center.z + bounds.size.z/2, 1), worldToProjection);

            code &=
                ComputeOutCode(new Vector4(bounds.center.x + bounds.size.x/2, bounds.center.y + bounds.size.y/2,
                    bounds.center.z - bounds.size.z/2, 1), worldToProjection);

            code &=
                ComputeOutCode(new Vector4(bounds.center.x - bounds.size.x/2, bounds.center.y + bounds.size.y/2,
                    bounds.center.z - bounds.size.z/2, 1), worldToProjection);

            code &=
                ComputeOutCode(new Vector4(bounds.center.x + bounds.size.x/2, bounds.center.y - bounds.size.y/2,
                    bounds.center.z - bounds.size.z/2, 1), worldToProjection);

            code &=
                ComputeOutCode(new Vector4(bounds.center.x - bounds.size.x/2, bounds.center.y - bounds.size.y/2,
                    bounds.center.z - bounds.size.z/2, 1), worldToProjection);


            if (code != 0) return false;

            return true;
        }

        private static int ComputeOutCode(Vector4 pos, Matrix4x4 projection)
        {
            pos = projection*pos;
            int code = 0;
            if (pos.x < -pos.w) code |= 0x01;
            if (pos.x > pos.w) code |= 0x02;
            if (pos.y < -pos.w) code |= 0x04;
            if (pos.y > pos.w) code |= 0x08;
            if (pos.z < -pos.w) code |= 0x10;
            if (pos.z > pos.w) code |= 0x20;
            return code;
        }
    }
}