using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.NavMesh;

namespace ASL.NavMesh
{
    

//    /// <summary>
//    /// 蒙版贴图混合方式
//    /// </summary>
//    public enum TextureBlendMode
//    {
//        /// <summary>
//        /// 添加
//        /// </summary>
//        Add,
//        /// <summary>
//        /// 替换
//        /// </summary>
//        Replace,
//    }

    internal class NavMeshRenderTriangle
    {
        public int vertexCount { get { return m_VertexCount; } }

        public List<Vector3> vertexList { get { return m_Vertexlist; } }
        public List<Vector2> uvList { get { return m_UVlist; } }
        public List<int> indexList { get { return m_Indexlist; } }

        private int m_VertexCount = 0;
        private List<Vector3> m_Vertexlist;
        private List<Vector2> m_UVlist;
        private List<int> m_Indexlist;

        public NavMeshRenderTriangle(bool noUv = false)
        {
            m_Vertexlist = new List<Vector3>();
            m_Indexlist = new List<int>();
            if (!noUv)
                m_UVlist = new List<Vector2>();
        }

        public void AddVertex(Vector3 vertex, Vector2 uv)
        {
            m_Indexlist.Add(m_Vertexlist.Count);
            m_Vertexlist.Add(vertex);
            if (m_UVlist != null)
                m_UVlist.Add(uv);
            m_VertexCount += 1;
        }

        public void AddVertex(Vector3 vertex)
        {
            m_Indexlist.Add(m_Vertexlist.Count);
            m_Vertexlist.Add(vertex);
            m_VertexCount += 1;
        }
    }
}

/// <summary>
/// 导航网格绘制数据
/// </summary>
public class NavMeshPainterData : ScriptableObject
{
    /// <summary>
    /// 用于渲染的mesh
    /// </summary>
    public Mesh[] renderMeshs;

    /// <summary>
    /// 八叉树
    /// </summary>
    public NavMeshOcTree ocTree;

    /// <summary>
    /// 创建Data
    /// </summary>
    /// <param name="gameObjects">物体列表</param>
    /// <param name="containChilds">是否包含子物体</param>
    /// <param name="angle">与法线夹角</param>
    /// <param name="maxDepth"></param>
    /// <param name="forceSetDepth"></param>
    public void Create(GameObject[] gameObjects, bool containChilds, float angle, int maxDepth, bool forceSetDepth)
    {
        Vector3 max = new Vector3(-Mathf.Infinity, -Mathf.Infinity, -Mathf.Infinity);
        Vector3 min = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        float maxArea = 0;

        List<NavMeshTriangle> triangles = new List<NavMeshTriangle>();

        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (!gameObjects[i].activeSelf)
                continue;
            FindTriangle(gameObjects[i].transform, triangles, angle, ref max, ref min, ref maxArea);
            if (containChilds)
                FindTriangleInChild(gameObjects[i].transform, triangles, angle, ref max, ref min, ref maxArea);
        }

        Vector3 size = max - min;

        if (size.x <= 0)
            size.x = 0.1f;
        if (size.y <= 0)
            size.y = 0.1f;
        if (size.z <= 0)
            size.z = 0.1f;

        Vector3 center = min + size*0.5f;
        ocTree = new NavMeshOcTree(center, size*1.1f, 7);

        //List<Vector3> vlist = new List<Vector3>();
        //List<Vector2> ulist = new List<Vector2>();
        //List<int> ilist = new List<int>();
        List<NavMeshRenderTriangle> triangleList = new List<NavMeshRenderTriangle>();
        NavMeshRenderTriangle triangle = new NavMeshRenderTriangle();
        triangleList.Add(triangle);

        for (int i = 0; i < triangles.Count; i++)
        {
            triangles[i].SetMaxDepth(maxDepth, maxArea, forceSetDepth);

            ocTree.Add(triangles[i]);

            if (triangle.vertexCount > 64990)
            {
                triangle = new NavMeshRenderTriangle();
                triangleList.Add(triangle);
            }

            triangle.AddVertex(triangles[i].vertex0, triangles[i].uv0);
            triangle.AddVertex(triangles[i].vertex1, triangles[i].uv1);
            triangle.AddVertex(triangles[i].vertex2, triangles[i].uv2);

            //ilist.Add(vlist.Count);
            //vlist.Add(triangles[i].vertex0);
            //ulist.Add(triangles[i].uv0);
            //ilist.Add(vlist.Count);
            //vlist.Add(triangles[i].vertex1);
            //ulist.Add(triangles[i].uv1);
            //ilist.Add(vlist.Count);
            //vlist.Add(triangles[i].vertex2);
            //ulist.Add(triangles[i].uv2);
        }

        //renderMesh = new Mesh();
        //renderMesh.SetVertices(vlist);
        //renderMesh.SetUVs(0, ulist);
        //renderMesh.SetTriangles(ilist, 0);
        //renderMesh.RecalculateNormals();

