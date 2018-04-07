using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ASL.NavMeshPainter
{
    [System.Serializable]
    public class NavMeshTriangle
    {
        public Vector3 vertex0
        {
            get
            {
                if (m_NodeLists == null || m_NodeLists.Count < 1)
                    return default(Vector3);
                return m_NodeLists[0].vertex0;
            }
        }

        public Vector3 vertex1
        {
            get
            {
                if (m_NodeLists == null || m_NodeLists.Count < 1)
                    return default(Vector3);
                return m_NodeLists[0].vertex1;
            }
        }

        public Vector3 vertex2
        {
            get
            {
                if (m_NodeLists == null || m_NodeLists.Count < 1)
                    return default(Vector3);
                return m_NodeLists[0].vertex2;
            }
        }

        /// <summary>
        /// 节点列表
        /// </summary>
        [SerializeField]
        private List<NavMeshTriangleNode> m_NodeLists;

        /// <summary>
        /// 三角形的AABB包围盒
        /// </summary>
        public Bounds bounds { get { return m_Bounds; } }

        [SerializeField]
        private Bounds m_Bounds;

        public NavMeshTriangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, float area)
        {
            NavMeshTriangleNode root = new NavMeshTriangleNode(vertex0, vertex1, vertex2);
            m_NodeLists = new List<NavMeshTriangleNode>();
            m_NodeLists.Add(root);

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

            float currentArea = Vector3.Cross(vertex1 - vertex0, vertex2 - vertex0).magnitude*0.5f;
            int depth = Mathf.RoundToInt(currentArea/area*0.25f);
            

            root.Subdivide(depth, m_NodeLists);
        }

        public void Check()
        {
            if (m_NodeLists.Count >= 1)
                m_NodeLists[0].Check(m_NodeLists);
        }

        public void CheckTriangle()
        {
            if (m_NodeLists.Count >= 1)
                m_NodeLists[0].CheckTriangle(m_NodeLists);
        }


        public void GenerateMesh(List<Vector3> vlist, List<int> ilist)
        {
            if (m_NodeLists.Count >= 1)
                m_NodeLists[0].GenerateMesh(m_NodeLists, vlist, ilist);
        }

        public void Draw(NavMeshBrush brush, bool clear)
        {
            if (m_NodeLists.Count >= 1)
                m_NodeLists[0].Paint(!clear, brush.position, brush.size, brush.maxHeight, m_NodeLists);
        }

        public void DrawTriangle()
        {
            if (m_NodeLists.Count >= 1)
                m_NodeLists[0].DrawTriangle(m_NodeLists);
        }
    }
}