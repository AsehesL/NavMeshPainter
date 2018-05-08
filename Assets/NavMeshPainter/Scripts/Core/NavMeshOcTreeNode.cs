using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ASL.NavMesh
{
    /// <summary>
    /// Unity Bounds不支持序列化
    /// </summary>
    [System.Serializable]
    public struct NMBounds
    {
        public NMVector3 center;
        public NMVector3 size;

        public NMBounds(NMVector3 center, NMVector3 size)
        {
            this.center = center;
            this.size = size;
        }

        public static implicit operator Bounds(NMBounds bounds)
        {
            return new Bounds(bounds.center, bounds.size);
        }

        public static implicit operator NMBounds(Bounds bounds)
        {
            return new NMBounds(bounds.center, bounds.size);
        }
    }

    /// <summary>
    /// Unity Vector3不支持序列化
    /// </summary>
    [System.Serializable]
    public struct NMVector3
    {
        public float x;
        public float y;
        public float z;

        public NMVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vector3(NMVector3 vector)
        {
            return new Vector3(vector.x, vector.y, vector.z);
        }

        public static implicit operator NMVector3(Vector3 vector)
        {
            return new NMVector3(vector.x, vector.y, vector.z);
        }

        public static NMVector3 operator +(NMVector3 l, NMVector3 r)
        {
            return new NMVector3(l.x + r.x, l.y + r.y, l.z + r.z);
        }

        public static NMVector3 operator -(NMVector3 l, NMVector3 r)
        {
            return new NMVector3(l.x - r.x, l.y - r.y, l.z - r.z);
        }
    }

    [System.Serializable]
    public struct NMVector2
    {
        public float x;
        public float y;

        public NMVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator Vector2(NMVector2 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static implicit operator NMVector2(Vector2 vector)
        {
            return new NMVector2(vector.x, vector.y);
        }

        public static NMVector2 operator +(NMVector2 l, NMVector2 r)
        {
            return new NMVector2(l.x + r.x, l.y + r.y);
        }

        public static NMVector2 operator -(NMVector2 l, NMVector2 r)
        {
            return new NMVector2(l.x - r.x, l.y - r.y);
        }

        public static NMVector2 operator *(NMVector2 l, float r)
        {
            return new NMVector2(l.x * r, l.y * r);
        }
    }
    /// <summary>
    /// 八叉树节点
    /// </summary>
    [System.Serializable]
    internal class NavMeshOcTreeNode 
    {
        public Bounds bounds
        {
            get
            {
                return m_Bounds;
            }
        }

        private NMBounds m_Bounds;
        
        private List<NavMeshTriangle> m_ItemList;
        
        private NavMeshOcTreeNode[] m_ChildNodes = new NavMeshOcTreeNode[] { null, null, null, null, null, null, null, null };

        public NavMeshOcTreeNode(Bounds bounds)
        {
            this.m_Bounds = bounds;
            this.m_ItemList = new List<NavMeshTriangle>();
        }

        public NavMeshOcTreeNode Insert(NavMeshTriangle item, int depth, int maxDepth)
        {
            if (depth < maxDepth)
            {
                NavMeshOcTreeNode node = GetContainerNode(item);
                if (node != null)
                    return node.Insert(item, depth + 1, maxDepth);
            }
            m_ItemList.Add(item);
            return this;
        }

        public void Interesect(IPaintingTool tool, bool erase)
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] != null)
                    m_ChildNodes[i].Interesect(tool, erase);
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

        public void GenerateMesh(List<NavMeshRenderTriangle> triangles)
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] != null)
                    m_ChildNodes[i].GenerateMesh(triangles);
            }
            if (m_ItemList != null)
            {
                for (int i = 0; i < m_ItemList.Count; i++)
                {
                    m_ItemList[i].GenerateMesh(triangles);
                }
            }
        }

        public void GenerateRenderMesh(List<NavMeshRenderTriangle> triangles)
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] != null)
                    m_ChildNodes[i].GenerateRenderMesh(triangles);
            }
            if (m_ItemList != null)
            {
                for (int i = 0; i < m_ItemList.Count; i++)
                {
                    m_ItemList[i].GenerateRenderMesh(triangles);
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] != null)
                    m_ChildNodes[i].Clear();
            }
            if (m_ItemList != null)
            {
                for (int i = 0; i < m_ItemList.Count; i++)
                {
                    m_ItemList[i].Clear();
                }
            }
        }

        public void SamplingFromTexture(Texture2D texture)
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] != null)
                    m_ChildNodes[i].SamplingFromTexture(texture);
            }
            if (m_ItemList != null)
            {
                for (int i = 0; i < m_ItemList.Count; i++)
                {
                    m_ItemList[i].SamplingFromTexture(texture);
                }
            }
        }

        public void DrawNodeGizmos(Camera sceneViewCamera, float lodDeltaDis)
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                if (m_ChildNodes[i] != null)
                    m_ChildNodes[i].DrawNodeGizmos(sceneViewCamera, lodDeltaDis);
            }
            if (bounds.IsBoundsInCamera(sceneViewCamera))
            {
                if (m_ItemList != null)
                {
                    for (int i = 0; i < m_ItemList.Count; i++)
                    {
                        m_ItemList[i].DrawTriangleGizmos(sceneViewCamera, lodDeltaDis);
                    }
                }
            }
        }

        private NavMeshOcTreeNode GetContainerNode(NavMeshTriangle item)
        {
            Vector3 halfSize = bounds.size / 2;
            NavMeshOcTreeNode result = null;
            result = GetContainerNode(ref m_ChildNodes[0], bounds.center + new Vector3(-halfSize.x / 2, halfSize.y / 2, halfSize.z / 2),
                halfSize, item);
            if (result != null)
                return result;

            result = GetContainerNode(ref m_ChildNodes[1], bounds.center + new Vector3(-halfSize.x / 2, halfSize.y / 2, -halfSize.z / 2),
                halfSize, item);
            if (result != null)
                return result;

            result = GetContainerNode(ref m_ChildNodes[2], bounds.center + new Vector3(halfSize.x / 2, halfSize.y / 2, halfSize.z / 2),
                halfSize, item);
            if (result != null)
                return result;

            result = GetContainerNode(ref m_ChildNodes[3], bounds.center + new Vector3(halfSize.x / 2, halfSize.y / 2, -halfSize.z / 2),
                halfSize, item);
            if (result != null)
                return result;

            result = GetContainerNode(ref m_ChildNodes[4], bounds.center + new Vector3(-halfSize.x / 2, -halfSize.y / 2, halfSize.z / 2),
                halfSize, item);
            if (result != null)
                return result;

            result = GetContainerNode(ref m_ChildNodes[5], bounds.center + new Vector3(-halfSize.x / 2, -halfSize.y / 2, -halfSize.z / 2),
                halfSize, item);
            if (result != null)
                return result;

            result = GetContainerNode(ref m_ChildNodes[6], bounds.center + new Vector3(halfSize.x / 2, -halfSize.y / 2, halfSize.z / 2),
                halfSize, item);
            if (result != null)
                return result;

            result = GetContainerNode(ref m_ChildNodes[7], bounds.center + new Vector3(halfSize.x / 2, -halfSize.y / 2, -halfSize.z / 2),
                halfSize, item);
            if (result != null)
                return result;

            return null;
        }

        private NavMeshOcTreeNode GetContainerNode(ref NavMeshOcTreeNode node, Vector3 centerPos, Vector3 size, NavMeshTriangle item)
        {
            NavMeshOcTreeNode result = null;
            Bounds bd = item.bounds;
            if (node == null)
            {
                Bounds bounds = new Bounds(centerPos, size);
                if (bounds.IsBoundsContainsAnotherBounds(bd))
                {
                    node = new NavMeshOcTreeNode(bounds);
                    result = node;
                }
            }
            else if (node.bounds.IsBoundsContainsAnotherBounds(bd))
            {
                result = node;
            }
            return result;
        }
    }
  
}