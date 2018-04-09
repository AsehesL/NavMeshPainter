using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.NavMeshPainter
{
    public class NavMeshPainterData : ScriptableObject
    {
        public Mesh renderMesh;

        public NavMeshOcTree ocTree;

        public float Area { get { return m_Area; } }

        [SerializeField]
        private float m_Area;

        public void Create(GameObject[] gameObjects, bool containChilds, float angle, float area, int maxDepth)
        {
            Vector3 max = new Vector3(-Mathf.Infinity, -Mathf.Infinity, -Mathf.Infinity);
            Vector3 min = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

            List<NavMeshTriangle> triangles = new List<NavMeshTriangle>();

            for (int i = 0; i < gameObjects.Length; i++)
            {
                if (!gameObjects[i].activeSelf)
                    continue;
                FindTriangle(gameObjects[i].transform, triangles, angle, area, maxDepth, ref max, ref min);
                if (containChilds)
                    FindTriangleInChild(gameObjects[i].transform, triangles, angle, area, maxDepth, ref max, ref min);
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

            List<Vector3> vlist = new List<Vector3>();
            List<int> ilist = new List<int>();

            for (int i = 0; i < triangles.Count; i++)
            {
                ocTree.Add(triangles[i]);
                ilist.Add(vlist.Count);
                vlist.Add(triangles[i].vertex0);
                ilist.Add(vlist.Count);
                vlist.Add(triangles[i].vertex1);
                ilist.Add(vlist.Count);
                vlist.Add(triangles[i].vertex2);
            }

            renderMesh = new Mesh();
            renderMesh.SetVertices(vlist);
            renderMesh.SetTriangles(ilist, 0);
            renderMesh.RecalculateNormals();

            m_Area = area;
        }

        public void Draw(IPaintingTool tool)
        {
            if (ocTree != null)
                ocTree.Draw(tool);
        }

        public void Erase(IPaintingTool tool)
        {
            if (ocTree != null)
                ocTree.Draw(tool, true);
        }

        public Mesh GenerateMesh()
        {
            if (ocTree != null)
                return ocTree.GenerateMesh();
            return null;
        }

        private void FindTriangle(Transform transform, List<NavMeshTriangle> triangles, float angle, float area, int maxDepth, ref Vector3 max,
            ref Vector3 min)
        {
            MeshFilter mf = transform.GetComponent<MeshFilter>();
            if (!mf || !mf.sharedMesh)
                return;
            Vector3[] vlist = mf.sharedMesh.vertices;
            Vector3[] nlist = mf.sharedMesh.normals;
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

                max = Vector3.Max(max, v0);
                max = Vector3.Max(max, v1);
                max = Vector3.Max(max, v2);

                min = Vector3.Min(min, v0);
                min = Vector3.Min(min, v1);
                min = Vector3.Min(min, v2);

                NavMeshTriangle triangle = new NavMeshTriangle(v0, v1, v2, area, maxDepth);
                triangles.Add(triangle);
            }
        }

        private void FindTriangleInChild(Transform transform, List<NavMeshTriangle> triangles, float angle, float area, int maxDepth, ref Vector3 max, ref Vector3 min)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.gameObject.activeSelf)
                    FindTriangle(child, triangles, angle, area, maxDepth, ref max, ref min);
            }
        }

        public void DrawGizmos(Color color)
        {
            Gizmos.color = color;
            if (ocTree != null)
                ocTree.DrawGizmos();
        }
    }

}