        renderMeshs = new Mesh[triangleList.Count];
        for (int i = 0; i < triangleList.Count; i++)
        {
            renderMeshs[i] = new Mesh();
            renderMeshs[i].SetVertices(triangleList[i].vertexList);
            renderMeshs[i].SetUVs(0, triangleList[i].uvList);
            renderMeshs[i].SetTriangles(triangleList[i].indexList, 0);
            renderMeshs[i].RecalculateNormals();
        }
    }

    public void Init()
    {
        if (ocTree != null)
            ocTree.Init();
    }

    public void Save()
    {
        if (ocTree != null)
            ocTree.Save();
    }

    public void Paint(IPaintingTool tool)
    {
        if (ocTree != null)
            ocTree.Interesect(tool);
    }

    public void Erase(IPaintingTool tool)
    {
        if (ocTree != null)
            ocTree.Interesect(tool, true);
    }

    public float GetMinSize()
    {
        if (ocTree == null)
            return 0;
        Bounds bd = ocTree.bounds;
        float x = bd.size.x;
        float z = bd.size.z;
        return Mathf.Min(x, z);
    }

    public Mesh[] GenerateMeshes(Color color)
    {
        if (ocTree != null)
            return ocTree.GenerateMeshes(color);
        return null;
    }

    public void Clear()
    {
        if (ocTree != null)
            ocTree.Clear();
    }

    public void SamplingFromTexture(Texture2D texture)
    {
        if (ocTree != null)
            ocTree.SamplingFromTexture(texture);
    }

    public void CheckMaxTriangleNodeCount()
    {
        if (ocTree != null)
            ocTree.CheckMaxTriangleNodeCount();
    }

    private void FindTriangle(Transform transform, List<NavMeshTriangle> triangles, float angle, ref Vector3 max,
        ref Vector3 min, ref float maxArea)
    {
        MeshFilter mf = transform.GetComponent<MeshFilter>();
        if (!mf || !mf.sharedMesh)
            return;
        Vector3[] vlist = mf.sharedMesh.vertices;
        Vector3[] nlist = mf.sharedMesh.normals;
        Vector2[] ulist = mf.sharedMesh.uv;
        int[] ilist = mf.sharedMesh.triangles;

        for (int i = 0; i < ilist.Length; i += 3)
        {
            Vector3 n0 = transform.localToWorldMatrix.MultiplyVector(nlist[ilist[i]]);
            Vector3 n1 = transform.localToWorldMatrix.MultiplyVector(nlist[ilist[i + 1]]);
            Vector3 n2 = transform.localToWorldMatrix.MultiplyVector(nlist[ilist[i + 2]]);

            float ag0 = Vector3.Angle(Vector3.up, n0);
            float ag1 = Vector3.Angle(Vector3.up, n1);
            float ag2 = Vector3.Angle(Vector3.up, n2);

            if (ag0 > angle || ag1 > angle || ag2 > angle)
                continue;

            Vector3 v0 = transform.localToWorldMatrix.MultiplyPoint(vlist[ilist[i]]);
            Vector3 v1 = transform.localToWorldMatrix.MultiplyPoint(vlist[ilist[i + 1]]);
            Vector3 v2 = transform.localToWorldMatrix.MultiplyPoint(vlist[ilist[i + 2]]);

            Vector2 u0 = ilist[i] < ulist.Length ? ulist[ilist[i]] : Vector2.zero;
            Vector2 u1 = ilist[i + 1] < ulist.Length ? ulist[ilist[i + 1]] : Vector2.zero;
            Vector2 u2 = ilist[i + 2] < ulist.Length ? ulist[ilist[i + 2]] : Vector2.zero;

            max = Vector3.Max(max, v0);
            max = Vector3.Max(max, v1);
            max = Vector3.Max(max, v2);

            min = Vector3.Min(min, v0);
            min = Vector3.Min(min, v1);
            min = Vector3.Min(min, v2);

            NavMeshTriangle triangle = new NavMeshTriangle(v0, v1, v2, u0, u1, u2);
            float area = triangle.GetArea();
            if (area > maxArea)
                maxArea = area;
            triangles.Add(triangle);
        }
    }

    private void FindTriangleInChild(Transform transform, List<NavMeshTriangle> triangles, float angle, ref Vector3 max,
        ref Vector3 min, ref float maxArea)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                FindTriangle(child, triangles, angle, ref max, ref min, ref maxArea);
                FindTriangleInChild(child, triangles, angle, ref max, ref min, ref maxArea);
            }
        }
    }

    public void DrawGizmos(Color color, Camera sceneViewCamera, float lodDeltaDis)
    {
        Gizmos.color = color;
        if (ocTree != null)
            ocTree.DrawGizmos(sceneViewCamera, lodDeltaDis);
    }

}