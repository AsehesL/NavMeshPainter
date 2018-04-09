using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ASL.NavMeshPainter
{
    [System.Serializable]
    public class NavMeshTriangleNode 
    {

        public Vector3 vertex0;
        public Vector3 vertex1;
        public Vector3 vertex2;

        public Vector3 max;
        public Vector3 min;

        public bool isBePainted;
        public bool isMix;
        
        [SerializeField] private int m_CenterNodeIndex = -1;
        [SerializeField] private int m_TopNodeIndex = -1;
        [SerializeField] private int m_LeftNodeIndex = -1;
        [SerializeField] private int m_RightNodeIndex = -1;

        public NavMeshTriangleNode(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
        {
            this.vertex0 = vertex0;
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
            max = Vector3.Max(vertex0, vertex1);
            max = Vector3.Max(max, vertex2);
            min = Vector3.Min(vertex0, vertex1);
            min = Vector3.Min(min, vertex2);
        }

        public void Subdivide(int depth, List<NavMeshTriangleNode> nodelist)
        {
            if (depth <= 0)
                return;
            Vector3 half01 = vertex0 + (vertex1 - vertex0) * 0.5f;
            Vector3 half02 = vertex0 + (vertex2 - vertex0) * 0.5f;
            Vector3 half12 = vertex1 + (vertex2 - vertex1) * 0.5f;

            NavMeshTriangleNode center = new NavMeshTriangleNode(half01, half12, half02);
            NavMeshTriangleNode top = new NavMeshTriangleNode(half01, vertex1, half12);
            NavMeshTriangleNode left = new NavMeshTriangleNode(vertex0, half01, half02);
            NavMeshTriangleNode right = new NavMeshTriangleNode(half02, half12, vertex2);

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

        public void Draw(bool paint, IPaintingTool tool, List<NavMeshTriangleNode> nodelist)
        {
            if (tool.IntersectsTriangle(this))
            {
                bool cotinuePaint = isBePainted != paint || isMix;
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
                    center.Draw(paint, tool, nodelist);

                if (top != null)
                    top.Draw(paint, tool, nodelist);

                if (left != null)
                    left.Draw(paint, tool, nodelist);

                if (right != null)
                    right.Draw(paint, tool, nodelist);

                ResetPaintMark(paint, center, top, left, right);
            }

        }

        public void DrawTriangle(List<NavMeshTriangleNode> nodeList)
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
                    center.DrawTriangle(nodeList);
                if (top != null)
                    top.DrawTriangle(nodeList);
                if (left != null)
                    left.DrawTriangle(nodeList);
                if (right != null)
                    right.DrawTriangle(nodeList);
            }
            else
            {
                if (isBePainted)
                {
                    Gizmos.DrawLine(vertex0, vertex1);
                    Gizmos.DrawLine(vertex0, vertex2);
                    Gizmos.DrawLine(vertex1, vertex2);
                }
            }
        }

        public void GenerateMesh(List<NavMeshTriangleNode> nodeList, List<Vector3> vlist, List<int> ilist)
        {
            if (!isMix)
            {
                if (isBePainted)
                {
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
                    center.GenerateMesh(nodeList, vlist, ilist);
                if (top != null)
                    top.GenerateMesh(nodeList, vlist, ilist);
                if (left != null)
                    left.GenerateMesh(nodeList, vlist, ilist);
                if (right != null)
                    right.GenerateMesh(nodeList, vlist, ilist);
            }
        }

        private bool IsHitTool(Vector3 brushPos, float radius, float height)
        {
            if (max.y < brushPos.y - height || min.y > brushPos.y + height)
                return false;
            Vector2 nearestpos = default(Vector2);
            nearestpos.x = Mathf.Clamp(brushPos.x, min.x, max.x);
            nearestpos.y = Mathf.Clamp(brushPos.z, min.z, max.z);
            float dis = Vector2.Distance(nearestpos, new Vector2(brushPos.x, brushPos.z));
            
            if (dis > radius)
                return false;
            return true;
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
    }
}