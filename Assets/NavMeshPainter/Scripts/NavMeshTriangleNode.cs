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

        public bool isBePainted;
        public bool isMix;
        
        [SerializeField] private int m_CenterNodeIndex = -1;
        [SerializeField] private int m_TopNodeIndex = -1;
        [SerializeField] private int m_LeftNodeIndex = -1;
        [SerializeField] private int m_RightNodeIndex = -1;

        private NavMeshTriangleNode() { }

        public NavMeshTriangleNode(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
        {
            this.vertex0 = vertex0;
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
        }

        public void Subdivide(int depth, List<NavMeshTriangleNode> nodelist)
        {
            if (depth <= 0)
                return;
            NavMeshTriangleNode center = new NavMeshTriangleNode();
            NavMeshTriangleNode top = new NavMeshTriangleNode();
            NavMeshTriangleNode left = new NavMeshTriangleNode();
            NavMeshTriangleNode right = new NavMeshTriangleNode();

            center.vertex0 = vertex0 + (vertex1 - vertex0)*0.5f;
            center.vertex1 = vertex1 + (vertex2 - vertex1)*0.5f;
            center.vertex2 = vertex0 + (vertex2 - vertex0)*0.5f;

            top.vertex0 = vertex0 + (vertex1 - vertex0) * 0.5f;
            top.vertex1 = vertex1;
            top.vertex2 = vertex1 + (vertex2 - vertex1) * 0.5f;

            left.vertex0 = vertex0;
            left.vertex1 = vertex0 + (vertex1 - vertex0) * 0.5f;
            left.vertex2 = vertex0 + (vertex2 - vertex0) * 0.5f;

            right.vertex0 = vertex0 + (vertex2 - vertex0) * 0.5f;
            right.vertex1 = vertex1 + (vertex2 - vertex1) * 0.5f;
            right.vertex2 = vertex2;

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

        public void Paint(bool paint, Vector3 brushPos, float radius, float height, List<NavMeshTriangleNode> nodelist)
        {
            bool isHit = IsHitBrush(vertex0, brushPos, radius, height);
            if (isHit)
            {
                SetPaintMark(paint, brushPos, radius, height, nodelist);
                return;
            }
            isHit = IsHitBrush(vertex1, brushPos, radius, height);
            if (isHit)
            {
                SetPaintMark(paint, brushPos, radius, height, nodelist);
                return;
            }
            isHit = IsHitBrush(vertex2, brushPos, radius, height);
            if (isHit)
            {
                SetPaintMark(paint, brushPos, radius, height, nodelist);
                return;
            }

        }

        public void SetPaintMark(bool paint, Vector3 brushPos, float radius, float height, List<NavMeshTriangleNode> nodelist)
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
                center.Paint(paint, brushPos, radius, height, nodelist);

            if (top != null)
                top.Paint(paint, brushPos, radius, height, nodelist);

            if (left != null)
                left.Paint(paint, brushPos, radius, height, nodelist);

            if (right != null)
                right.Paint(paint, brushPos, radius, height, nodelist);

            ResetPaintMark(paint, center, top, left, right);
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

        public void Check(List<NavMeshTriangleNode> nodeList)
        {
            if (!isMix)
            {
                if (isBePainted)
                {
                    Debug.Log("Paint");
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
                    center.Check(nodeList);
                if (top != null)
                    top.Check(nodeList);
                if (left != null)
                    left.Check(nodeList);
                if (right != null)
                    right.Check(nodeList);
            }
        }

        public void CheckTriangle(List<NavMeshTriangleNode> nodeList)
        {
            Debug.Log("Triangle:" + vertex0 + "," + vertex1 + "," + vertex2);

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
                center.Check(nodeList);
            if (top != null)
                top.Check(nodeList);
            if (left != null)
                left.Check(nodeList);
            if (right != null)
                right.Check(nodeList);
        }

        private bool IsHitBrush(Vector3 pos, Vector3 brushPos, float radius, float height)
        {
            float deltaH = Mathf.Abs(pos.y - brushPos.y);
            if (deltaH > height)
                return false;
            float dis = Vector2.Distance(new Vector2(pos.x, pos.z), new Vector2(brushPos.x, brushPos.z));
            //Debug.Log("Dis:" + dis + "," + radius);
            return dis < radius;
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