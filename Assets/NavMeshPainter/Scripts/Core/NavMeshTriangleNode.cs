using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ASL.NavMesh
{
    /// <summary>
    /// NavMesh三角面细分节点
    /// </summary>
    [System.Serializable]
    public class NavMeshTriangleNode 
    {
        #region 权重坐标公式：(1-u-v)*v0+u*v1+v*v2   u+v=1
        /// <summary>
        /// 权重坐标0
        /// </summary>
        public Vector2 weight0;
        /// <summary>
        /// 权重坐标1
        /// </summary>
        public Vector2 weight1;
        /// <summary>
        /// 权重坐标2
        /// </summary>
        public Vector2 weight2;
        #endregion

        /// <summary>
        /// 标记是否被绘制
        /// </summary>
        public bool isBePainted;
        /// <summary>
        /// 是否标记为混合：如果标记为混合，表示子节点存在颜色不相同的情况，不进行节点合并
        /// </summary>
        public bool isMix;
        
        [SerializeField] private int m_CenterNodeIndex = -1;
        [SerializeField] private int m_TopNodeIndex = -1;
        [SerializeField] private int m_LeftNodeIndex = -1;
        [SerializeField] private int m_RightNodeIndex = -1;

        public NavMeshTriangleNode(Vector2 weight0, Vector2 weight1, Vector2 weight2)
        {
            this.weight0 = weight0;
            this.weight1 = weight1;
            this.weight2 = weight2;
        }

        /// <summary>
        /// 进行细分递归
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="nodelist"></param>
        public void Subdivide(int depth, List<NavMeshTriangleNode> nodelist)
        {
            if (depth <= 0)
                return;

            Vector2 halfw01 = weight0 + (weight1 - weight0)*0.5f;
            Vector2 halfw02 = weight0 + (weight2 - weight0)*0.5f;
            Vector2 halfw12 = weight1 + (weight2 - weight1)*0.5f;

            NavMeshTriangleNode center = new NavMeshTriangleNode(halfw01, halfw12, halfw02);
            NavMeshTriangleNode top = new NavMeshTriangleNode(halfw01, weight1, halfw12);
            NavMeshTriangleNode left = new NavMeshTriangleNode(weight0, halfw01, halfw02);
            NavMeshTriangleNode right = new NavMeshTriangleNode(halfw02, halfw12, weight2);

            m_CenterNodeIndex = nodelist.Count;
            nodelist.Add(center);
            m_TopNodeIndex = nodelist.Count;
            nodelist.Add(top);
            m_LeftNodeIndex = nodelist.Count;
            nodelist.Add(left);
            m_RightNodeIndex = nodelist.Count;
            nodelist.Add(right);

            center.Subdivide(depth - 1, nodelist);
            top.Subdivide(depth - 1, nodelist);
            left.Subdivide(depth - 1, nodelist);
            right.Subdivide(depth - 1, nodelist);
        }

        public void Interesect(IPaintingTool tool, bool erase, Vector3 v0, Vector3 v1, Vector3 v2, List<NavMeshTriangleNode> nodelist)
        {
            Vector3 vertex0 = (1 - weight0.x - weight0.y) * v0 + weight0.x * v1 + weight0.y * v2;
            Vector3 vertex1 = (1 - weight1.x - weight1.y) * v0 + weight1.x * v1 + weight1.y * v2;
            Vector3 vertex2 = (1 - weight2.x - weight2.y) * v0 + weight2.x * v1 + weight2.y * v2;
            if (tool.IntersectsTriangle(vertex0, vertex1, vertex2))
            {
                bool cotinuePaint = isBePainted != !erase || isMix;
                if (!cotinuePaint)
                    return;

                NavMeshTriangleNode center = m_CenterNodeIndex >= 0 && m_CenterNodeIndex < nodelist.Count
                        ? nodelist[m_CenterNodeIndex]
                        : null;
                NavMeshTriangleNode top = m_TopNodeIndex >= 0 && m_TopNodeIndex < nodelist.Count
                    ? nodelist[m_TopNodeIndex]
                    : null;
                NavMeshTriangleNode left = m_LeftNodeIndex >= 0 && m_LeftNodeIndex < nodelist.Count
                    ? nodelist[m_LeftNodeIndex]
                    : null;
                NavMeshTriangleNode right = m_RightNodeIndex >= 0 && m_RightNodeIndex < nodelist.Count
                    ? nodelist[m_RightNodeIndex]
                    : null;

                if (center != null)
                    center.Interesect(tool, erase, v0, v1, v2, nodelist);

                if (top != null)
                    top.Interesect(tool, erase, v0, v1, v2, nodelist);

                if (left != null)
                    left.Interesect(tool, erase, v0, v1, v2, nodelist);

                if (right != null)
                    right.Interesect(tool, erase, v0, v1, v2, nodelist);

                ResetPaintMark(!erase, center, top, left, right);
            }

        }

        public void DrawTriangleGizmos(Vector3 v0, Vector3 v1, Vector3 v2, List<NavMeshTriangleNode> nodeList)
        {
            if (isMix)
            {
                NavMeshTriangleNode center = m_CenterNodeIndex >= 0 && m_CenterNodeIndex < nodeList.Count
                    ? nodeList[m_CenterNodeIndex]
                    : null;
                NavMeshTriangleNode top = m_TopNodeIndex >= 0 && m_TopNodeIndex < nodeList.Count
                    ? nodeList[m_TopNodeIndex]
                    : null;
                NavMeshTriangleNode left = m_LeftNodeIndex >= 0 && m_LeftNodeIndex < nodeList.Count
                    ? nodeList[m_LeftNodeIndex]
                    : null;
                NavMeshTriangleNode right = m_RightNodeIndex >= 0 && m_RightNodeIndex < nodeList.Count
                    ? nodeList[m_RightNodeIndex]
                    : null;
                if (center != null)
                    center.DrawTriangleGizmos(v0, v1, v2, nodeList);
                if (top != null)
                    top.DrawTriangleGizmos(v0, v1, v2, nodeList);
                if (left != null)
                    left.DrawTriangleGizmos(v0, v1, v2, nodeList);
                if (right != null)
                    right.DrawTriangleGizmos(v0, v1, v2, nodeList);
            }
            else
            {
                if (isBePainted)
                {
                    Vector3 vertex0 = (1 - weight0.x - weight0.y) * v0 + weight0.x * v1 + weight0.y * v2;
                    Vector3 vertex1 = (1 - weight1.x - weight1.y) * v0 + weight1.x * v1 + weight1.y * v2;
                    Vector3 vertex2 = (1 - weight2.x - weight2.y) * v0 + weight2.x * v1 + weight2.y * v2;

                    Gizmos.DrawLine(vertex0, vertex1);
                    Gizmos.DrawLine(vertex0, vertex2);
                    Gizmos.DrawLine(vertex1, vertex2);
                }
            }
        }

        public void GenerateMesh(Vector3 v0, Vector3 v1, Vector3 v2, List<NavMeshTriangleNode> nodeList, List<Vector3> vlist, List<int> ilist)
        {
            if (!isMix)
            {
                if (isBePainted)
                {
                    Vector3 vertex0 = (1 - weight0.x - weight0.y) * v0 + weight0.x * v1 + weight0.y * v2;
                    Vector3 vertex1 = (1 - weight1.x - weight1.y) * v0 + weight1.x * v1 + weight1.y * v2;
                    Vector3 vertex2 = (1 - weight2.x - weight2.y) * v0 + weight2.x * v1 + weight2.y * v2;

                    ilist.Add(vlist.Count);
                    vlist.Add(vertex0);
                    ilist.Add(vlist.Count);
                    vlist.Add(vertex1);
                    ilist.Add(vlist.Count);
                    vlist.Add(vertex2);
                }
            }
            else
            {
                NavMeshTriangleNode center = m_CenterNodeIndex >= 0 && m_CenterNodeIndex < nodeList.Count
                    ? nodeList[m_CenterNodeIndex]
                    : null;
                NavMeshTriangleNode top = m_TopNodeIndex >= 0 && m_TopNodeIndex < nodeList.Count
                    ? nodeList[m_TopNodeIndex]
                    : null;
                NavMeshTriangleNode left = m_LeftNodeIndex >= 0 && m_LeftNodeIndex < nodeList.Count
                    ? nodeList[m_LeftNodeIndex]
                    : null;
                NavMeshTriangleNode right = m_RightNodeIndex >= 0 && m_RightNodeIndex < nodeList.Count
                    ? nodeList[m_RightNodeIndex]
                    : null;
                if (center != null)
                    center.GenerateMesh(v0, v1, v2, nodeList, vlist, ilist);
                if (top != null)
                    top.GenerateMesh(v0, v1, v2, nodeList, vlist, ilist);
                if (left != null)
                    left.GenerateMesh(v0, v1, v2, nodeList, vlist, ilist);
                if (right != null)
                    right.GenerateMesh(v0, v1, v2, nodeList, vlist, ilist);
            }
        }

        public void SamplingFromTexture(Vector2 u0, Vector2 u1, Vector2 u2, List<NavMeshTriangleNode> nodeList, Texture2D texture, TextureBlendMode blendMode)
        {
            NavMeshTriangleNode center = m_CenterNodeIndex >= 0 && m_CenterNodeIndex < nodeList.Count
                        ? nodeList[m_CenterNodeIndex]
                        : null;
            NavMeshTriangleNode top = m_TopNodeIndex >= 0 && m_TopNodeIndex < nodeList.Count
                ? nodeList[m_TopNodeIndex]
                : null;
            NavMeshTriangleNode left = m_LeftNodeIndex >= 0 && m_LeftNodeIndex < nodeList.Count
                ? nodeList[m_LeftNodeIndex]
                : null;
            NavMeshTriangleNode right = m_RightNodeIndex >= 0 && m_RightNodeIndex < nodeList.Count
                ? nodeList[m_RightNodeIndex]
                : null;

            if (center != null)
                center.SamplingFromTexture(u0, u1, u2, nodeList, texture, blendMode);

            if (top != null)
                top.SamplingFromTexture(u0, u1, u2, nodeList, texture, blendMode);

            if (left != null)
                left.SamplingFromTexture(u0, u1, u2, nodeList, texture, blendMode);

            if (right != null)
                right.SamplingFromTexture(u0, u1, u2, nodeList, texture, blendMode);

            bool isInMask = SamplingTexture(u0, u1, u2, texture);
            if (blendMode == TextureBlendMode.Add)
            {
                if (isInMask)
                    isBePainted = true;
            }
            else
                isBePainted = isInMask;

            ResetPaintMark(isBePainted, center, top, left, right);
        }

        private void ResetPaintMark(bool paint, NavMeshTriangleNode center, NavMeshTriangleNode top, NavMeshTriangleNode left, NavMeshTriangleNode right)
        {
            isBePainted = paint;
            if (center != null && (center.isBePainted != paint || center.isMix))
            {
                isMix = true;
                return;
            }
            if (top != null && (top.isBePainted != paint || top.isMix))
            {
                isMix = true;
                return;
            }
            if (left != null && (left.isBePainted != paint || left.isMix))
            {
                isMix = true;
                return;
            }
            if (right != null && (right.isBePainted != paint || right.isMix))
            {
                isMix = true;
                return;
            }
            isMix = false;
        }

        private bool SamplingTexture(Vector2 u0, Vector2 u1, Vector2 u2, Texture2D texture)
        {
            Vector2 uv0 = (1 - weight0.x - weight0.y)*u0 + weight0.x*u1 + weight0.y*u2;
            Vector2 uv1 = (1 - weight1.x - weight1.y)*u0 + weight1.x*u1 + weight1.y*u2;
            Vector2 uv2 = (1 - weight2.x - weight2.y)*u0 + weight2.x*u1 + weight2.y*u2;

            Vector2 uv = (uv0 + uv1 + uv2)/3;
            int x = (int) (uv.x*texture.width);
            int y = (int) (uv.y*texture.height);
            Color col = texture.GetPixel(x, y);
            if (col.r > 0.5f)
            {
                return true;
            }
            return false;
        }
    }
}