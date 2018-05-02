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
        public enum PaintedType
        {
            None,
            Painted,
            Mix,
        }
        
        private PaintedType m_PaintedType;
        

        private NavMeshTriangleNode m_CenterNode;
        private NavMeshTriangleNode m_TopNode;
        private NavMeshTriangleNode m_LeftNode;
        private NavMeshTriangleNode m_RightNode;

        public NavMeshTriangleNode(PaintedType painted = PaintedType.None)
        {
            this.m_PaintedType = painted;
        }

        public void Interesect(IPaintingTool tool, bool erase, Vector2 weight0, Vector2 weight1, Vector2 weight2, Vector3 v0, Vector3 v1, Vector3 v2, int depth, int maxDepth)
        {
            Vector3 vertex0 = (1 - weight0.x - weight0.y) * v0 + weight0.x * v1 + weight0.y * v2;
            Vector3 vertex1 = (1 - weight1.x - weight1.y) * v0 + weight1.x * v1 + weight1.y * v2;
            Vector3 vertex2 = (1 - weight2.x - weight2.y) * v0 + weight2.x * v1 + weight2.y * v2;
            if (tool.IntersectsTriangle(vertex0, vertex1, vertex2))
            {
                bool continuePaint = m_PaintedType == PaintedType.Mix;
                if (m_PaintedType == PaintedType.Painted && erase)
                    continuePaint = true;
                if (m_PaintedType == PaintedType.None && !erase)
                    continuePaint = true;
                if (!continuePaint)
                    return;
                
                if (depth <= maxDepth)
                {
                    
                    if (m_CenterNode == null)
                        m_CenterNode = new NavMeshTriangleNode(m_PaintedType);
                    if (m_TopNode == null)
                        m_TopNode = new NavMeshTriangleNode(m_PaintedType);
                    if (m_LeftNode == null)
                        m_LeftNode = new NavMeshTriangleNode(m_PaintedType);
                    if (m_RightNode == null)
                        m_RightNode = new NavMeshTriangleNode(m_PaintedType);
                }

                Vector2 halfw01 = weight0 + (weight1 - weight0) * 0.5f;
                Vector2 halfw02 = weight0 + (weight2 - weight0) * 0.5f;
                Vector2 halfw12 = weight1 + (weight2 - weight1) * 0.5f;

                if (m_CenterNode != null)
                    m_CenterNode.Interesect(tool, erase, halfw01, halfw12, halfw02, v0, v1, v2, depth + 1, maxDepth);

                if (m_TopNode != null)
                    m_TopNode.Interesect(tool, erase, halfw01, weight1, halfw12, v0, v1, v2, depth + 1, maxDepth);

                if (m_LeftNode != null)
                    m_LeftNode.Interesect(tool, erase, weight0, halfw01, halfw02, v0, v1, v2, depth + 1, maxDepth);

                if (m_RightNode != null)
                    m_RightNode.Interesect(tool, erase, halfw02, halfw12, weight2, v0, v1, v2, depth + 1, maxDepth);

                ResetPaintMark(!erase);
            }

        }

        public void DrawTriangleGizmos(Vector2 weight0, Vector2 weight1, Vector2 weight2, Vector3 v0, Vector3 v1, Vector3 v2, int lod, int maxDepth)
        {
            if (m_PaintedType == PaintedType.Mix)
            {
                if (lod > maxDepth)
                    return;
                Vector2 halfw01 = weight0 + (weight1 - weight0) * 0.5f;
                Vector2 halfw02 = weight0 + (weight2 - weight0) * 0.5f;
                Vector2 halfw12 = weight1 + (weight2 - weight1) * 0.5f;

                if (m_CenterNode != null)
                    m_CenterNode.DrawTriangleGizmos(halfw01, halfw12, halfw02, v0, v1, v2, lod + 1, maxDepth);
                if (m_TopNode != null)
                    m_TopNode.DrawTriangleGizmos(halfw01, weight1, halfw12, v0, v1, v2, lod + 1, maxDepth);
                if (m_LeftNode != null)
                    m_LeftNode.DrawTriangleGizmos(weight0, halfw01, halfw02, v0, v1, v2, lod + 1, maxDepth);
                if (m_RightNode != null)
                    m_RightNode.DrawTriangleGizmos(halfw02, halfw12, weight2, v0, v1, v2, lod + 1, maxDepth);
            }
            else if(m_PaintedType == PaintedType.Painted)
            {
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

        internal void GenerateMesh(Vector2 weight0, Vector2 weight1, Vector2 weight2, Vector3 v0, Vector3 v1, Vector3 v2, Vector2 u0, Vector2 u1, Vector2 u2, List<NavMeshRenderTriangle> triangles)
        {
            if (m_PaintedType == PaintedType.Painted)
            {
                //if (isBePainted)
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
                    if (triangle.vertexCount >= 64990)
                    {
                        triangle = new NavMeshRenderTriangle();
                        triangles.Add(triangle);
                    }

                    triangle.AddVertex(vertex0, uv0);
                    triangle.AddVertex(vertex1, uv1);
                    triangle.AddVertex(vertex2, uv2);
                    
                }
            }
            else if(m_PaintedType == PaintedType.Mix)
            {
                Vector2 halfw01 = weight0 + (weight1 - weight0) * 0.5f;
                Vector2 halfw02 = weight0 + (weight2 - weight0) * 0.5f;
                Vector2 halfw12 = weight1 + (weight2 - weight1) * 0.5f;
                if (m_CenterNode != null)
                    m_CenterNode.GenerateMesh(halfw01, halfw12, halfw02, v0, v1, v2, u0, u1, u2, triangles);
                if (m_TopNode != null)
                    m_TopNode.GenerateMesh(halfw01, weight1, halfw12, v0, v1, v2, u0, u1, u2, triangles);
                if (m_LeftNode != null)
                    m_LeftNode.GenerateMesh(weight0, halfw01, halfw02, v0, v1, v2, u0, u1, u2, triangles);
                if (m_RightNode != null)
                    m_RightNode.GenerateMesh(halfw02, halfw12, weight2, v0, v1, v2, u0, u1, u2, triangles);
            }
        }

        public void SamplingFromTexture(Vector2 weight0, Vector2 weight1, Vector2 weight2, Vector2 u0, Vector2 u1, Vector2 u2, Texture2D texture, int depth, int maxDepth)
        {
            if (depth <= maxDepth)
            {
                Vector2 halfw01 = weight0 + (weight1 - weight0) * 0.5f;
                Vector2 halfw02 = weight0 + (weight2 - weight0) * 0.5f;
                Vector2 halfw12 = weight1 + (weight2 - weight1) * 0.5f;
                if (m_CenterNode != null)
                {
                    m_CenterNode.SamplingFromTexture(halfw01, halfw12, halfw02, u0, u1, u2, texture, depth + 1, maxDepth);
                }
                else
                {
                    m_CenterNode = SampleTexture(halfw01, halfw12, halfw02, u0, u1, u2, texture, depth + 1, maxDepth);
                }

                if (m_TopNode != null)
                {
                    m_TopNode.SamplingFromTexture(halfw01, weight1, halfw12, u0, u1, u2, texture, depth + 1, maxDepth);
                }
                else
                {
                    m_TopNode = SampleTexture(halfw01, weight1, halfw12, u0, u1, u2, texture, depth + 1, maxDepth);
                }

                if (m_LeftNode != null)
                {
                    m_LeftNode.SamplingFromTexture(weight0, halfw01, halfw02, u0, u1, u2, texture, depth + 1, maxDepth);
                }
                else
                {
                    m_LeftNode = SampleTexture(weight0, halfw01, halfw02, u0, u1, u2, texture, depth + 1, maxDepth);
                }

                if (m_RightNode != null)
                {
                    m_RightNode.SamplingFromTexture(halfw02, halfw12, weight2, u0, u1, u2, texture, depth + 1, maxDepth);
                }
                else
                {
                    m_RightNode = SampleTexture(halfw02, halfw12, weight2, u0, u1, u2, texture, depth + 1, maxDepth);
                }

                ResetPaintMark();
            }
            else
            {
                bool painted = SampleTexture(weight0, weight1, weight2, u0, u1, u2, texture);
                //if (blendMode == TextureBlendMode.Replace)
                {
                    m_PaintedType = painted ? PaintedType.Painted : PaintedType.None;
                }
            }
        }

        public NavMeshTriangleNode SampleTexture(Vector2 w0, Vector2 w1, Vector2 w2, Vector2 u0, Vector2 u1, Vector2 u2,
            Texture2D texture, int depth, int maxDepth)
        {
            if (depth <= maxDepth)
            {
                Vector2 halfw01 = w0 + (w1 - w0) *0.5f;
                Vector2 halfw02 = w0 + (w2 - w0) *0.5f;
                Vector2 halfw12 = w1 + (w2 - w1) *0.5f;
                NavMeshTriangleNode center = SampleTexture(halfw01, halfw12, halfw02, u0, u1, u2, texture, depth + 1, maxDepth);
                NavMeshTriangleNode top = SampleTexture(halfw01, w1, halfw12, u0, u1, u2, texture, depth + 1, maxDepth);
                NavMeshTriangleNode left = SampleTexture(w0, halfw01, halfw02, u0, u1, u2, texture, depth + 1, maxDepth);
                NavMeshTriangleNode right = SampleTexture(halfw02, halfw12, w2, u0, u1, u2, texture, depth + 1, maxDepth);
                
                NavMeshTriangleNode node = new NavMeshTriangleNode();
                node.m_CenterNode = center;
                node.m_TopNode = top;
                node.m_LeftNode = left;
                node.m_RightNode = right;
                node.ResetPaintMark();
                //node.isMix = mix;
                return node;
            }
            else
            {
                NavMeshTriangleNode node = new NavMeshTriangleNode();
                if (node.SampleTexture(w0, w1, w2, u0, u1, u2, texture))
                {
                    node.m_PaintedType = PaintedType.Painted;
                    return node;
                }
                return node;
            }

           
        }
       

        private void ResetPaintMark(bool paint)
        {
            m_PaintedType = paint ? PaintedType.Painted : PaintedType.None;
            if (m_CenterNode != null && (m_CenterNode.m_PaintedType != m_PaintedType || m_CenterNode.m_PaintedType == PaintedType.Mix))
            {
                m_PaintedType = PaintedType.Mix;
                return;
            }
            if (m_TopNode != null && (m_TopNode.m_PaintedType != m_PaintedType || m_TopNode.m_PaintedType == PaintedType.Mix))
            {
                m_PaintedType = PaintedType.Mix;
                return;
            }
            if (m_LeftNode != null && (m_LeftNode.m_PaintedType != m_PaintedType || m_LeftNode.m_PaintedType == PaintedType.Mix))
            {
                m_PaintedType = PaintedType.Mix;
                return;
            }
            if (m_RightNode != null && (m_RightNode.m_PaintedType != m_PaintedType || m_RightNode.m_PaintedType == PaintedType.Mix))
            {
                m_PaintedType = PaintedType.Mix;
                return;
            }

            m_CenterNode = null;
            m_TopNode = null;
            m_LeftNode = null;
            m_RightNode = null;
        }

        private void ResetPaintMark()
        {
            bool initPaintedtype = false;
            if (m_CenterNode != null)
            {
                initPaintedtype = true;
                m_PaintedType = m_CenterNode.m_PaintedType;
            }
            if (m_TopNode != null)
            {
                if (!initPaintedtype)
                {
                    initPaintedtype = true;
                    m_PaintedType = m_TopNode.m_PaintedType;
                }
                else
                {
                    if (m_TopNode.m_PaintedType != m_PaintedType || m_TopNode.m_PaintedType == PaintedType.Mix)
                        m_PaintedType = PaintedType.Mix;
                }
            }
            if (m_LeftNode != null)
            {
                if (!initPaintedtype)
                {
                    initPaintedtype = true;
                    m_PaintedType = m_LeftNode.m_PaintedType;
                }
                else
                {
                    if (m_LeftNode.m_PaintedType != m_PaintedType || m_LeftNode.m_PaintedType == PaintedType.Mix)
                        m_PaintedType = PaintedType.Mix;
                }
            }
            if (m_RightNode != null)
            {
                if (!initPaintedtype)
                {
                    m_PaintedType = m_RightNode.m_PaintedType;
                }
                else
                {
                    if (m_RightNode.m_PaintedType != m_PaintedType || m_RightNode.m_PaintedType == PaintedType.Mix)
                        m_PaintedType = PaintedType.Mix;
                }
            }

            if (m_PaintedType != PaintedType.Mix)
            {
                m_CenterNode = null;
                m_TopNode = null;
                m_LeftNode = null;
                m_RightNode = null;
            }
        }

        private bool SampleTexture(Vector2 weight0, Vector2 weight1, Vector2 weight2, Vector2 u0, Vector2 u1, Vector2 u2, Texture2D texture)
        {
            Vector2 uv0 = (1 - weight0.x - weight0.y)*u0 + weight0.x*u1 + weight0.y*u2;
            Vector2 uv1 = (1 - weight1.x - weight1.y)*u0 + weight1.x*u1 + weight1.y*u2;
            Vector2 uv2 = (1 - weight2.x - weight2.y)*u0 + weight2.x*u1 + weight2.y*u2;

            Vector2 uv = (uv0 + uv1 + uv2)/3;

            if (SampleTexture(uv, texture))
                return true;
            return false;
        }

        private bool SampleTexture(Vector2 uv, Texture2D texture)
        {
            int x = (int)(uv.x * texture.width);
            int y = (int)(uv.y * texture.height);
            Color col = texture.GetPixel(x, y);
            if (col.r > 0.5f)
            {
                return true;
            }
            return false;
        }
    }
}