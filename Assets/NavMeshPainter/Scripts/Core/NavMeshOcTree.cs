using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ASL.NavMesh
{
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
                if (m_Root != null )
                    return m_Root.bounds;
                return default(Bounds);
            }
        }
        
        private int m_Count;
        
        private int m_MaxDepth;

        private NavMeshOcTreeNode m_Root;

        private float m_MaxTriangleArea;
        private int m_MaxTriangleDepth;
        private bool m_FOrceSetTriangleDepth;
        

        /// <summary>
        /// 构造八叉树
        /// </summary>
        /// <param name="center">八叉树中心坐标</param>
        /// <param name="size">八叉树区域大小</param>
        /// <param name="maxDepth">最大深度</param>
        public NavMeshOcTree(Vector3 center, Vector3 size, int maxDepth)
        {
            this.m_MaxDepth = maxDepth;
            this.m_Root = new NavMeshOcTreeNode(new Bounds(center, size));
        }

        public static NavMeshOcTree Create(MeshFilter[] meshFilter, int maxTriangleDepth, bool forceSetTriangleDepth)
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
            NavMeshOcTree ocTree = new NavMeshOcTree(center, size*1.1f, 7);

            for (int i = 0; i < triangles.Count; i++)
            {
                triangles[i].SetMaxDepth(maxDepth, maxArea, forceSetDepth);

                ocTree.Add(triangles[i]);
            }

            return ocTree;
        }

        private static void FindTriangle(Transform transform, List<NavMeshTriangle> triangles, float angle, ref Vector3 max,
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

        private static void FindTriangleInChild(Transform transform, List<NavMeshTriangle> triangles, float angle,
            ref Vector3 max,
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

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="item"></param>
        public void Add(NavMeshTriangle item)
        {
            if (m_Root != null)
            {
                NavMeshOcTreeNode node = m_Root.Insert(item, 0, m_MaxDepth);
                if (node != null)
                {
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
        public Mesh[] GenerateMeshes(Color color)
        {
            List<NavMeshRenderTriangle> triangles = new List<NavMeshRenderTriangle>();
            
            if (m_Root != null)
                m_Root.GenerateMesh(triangles);

            Mesh[] meshes = new Mesh[triangles.Count];

            for (int i = 0; i < triangles.Count; i++)
            {
                meshes[i] = new Mesh();
                List<Color> clist = new List<Color>();
                for (int j = 0; j < triangles[i].vertexList.Count; j++)
                    clist.Add(color);
                meshes[i].SetVertices(triangles[i].vertexList);
                meshes[i].SetColors(clist);
                meshes[i].SetUVs(0, triangles[i].uvList);
                meshes[i].SetTriangles(triangles[i].indexList, 0);
                meshes[i].RecalculateNormals();
                meshes[i].hideFlags = HideFlags.HideAndDontSave;
            }
            
            return meshes;
        }

        public Mesh[] GenerateRenderMesh()
        {
            List<NavMeshRenderTriangle> triangles = new List<NavMeshRenderTriangle>();
            if (m_Root != null)
                m_Root.GenerateRenderMesh(triangles);
            Mesh[] meshes = new Mesh[triangles.Count];

            for (int i = 0; i < triangles.Count; i++)
            {
                meshes[i] = new Mesh();
                meshes[i].SetVertices(triangles[i].vertexList);
                meshes[i].SetUVs(0, triangles[i].uvList);
                meshes[i].SetTriangles(triangles[i].indexList, 0);
                meshes[i].RecalculateNormals();
                meshes[i].hideFlags = HideFlags.HideAndDontSave;
            }
            return meshes;
        }

        public float GetMinSize()
        {
            float x = bounds.size.x;
            float z = bounds.size.z;
            return Mathf.Min(x, z);
        }

        public void Clear()
        {
            if (m_Root != null)
                m_Root.Clear();
        }

        /// <summary>
        /// 从贴图采样
        /// </summary>
        /// <param name="texture">目标贴图</param>
        /// <param name="blendMode">混合模式</param>
        public void SamplingFromTexture(Texture2D texture)
        {
            if (m_Root != null)
                m_Root.SamplingFromTexture(texture);
        }

        /// <summary>
        /// 相交测试
        /// </summary>
        /// <param name="tool">绘画工具</param>
        /// <param name="erase">是否擦除</param>
        public void Interesect(IPaintingTool tool, bool erase = false)
        {
            if (m_Root != null)
                m_Root.Interesect(tool, erase);
        }

        public void DrawGizmos(Color color, Camera sceneViewCamera, float lodDeltaDis)
        {
            Gizmos.color = color;
            if (m_Root != null)
                m_Root.DrawNodeGizmos(sceneViewCamera, lodDeltaDis);
        }

        public static bool Save(NavMeshOcTree tree, string path)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                bf.Serialize(fs, tree);

                fs.Close();
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public static NavMeshOcTree Load(string path)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                NavMeshOcTree tree = bf.Deserialize(fs) as NavMeshOcTree;
                
                fs.Close();

                return tree;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null; 
            }
        }
    }

}