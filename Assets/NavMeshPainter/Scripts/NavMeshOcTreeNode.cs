using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ASL.NavMeshPainter
{
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

        public void Draw(NavMeshBrush brush, List<NavMeshOcTreeNode> nodeList, bool clear)
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] > 0)
                    nodeList[m_ChildNodes[i]].Draw(brush, nodeList, clear);
            }

            if (brush.bounds.Intersects(this.bounds))
            {
                for (int i = 0; i < m_ItemList.Count; i++)
                {
                    if (brush.bounds.Intersects(m_ItemList[i].bounds))
                        m_ItemList[i].Draw(brush, clear);
                }
            }
        }

        public void DrawNode(List<NavMeshOcTreeNode> nodeList)
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] > 0)
                    nodeList[m_ChildNodes[i]].DrawNode(nodeList);
            }
            if (m_ItemList != null)
            {
                for (int i = 0; i < m_ItemList.Count; i++)
                {
                    m_ItemList[i].DrawTriangle();
                }
            }
        }

        //public void Trigger(ITrigger trigger, OctProjectorMesh mesh, List<MeshOcTreeNode> nodeList, MeshOcTreeTriggerHandle handle)
        //{
        //    if (handle == null)
        //        return;
        //    for (int i = 0; i < m_ChildNodes.Length; i++)
        //    {
        //        if (m_ChildNodes[i] > 0)
        //            nodeList[m_ChildNodes[i]].Trigger(trigger, mesh, nodeList, handle);
        //    }

        //    if (trigger.IsDetected(this.bounds))
        //    {
        //        for (int i = 0; i < m_ItemList.Count; i++)
        //        {
        //            if (trigger.IsDetected(m_ItemList[i].bounds))
        //                handle(mesh, m_ItemList[i]);
        //        }
        //    }
        //}

        public void Check(List<NavMeshOcTreeNode> nodeList)
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] > 0)
                    nodeList[m_ChildNodes[i]].Check(nodeList);
            }
            if (m_ItemList != null)
            {
                for (int i = 0; i < m_ItemList.Count; i++)
                {
                    m_ItemList[i].Check();
                }
            }
        }

        public void CheckTriangle(List<NavMeshOcTreeNode> nodeList)
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] > 0)
                    nodeList[m_ChildNodes[i]].CheckTriangle(nodeList);
            }
            if (m_ItemList != null)
            {
                for (int i = 0; i < m_ItemList.Count; i++)
                {
                    m_ItemList[i].CheckTriangle();
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