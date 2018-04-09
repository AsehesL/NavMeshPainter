using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ASL.NavMesh.Editor
{
    internal class NavMeshEditorUtils
    {
        private static System.Reflection.MethodInfo intersectRayMesh
        {
            get
            {
                if (m_IntersectRayMesh == null)
                {
                    var tp = typeof(HandleUtility);
                    m_IntersectRayMesh = tp.GetMethod("IntersectRayMesh",
                        System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                }
                return m_IntersectRayMesh;
            }
        }

        public static Material GLMaterial
        {
            get
            {
                if (m_GLMaterial == null)
                {
                    m_GLMaterial = new Material((Shader)EditorGUIUtility.Load("NavMeshPainter/Shader/NavMeshDebug.shader"));

                    m_GLMaterial.hideFlags = HideFlags.HideAndDontSave;
                    m_GLMaterial.SetFloat("_CellSize", 2.5f);
                }
                return m_GLMaterial;
            }
        }

        private static System.Reflection.MethodInfo m_IntersectRayMesh;

        private static Material m_GLMaterial;

        public static bool RayCastInSceneView(Mesh mesh, out RaycastHit hit)
        {
            hit = default(RaycastHit);
            if (!mesh)
                return false;
            if (UnityEditor.Tools.viewTool != ViewTool.Pan)
                return false;
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            SceneView.RepaintAll();
            if (RaycastMesh(ray, mesh, Matrix4x4.identity, out hit))
                return true;
            return false;
        }
        public static bool RaycastMesh(Ray ray, Mesh mesh, Matrix4x4 matrix, out RaycastHit hit)
        {
            var parameters = new object[] { ray, mesh, matrix, null };
            bool result = (bool)intersectRayMesh.Invoke(null, parameters);
            hit = (RaycastHit)parameters[3];
            return result;
        }

        public static void SetCheckerBoardCellSize(float size)
        {
            GLMaterial.SetFloat("_CellSize", size*0.25f);
        }

        public static void SetMaskTexture(Texture2D texture)
        {
            GLMaterial.SetTexture("_Mask", texture);
        }

        public static void DrawCheckerBoard(Mesh mesh, Matrix4x4 matrix)
        {
            if (mesh && GLMaterial)
            {
                GLMaterial.SetPass(0);
                Graphics.DrawMeshNow(mesh, matrix);
            }
        }

        public static void DrawMask(Mesh mesh, Matrix4x4 matrix)
        {
            if (mesh && GLMaterial)
            {
                GLMaterial.SetPass(4);
                Graphics.DrawMeshNow(mesh, matrix);
            }
        }

        public static void DrawLine(Vector3 from, Vector3 to, Color color)
        {
            DrawLineInternal(from, to, color, false);
            DrawLineInternal(from, to, color, true);
        }

        public static void DrawWireCube(Vector3 center, Vector3 size, Color color)
        {
            DrawWireCubeInternal(center, size, color, false);
            DrawWireCubeInternal(center, size, color, true);
        }

        public static void DrawBounds(Bounds bounds, Color color)
        {
            DrawWireCube(bounds.center, bounds.size, color);
        }

        public static void DrawBrush(Mesh mesh, Matrix4x4 matrix, Vector3 position, float xsize, float zsize, float height, NavMeshBrushType brushType)
        {
            if (mesh && GLMaterial)
            {
                float type = brushType == NavMeshBrushType.Box ? 1 : 0;
                GLMaterial.SetPass(3);
                GLMaterial.SetVector("_BrushPos", position);
                GLMaterial.SetVector("_BrushSize", new Vector4(xsize, zsize, height, type));
                GLMaterial.SetColor("_BrushColor", new Color(0, 0.5f, 1, 0.5f));
                Graphics.DrawMeshNow(mesh, matrix);
            }
            
        }

        public static void ClearBrush()
        {
            GLMaterial.SetColor("_BrushColor", Color.clear);
        }

        private static void DrawLineInternal(Vector3 from, Vector3 to, Color color, bool seeThrough)
        {
            GL.PushMatrix();
            GLMaterial.SetPass(!seeThrough ? 1 : 2);
            GL.MultMatrix(Handles.matrix);
            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex(from);
            GL.Vertex(to);
            GL.End();
            GL.PopMatrix();
        }

        private static void DrawWireCubeInternal(Vector3 center, Vector3 size, Color color, bool seeThrough)
        {
            Vector3 p0 = center + new Vector3(-size.x * 0.5f, -size.y * 0.5f, -size.z * 0.5f);
            Vector3 p1 = center + new Vector3(-size.x * 0.5f, -size.y * 0.5f, size.z * 0.5f);
            Vector3 p2 = center + new Vector3(size.x * 0.5f, -size.y * 0.5f, size.z * 0.5f);
            Vector3 p3 = center + new Vector3(size.x * 0.5f, -size.y * 0.5f, -size.z * 0.5f);
            Vector3 p4 = center + new Vector3(-size.x * 0.5f, size.y * 0.5f, -size.z * 0.5f);
            Vector3 p5 = center + new Vector3(-size.x * 0.5f, size.y * 0.5f, size.z * 0.5f);
            Vector3 p6 = center + new Vector3(size.x * 0.5f, size.y * 0.5f, size.z * 0.5f);
            Vector3 p7 = center + new Vector3(size.x * 0.5f, size.y * 0.5f, -size.z * 0.5f);

            GL.PushMatrix();
            GLMaterial.SetPass(!seeThrough ? 1 : 2);
            GL.MultMatrix(Handles.matrix);
            GL.Begin(GL.LINES);
            GL.Color(color);

            GL.Vertex(p0);
            GL.Vertex(p1);

            GL.Vertex(p1);
            GL.Vertex(p2);

            GL.Vertex(p2);
            GL.Vertex(p3);

            GL.Vertex(p3);
            GL.Vertex(p0);

            GL.Vertex(p4);
            GL.Vertex(p5);

            GL.Vertex(p5);
            GL.Vertex(p6);

            GL.Vertex(p6);
            GL.Vertex(p7);

            GL.Vertex(p7);
            GL.Vertex(p4);

            GL.Vertex(p0);
            GL.Vertex(p4);

            GL.Vertex(p1);
            GL.Vertex(p5);

            GL.Vertex(p2);
            GL.Vertex(p6);

            GL.Vertex(p3);
            GL.Vertex(p7);
            GL.End();
            GL.PopMatrix();
        }
    }
}