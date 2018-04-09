using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ASL.NavMesh
{
    [System.Serializable]
    public class NavMeshOcTree
    {

        public int count { get { return m_Count; } }

        /// <summary>
        /// 八叉树包围盒区域
        /// </summary>
        public Bounds Bounds
        {
            get
            {
                if (m_NodeLists != null && m_NodeLists.Count > 0)
                    return m_NodeLists[0].bounds;
                return default(Bounds);
            }
        }

        [SerializeField]
        private int m_Count;

        [SerializeField]
        private int m_MaxDepth;

        /// <summary>
        /// 节点列表
        /// </summary>
        [SerializeField]
        private List<NavMeshOcTreeNode> m_NodeLists;

        /// <summary>
        /// 构造八叉树
        /// </summary>
        /// <param name="center">八叉树中心坐标</param>
        /// <param name="size">八叉树区域大小</param>
        /// <param name="maxDepth">最大深度</param>
        public NavMeshOcTree(Vector3 center, Vector3 size, int maxDepth)
        {
            this.m_MaxDepth = maxDepth;
            this.m_NodeLists = new List<NavMeshOcTreeNode>();
            //this.m_NodeIndexList = new List<int>();
            this.m_NodeLists.Add(new NavMeshOcTreeNode(new Bounds(center, size)));
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="item"></param>
        public void Add(NavMeshTriangle item)
        {
            if (m_NodeLists != null && m_NodeLists.Count > 0)
            {
                NavMeshOcTreeNode node = m_NodeLists[0].Insert(item, 0, m_MaxDepth, m_NodeLists);
                if (node != null)
                {
                    int index = m_NodeLists.IndexOf(node);
                    if (index >= 0)
                    {
                        //m_NodeIndexList.Add(index);
                        m_Count++;
                    }
                }
            }
        }

        public Mesh GenerateMesh()
        {
            List<Vector3> vlist = new List<Vector3>();
            List<int> ilist = new List<int>();
            if (m_NodeLists != null && m_NodeLists.Count > 0)
                m_NodeLists[0].GenerateMesh(m_NodeLists, vlist, ilist);
            Mesh mesh = new Mesh();
            mesh.SetVertices(vlist);
            mesh.SetTriangles(ilist, 0);
            mesh.hideFlags = HideFlags.HideAndDontSave;
            return mesh;
        }

        public void Draw(IPaintingTool tool, bool clear = false)
        {
            if (m_NodeLists != null && m_NodeLists.Count > 0)
                m_NodeLists[0].Draw(tool, m_NodeLists, clear);
        }

        public void DrawGizmos()
        {
            if (m_NodeLists != null && m_NodeLists.Count > 0)
                m_NodeLists[0].DrawNode(m_NodeLists);
        }
    }

}