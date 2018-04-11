using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ASL.NavMesh
{
    /// <summary>
    /// 八叉树
    /// </summary>
    [System.Serializable]
    public class NavMeshOcTree
    {

        public int count { get { return m_Count; } }

        /// <summary>
        /// 八叉树包围盒区域
        /// </summary>
        public Bounds bounds
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
            this.m_NodeLists.Add(new NavMeshOcTreeNode(new Bounds(center, size)));
        }

        public void Init()
        {
            if (m_NodeLists != null && m_NodeLists.Count > 0)
                m_NodeLists[0].Init(m_NodeLists);
        }

        public void Save()
        {
            if (m_NodeLists != null && m_NodeLists.Count > 0)
                m_NodeLists[0].Save(m_NodeLists);
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
                        m_Count++;
                    }
                }
            }
        }

        /// <summary>
        /// 生成mesh
        /// </summary>
        /// <param name="color">mesh顶点色</param>
        /// <returns></returns>
        public Mesh GenerateMesh(Color color)
        {
            List<Vector3> vlist = new List<Vector3>();
            List<int> ilist = new List<int>();
            if (m_NodeLists != null && m_NodeLists.Count > 0)
                m_NodeLists[0].GenerateMesh(m_NodeLists, vlist, ilist);
            Mesh mesh = new Mesh();
            List<Color> clist = new List<Color>();
            for (int i = 0; i < vlist.Count; i++)
                clist.Add(color);
            mesh.SetVertices(vlist);
            mesh.SetColors(clist);
            mesh.SetTriangles(ilist, 0);
            mesh.hideFlags = HideFlags.HideAndDontSave;
            return mesh;
        }

        /// <summary>
        /// 从贴图采样
        /// </summary>
        /// <param name="texture">目标贴图</param>
        /// <param name="blendMode">混合模式</param>
        public void SamplingFromTexture(Texture2D texture, TextureBlendMode blendMode)
        {
            if (m_NodeLists != null && m_NodeLists.Count > 0)
                m_NodeLists[0].SamplingFromTexture(m_NodeLists, texture, blendMode);
        }

        public void CheckMaxTriangleNodeCount()
        {
            int maxDepth = 0;
            if (m_NodeLists != null && m_NodeLists.Count > 0)
                m_NodeLists[0].CheckMaxTriangleNodeCount(m_NodeLists, ref maxDepth);
            Debug.Log("Max Triangle Node Count:" + maxDepth);
        }

        /// <summary>
        /// 相交测试
        /// </summary>
        /// <param name="tool">绘画工具</param>
        /// <param name="erase">是否擦除</param>
        public void Interesect(IPaintingTool tool, bool erase = false)
        {
            if (m_NodeLists != null && m_NodeLists.Count > 0)
                m_NodeLists[0].Interesect(tool, m_NodeLists, erase);
        }

        public void DrawGizmos()
        {
            if (m_NodeLists != null && m_NodeLists.Count > 0)
                m_NodeLists[0].DrawNodeGizmos(m_NodeLists);
        }
    }

}