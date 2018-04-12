using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ASL.NavMesh
{
    /// <summary>
    /// 八叉树节点
    /// </summary>
    [System.Serializable]
    internal class NavMeshOcTreeNode 
    {
        public Bounds bounds { get { return m_Bounds; } }

        [SerializeField]
        private Bounds m_Bounds;

        [SerializeField]
        private List<NavMeshTriangle> m_ItemList;

        [SerializeField]
        private int[] m_ChildNodes = new int[] { -1, -1, -1, -1, -1, -1, -1, -1 };

        public NavMeshOcTreeNode(Bounds bounds)
        {
            this.m_Bounds = bounds;
            this.m_ItemList = new List<NavMeshTriangle>();
        }

        public void Init(List<NavMeshOcTreeNode> nodeList)
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] > 0)
                    nodeList[m_ChildNodes[i]].Init(nodeList);
            }
            if (m_ItemList != null)
            {
                for (int i = 0; i < m_ItemList.Count; i++)
                {
                    m_ItemList[i].Init();
                }
            }
        }

        public void Save(List<NavMeshOcTreeNode> nodeList)
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] > 0)
                    nodeList[m_ChildNodes[i]].Save(nodeList);
            }
            if (m_ItemList != null)
            {
                for (int i = 0; i < m_ItemList.Count; i++)
                {
                    m_ItemList[i].Save();
                }
            }
        }

        public NavMeshOcTreeNode Insert(NavMeshTriangle item, int depth, int maxDepth, List<NavMeshOcTreeNode> nodeList)
        {
            if (depth < maxDepth)
            {
                NavMeshOcTreeNode node = GetContainerNode(item, nodeList);
                if (node != null)
                    return node.Insert(item, depth + 1, maxDepth, nodeList);
            }
            m_ItemList.Add(item);
            return this;
        }

        public void Interesect(IPaintingTool tool, List<NavMeshOcTreeNode> nodeList, bool erase)
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] > 0)
                    nodeList[m_ChildNodes[i]].Interesect(tool, nodeList, erase);
            }

            if (tool.IntersectsBounds(this.bounds))
            {
                for (int i = 0; i < m_ItemList.Count; i++)
                {
                    if (tool.IntersectsBounds(m_ItemList[i].bounds))
                        m_ItemList[i].Interesect(tool, erase);
                }
            }
        }

        public void GenerateMesh(List<NavMeshOcTreeNode> nodeList, List<NavMeshRenderTriangle> triangles)
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] > 0)
                    nodeList[m_ChildNodes[i]].GenerateMesh(nodeList, triangles);
            }
            if (m_ItemList != null)
            {
                for (int i = 0; i < m_ItemList.Count; i++)
                {
                    m_ItemList[i].GenerateMesh(triangles);
                }
            }
        }

        public void SamplingFromTexture(List<NavMeshOcTreeNode> nodeList, Texture2D texture, TextureBlendMode blendMode)
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] > 0)
                    nodeList[m_ChildNodes[i]].SamplingFromTexture(nodeList, texture, blendMode);
            }
            if (m_ItemList != null)
            {
                for (int i = 0; i < m_ItemList.Count; i++)
                {
                    m_ItemList[i].SamplingFromTexture(texture, blendMode);
                }
            }
        }

        public void CheckMaxTriangleNodeCount(List<NavMeshOcTreeNode> nodeList, ref int maxCount)
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] > 0)
                    nodeList[m_ChildNodes[i]].CheckMaxTriangleNodeCount(nodeList, ref maxCount);
            }
            if (m_ItemList != null)
            {
                for (int i = 0; i < m_ItemList.Count; i++)
                {
                    int count = m_ItemList[i].CheckMaxTriangleNodeCount();
                    if (count > maxCount)
                        maxCount = count;
                }
            }
        }

        public void DrawNodeGizmos(List<NavMeshOcTreeNode> nodeList)
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] > 0)
                    nodeList[m_ChildNodes[i]].DrawNodeGizmos(nodeList);
            }
            if (m_ItemList != null)
            {
                for (int i = 0; i < m_ItemList.Count; i++)
                {
                    m_ItemList[i].DrawTriangleGizmos();
                }
            }
        }

        private NavMeshOcTreeNode GetContainerNode(NavMeshTriangle item, List<NavMeshOcTreeNode> nodeList)
        {
            Vector3 halfSize = bounds.size / 2;
            int result = -1;
            result = GetContainerNode(ref m_ChildNodes[0], bounds.center + new Vector3(-halfSize.x / 2, halfSize.y / 2, halfSize.z / 2),
                halfSize, item, nodeList);
            if (result > 0)
                return nodeList[result];

            result = GetContainerNode(ref m_ChildNodes[1], bounds.center + new Vector3(-halfSize.x / 2, halfSize.y / 2, -halfSize.z / 2),
                halfSize, item, nodeList);
            if (result > 0)
                return nodeList[result];

            result = GetContainerNode(ref m_ChildNodes[2], bounds.center + new Vector3(halfSize.x / 2, halfSize.y / 2, halfSize.z / 2),
                halfSize, item, nodeList);
            if (result > 0)
                return nodeList[result];

            result = GetContainerNode(ref m_ChildNodes[3], bounds.center + new Vector3(halfSize.x / 2, halfSize.y / 2, -halfSize.z / 2),
                halfSize, item, nodeList);
            if (result > 0)
                return nodeList[result];

            result = GetContainerNode(ref m_ChildNodes[4], bounds.center + new Vector3(-halfSize.x / 2, -halfSize.y / 2, halfSize.z / 2),
                halfSize, item, nodeList);
            if (result > 0)
                return nodeList[result];

            result = GetContainerNode(ref m_ChildNodes[5], bounds.center + new Vector3(-halfSize.x / 2, -halfSize.y / 2, -halfSize.z / 2),
                halfSize, item, nodeList);
            if (result > 0)
                return nodeList[result];

            result = GetContainerNode(ref m_ChildNodes[6], bounds.center + new Vector3(halfSize.x / 2, -halfSize.y / 2, halfSize.z / 2),
                halfSize, item, nodeList);
            if (result > 0)
                return nodeList[result];

            result = GetContainerNode(ref m_ChildNodes[7], bounds.center + new Vector3(halfSize.x / 2, -halfSize.y / 2, -halfSize.z / 2),
                halfSize, item, nodeList);
            if (result > 0)
                return nodeList[result];

            return null;
        }

        private int GetContainerNode(ref int node, Vector3 centerPos, Vector3 size, NavMeshTriangle item, List<NavMeshOcTreeNode> nodeList)
        {
            int result = -1;
            Bounds bd = item.bounds;
            if (node < 0)
            {
                Bounds bounds = new Bounds(centerPos, size);
                if (bounds.IsBoundsContainsAnotherBounds(bd))
                {
                    nodeList.Add(new NavMeshOcTreeNode(bounds));
                    node = nodeList.Count - 1;
                    result = node;
                }
            }
            else if (nodeList[node].bounds.IsBoundsContainsAnotherBounds(bd))
            {
                result = node;
            }
            return result;
        }
    }
  
}