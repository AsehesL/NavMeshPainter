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

        private NavMeshTriangleNode m_CenterNode;
        private NavMeshTriangleNode m_TopNode;
        private NavMeshTriangleNode m_LeftNode;
        private NavMeshTriangleNode m_RightNode;

        public NavMeshTriangleNode(Vector2 weight0, Vector2 weight1, Vector2 weight2, bool isBePainted = false)
        {
            this.weight0 = weight0;
            this.weight1 = weight1;
            this.weight2 = weight2;
            this.isBePainted = isBePainted;
        }

        public void Init(List<NavMeshTriangleNode> nodelist)
        {
            //Linear to Linked
            {
                m_CenterNode = m_CenterNodeIndex >= 0 && m_CenterNodeIndex < nodelist.Count
                    ? nodelist[m_CenterNodeIndex]
                    : null;
                m_TopNode = m_TopNodeIndex >= 0 && m_TopNodeIndex < nodelist.Count
                    ? nodelist[m_TopNodeIndex]
                    : null;
                m_LeftNode = m_LeftNodeIndex >= 0 && m_LeftNodeIndex < nodelist.Count
                    ? nodelist[m_LeftNodeIndex]
                    : null;
                m_RightNode = m_RightNodeIndex >= 0 && m_RightNodeIndex < nodelist.Count
                    ? nodelist[m_RightNodeIndex]
                    : null;

                if (m_CenterNode != null)
                    m_CenterNode.Init(nodelist);
                if (m_TopNode != null)
                    m_TopNode.Init(nodelist);
                if (m_LeftNode != null)
                    m_LeftNode.Init(nodelist);
                if (m_RightNode != null)
                    m_RightNode.Init(nodelist);
            }
        }

        public void Save(List<NavMeshTriangleNode> nodelist)
        {
            //Linked to Linear
            if (m_CenterNode != null)
            {
                m_CenterNodeIndex = nodelist.Count;
                nodelist.Add(m_CenterNode);
                m_CenterNode.Save(nodelist);
            }
            else
            {
                m_CenterNodeIndex = -1;
            }
            if (m_TopNode != null)
            {
                m_TopNodeIndex = nodelist.Count;
                nodelist.Add(m_TopNode);
                m_TopNode.Save(nodelist);
            }
            else
            {
                m_TopNodeIndex = -1;
            }
            if (m_LeftNode != null)
            {
                m_LeftNodeIndex = nodelist.Count;
                nodelist.Add(m_LeftNode);
                m_LeftNode.Save(nodelist);
            }
            else
            {
                m_LeftNodeIndex = -1;
            }
            if (m_RightNode != null)
            {
                m_RightNodeIndex = nodelist.Count;
                nodelist.Add(m_RightNode);
                m_RightNode.Save(nodelist);
            }
            else
            {
                m_RightNodeIndex = -1;
            }

        }



        public void Interesect(IPaintingTool tool, bool erase, Vector3 v0, Vector3 v1, Vector3 v2, int depth, int maxDepth)
        {
            Vector3 vertex0 = (1 - weight0.x - weight0.y) * v0 + weight0.x * v1 + weight0.y * v2;
            Vector3 vertex1 = (1 - weight1.x - weight1.y) * v0 + weight1.x * v1 + weight1.y * v2;
            Vector3 vertex2 = (1 - weight2.x - weight2.y) * v0 + weight2.x * v1 + weight2.y * v2;
            if (tool.IntersectsTriangle(vertex0, vertex1, vertex2))
            {
                bool cotinuePaint = isBePainted == erase || isMix;
                if (!cotinuePaint)
                    return;
                
                if (depth <= maxDepth)
                {
                    Vector2 halfw01 = weight0 + (weight1 - weight0) * 0.5f;
                    Vector2 halfw02 = weight0 + (weight2 - weight0) * 0.5f;
                    Vector2 halfw12 = weight1 + (weight2 - weight1) * 0.5f;
                    if (m_CenterNode == null)
                        m_CenterNode = new NavMeshTriangleNode(halfw01, halfw12, halfw02, isBePainted);
                    if (m_TopNode == null)
                        m_TopNode = new NavMeshTriangleNode(halfw01, weight1, halfw12, isBePainted);
                    if (m_LeftNode == null)
                        m_LeftNode = new NavMeshTriangleNode(weight0, halfw01, halfw02, isBePainted);
                    if (m_RightNode == null)
                        m_RightNode = new NavMeshTriangleNode(halfw02, halfw12, weight2, isBePainted);
                }

                if (m_CenterNode != null)
                    m_CenterNode.Interesect(tool, erase, v0, v1, v2, depth + 1, maxDepth);

                if (m_TopNode != null)
                    m_TopNode.Interesect(tool, erase, v0, v1, v2, depth + 1, maxDepth);

                if (m_LeftNode != null)
                    m_LeftNode.Interesect(tool, erase, v0, v1, v2, depth + 1, maxDepth);

                if (m_RightNode != null)
                    m_RightNode.Interesect(tool, erase, v0, v1, v2, depth + 1, maxDepth);

                ResetPaintMark(!erase);
            }

        }

        public void DrawTriangleGizmos(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            if (isMix)
            {
                if (m_CenterNode != null)
                    m_CenterNode.DrawTriangleGizmos(v0, v1, v2);
                if (m_TopNode != null)
                    m_TopNode.DrawTriangleGizmos(v0, v1, v2);
                if (m_LeftNode != null)
                    m_LeftNode.DrawTriangleGizmos(v0, v1, v2);
                if (m_RightNode != null)
                    m_RightNode.DrawTriangleGizmos(v0, v1, v2);
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

        internal void GenerateMesh(Vector3 v0, Vector3 v1, Vector3 v2, Vector2 u0, Vector2 u1, Vector2 u2, List<NavMeshRenderTriangle> triangles)
        {
            if (!isMix)
            {
                if (isBePainted)
                {
                    Vector3 vertex0 = (1 - weight0.x - weight0.y) * v0 + weight0.x * v1 + weight0.y * v2;
                    Vector3 vertex1 = (1 - weight1.x - weight1.y) * v0 + weight1.x * v1 + weight1.y * v2;
                    Vector3 vertex2 = (1 - weight2.x - weight2.y) * v0 + weight2.x * v1 + weight2.y * v2;

                    Vector2 uv0 = (1 - weight0.x - weight0.y) * u0 + weight0.x * u1 + weight0.y * u2;
                    Vector2 uv1 = (1 - weight1.x - weight1.y) * u0 + weight1.x * u1 + weight1.y * u2;
                    Vector2 uv2 = (1 - weight2.x - weight2.y) * u0 + weight2.x * u1 + weight2.y * u2;

                    NavMeshRenderTriangle triangle = null;
                    if (triangles.Count <= 0)
                    {
                        triangle = new NavMeshRenderTriangle();
                        triangles.Add(triangle);
                    }
                    else
                    {
                        triangle = triangles[triangles.Count - 1];
                    }
                    if (triangles.Count >= 64990)
                    {
                        triangle = new NavMeshRenderTriangle();
                        triangles.Add(triangle);
                    }

                    triangle.AddVertex(vertex0, uv0);
                    triangle.AddVertex(vertex1, uv1);
                    triangle.AddVertex(vertex2, uv2);

//                    ilist.Add(vlist.Count);
//                    vlist.Add(vertex0);
//                    ilist.Add(vlist.Count);
//                    vlist.Add(vertex1);
//                    ilist.Add(vlist.Count);
//                    vlist.Add(vertex2);
                }
            }
            else
            {
                if (m_CenterNode != null)
                    m_CenterNode.GenerateMesh(v0, v1, v2, u0, u1, u2, triangles);
                if (m_TopNode != null)
                    m_TopNode.GenerateMesh(v0, v1, v2, u0, u1, u2, triangles);
                if (m_LeftNode != null)
                    m_LeftNode.GenerateMesh(v0, v1, v2, u0, u1, u2, triangles);
                if (m_RightNode != null)
                    m_RightNode.GenerateMesh(v0, v1, v2, u0, u1, u2, triangles);
            }
        }



        public void SamplingFromTexture(Vector2 u0, Vector2 u1, Vector2 u2, Texture2D texture, TextureBlendMode blendMode, int depth, int maxDepth)
        {
           
            bool continueSample = false;
            bool isInMask = SamplingTexture(u0, u1, u2, texture);
            bool painted = isBePainted;
            if (blendMode == TextureBlendMode.Add)
            {
                if (isInMask)
                {
                    isBePainted = true;
                    continueSample = true;
                }
            }
            else
            {
                if (isBePainted != isInMask)
                    continueSample = true;
                isBePainted = isInMask;
            }
            if (!continueSample)
                return;

            if (depth <= maxDepth)
            {
                Vector2 halfw01 = weight0 + (weight1 - weight0) * 0.5f;
                Vector2 halfw02 = weight0 + (weight2 - weight0) * 0.5f;
                Vector2 halfw12 = weight1 + (weight2 - weight1) * 0.5f;
                if (m_CenterNode == null)
                    m_CenterNode = new NavMeshTriangleNode(halfw01, halfw12, halfw02, painted);
                if (m_TopNode == null)
                    m_TopNode = new NavMeshTriangleNode(halfw01, weight1, halfw12, painted);
                if (m_LeftNode == null)
                    m_LeftNode = new NavMeshTriangleNode(weight0, halfw01, halfw02, painted);
                if (m_RightNode == null)
                    m_RightNode = new NavMeshTriangleNode(halfw02, halfw12, weight2, painted);
            }

            if (m_CenterNode != null)
                m_CenterNode.SamplingFromTexture(u0, u1, u2, texture, blendMode, depth + 1, maxDepth);

            if (m_TopNode != null)
                m_TopNode.SamplingFromTexture(u0, u1, u2, texture, blendMode, depth + 1, maxDepth);

            if (m_LeftNode != null)
                m_LeftNode.SamplingFromTexture(u0, u1, u2, texture, blendMode, depth + 1, maxDepth);

            if (m_RightNode != null)
                m_RightNode.SamplingFromTexture(u0, u1, u2, texture, blendMode, depth + 1, maxDepth);
            

            ResetPaintMark(isBePainted);
        }

        private void ResetPaintMark(bool paint)
        {
            isBePainted = paint;
            if (m_CenterNode != null && (m_CenterNode.isBePainted != paint || m_CenterNode.isMix))
            {
                isMix = true;
                return;
            }
            if (m_TopNode != null && (m_TopNode.isBePainted != paint || m_TopNode.isMix))
            {
                isMix = true;
                return;
            }
            if (m_LeftNode != null && (m_LeftNode.isBePainted != paint || m_LeftNode.isMix))
            {
                isMix = true;
                return;
            }
            if (m_RightNode != null && (m_RightNode.isBePainted != paint || m_RightNode.isMix))
            {
                isMix = true;
                return;
            }
            isMix = false;

            m_CenterNode = null;
            m_TopNode = null;
            m_LeftNode = null;
            m_RightNode = null;
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