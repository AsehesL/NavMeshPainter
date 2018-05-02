using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ASL.NavMesh
{
    /// <summary>
    /// NavMesh三角面
    /// </summary>
    [System.Serializable]
    public class NavMeshTriangle
    {

        public NMVector3 vertex0;
        public NMVector3 vertex1;
        public NMVector3 vertex2;
        public NMVector2 uv0;
        public NMVector2 uv1;
        public NMVector2 uv2;
     

        /// <summary>
        /// 三角形的AABB包围盒
        /// </summary>
        public Bounds bounds { get { return m_Bounds; } }
        
        private NMBounds m_Bounds;
        
        private int m_MaxDepth;

        private NavMeshTriangleNode m_Root;

        public NavMeshTriangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Vector2 uv0, Vector2 uv1, Vector2 uv2)
        {
            m_Root = new NavMeshTriangleNode();

            float maxX = Mathf.Max(vertex0.x, vertex1.x, vertex2.x);
            float maxY = Mathf.Max(vertex0.y, vertex1.y, vertex2.y);
            float maxZ = Mathf.Max(vertex0.z, vertex1.z, vertex2.z);

            float minX = Mathf.Min(vertex0.x, vertex1.x, vertex2.x);
            float minY = Mathf.Min(vertex0.y, vertex1.y, vertex2.y);
            float minZ = Mathf.Min(vertex0.z, vertex1.z, vertex2.z);

            Vector3 si = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
            if (si.x <= 0)
                si.x = 0.1f;
            if (si.y <= 0)
                si.y = 0.1f;
            if (si.z <= 0)
                si.z = 0.1f;
            Vector3 ct = new Vector3(minX, minY, minZ) + si / 2;

            this.m_Bounds = new Bounds(ct, si);

            this.vertex0 = vertex0;
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
            this.uv0 = uv0;
            this.uv1 = uv1;
            this.uv2 = uv2;
        }

        public NavMeshTriangle(Vector2 uv0, Vector2 uv1, Vector2 uv2)
        {
            m_Root = new NavMeshTriangleNode();

            this.uv0 = uv0;
            this.uv1 = uv1;
            this.uv2 = uv2;
        }

        public void Clear()
        {
            m_Root = new NavMeshTriangleNode();
        }

        /// <summary>
        /// 节点细分
        /// </summary>
        /// <param name="maxDepth">最大深度</param>
        /// <param name="maxArea">最大三角面积</param>
        public void SetMaxDepth(int maxDepth, float maxArea, bool forceSet = false)
        {
            if (forceSet)
            {
                m_MaxDepth = maxDepth;
                return;
            }
            //根据最大三角面积及最大深度，确定当前三角面的细分深度
            //if (m_NodeLists != null && m_NodeLists.Count >= 1)
            {
                float tcount = Mathf.Pow(4, maxDepth);
                float marea = maxArea/tcount;
                float carea = GetArea();
                for (int i = 0; i <= maxDepth; i++)
                {
                    float dp = Mathf.Pow(4, i);
                    float area = carea / dp;
                    float m = Mathf.Round(area/marea);
                    if (m <= 1.0f)
                    {
                        maxDepth = i;
                        break;
                    }
                }
                m_MaxDepth = maxDepth;
            }
        }

        /// <summary>
        /// 获得当前三角面的面积
        /// </summary>
        /// <returns></returns>
        public float GetArea()
        {
            return Vector3.Cross(vertex1 - vertex0, vertex2 - vertex0).magnitude * 0.5f;
        }


        internal void GenerateMesh(List<NavMeshRenderTriangle> triangles)
        {
            if (m_Root != null)
                m_Root.GenerateMesh(Vector2.zero, Vector2.right, Vector2.up, vertex0, vertex1, vertex2, uv0, uv1, uv2, triangles);
        }

        internal void GenerateRenderMesh(List<NavMeshRenderTriangle> triangles)
        {
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

        public void SamplingFromTexture(Texture2D texture)
        {
            if (m_Root != null)
                m_Root.SamplingFromTexture(Vector2.zero, Vector2.right, Vector2.up, uv0, uv1, uv2, texture, 0, m_MaxDepth);
        }

        public void Interesect(IPaintingTool tool, bool erase)
        {
            if (m_Root != null)
                m_Root.Interesect(tool, erase, Vector2.zero, Vector2.right, Vector2.up, vertex0, vertex1, vertex2, 0, m_MaxDepth);
        }

        public void DrawTriangleGizmos(Camera sceneViewCamera, float lodDeltaDis)
        {
            if (m_Root != null)
            {
                float dis = Vector3.Distance(sceneViewCamera.transform.position, this.bounds.center);
                int lod = (int)(dis/lodDeltaDis);
                m_Root.DrawTriangleGizmos(Vector2.zero, Vector2.right, Vector2.up, vertex0, vertex1, vertex2, lod, m_MaxDepth);
            }
        }
    }
}