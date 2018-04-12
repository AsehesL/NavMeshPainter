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


        public static bool RayCastInSceneView(Mesh[] meshes, out RaycastHit hit)
        {
            if (meshes != null && meshes.Length > 0)
            {
                for (int i = 0; i < meshes.Length; i++)
                {
                    if (RayCastInSceneView(meshes[i], out hit))
                    {
                        return true;
                    }
                }
            }
            hit = default(RaycastHit);
            return false;
        }
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
            //GLMaterial.SetFloat("_CellSize", size*0.25f);
            GLMaterial.SetFloat("_CellSize", 30f/size);
        }

        public static void SetMaskTexture(Texture2D texture)
        {
            GLMaterial.SetTexture("_Mask", texture);
        }

        public static void DrawCheckerBoard(Mesh[] meshes, Matrix4x4 matrix)
        {
            if (meshes != null && meshes.Length > 0)
            {
                for (int i = 0; i < meshes.Length; i++)
                    DrawCheckerBoard(meshes[i], matrix);
            }
        }

        public static void DrawCheckerBoard(Mesh mesh, Matrix4x4 matrix)
        {
            if (mesh && GLMaterial)
            {
                GLMaterial.SetPass(0);
                Graphics.DrawMeshNow(mesh, matrix);
            }
        }

        public static void DrawMask(Mesh[] meshes, Matrix4x4 matrix)
        {
            if (meshes != null && meshes.Length > 0)
            {
                for (int i = 0; i < meshes.Length; i++)
                    DrawMask(meshes[i], matrix);
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

        public static void DrawWireCylinder(Bounds bounds, Color color)
        {
            DrawWireCylinderInternal(bounds.center, bounds.size, color, false);
            DrawWireCylinderInternal(bounds.center, bounds.size, color, true);
        }

        public static void DrawWireCylinder(Vector3 center, Vector3 size, Color color)
        {
            DrawWireCylinderInternal(center, size, color, false);
            DrawWireCylinderInternal(center, size, color, true);
        }

        public static void DrawBounds(Bounds bounds, Color color)
        {
            DrawWireCube(bounds.center, bounds.size, color);
        }

        

        public static void DrawBrush(Mesh[] meshes, Matrix4x4 matrix, Vector3 position, float xsize, float zsize, float height, NavMeshBrushTool.NavMeshBrushType brushType)
        {
            if (meshes != null && meshes.Length > 0)
            {
                for (int i = 0; i < meshes.Length; i++)
                    DrawBrush(meshes[i], matrix, position, xsize, zsize, height, brushType);
            }

        }

        public static void DrawBrush(Mesh mesh, Matrix4x4 matrix, Vector3 position, float xsize, float zsize, float height, NavMeshBrushTool.NavMeshBrushType brushType)
        {
            if (mesh && GLMaterial)
            {
                float type = brushType == NavMeshBrushTool.NavMeshBrushType.Box ? 1 : 0;
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

        private static void DrawWireCylinderInternal(Vector3 center, Vector3 size, Color color, bool seeThrough)
        {
            float dtAngle = 360f/12;
            GL.PushMatrix();
            GLMaterial.SetPass(!seeThrough ? 1 : 2);
            GL.MultMatrix(Handles.matrix);
            GL.Begin(GL.LINES);
            GL.Color(color);
            for (int i = 0; i < 12; i++)
            {
                float x0 = Mathf.Cos(i * dtAngle * Mathf.Deg2Rad) * size.x * 0.5f;
                float y0 = Mathf.Sin(i * dtAngle * Mathf.Deg2Rad) * size.x * 0.5f;
                float x1 = Mathf.Cos((i + 1) * dtAngle * Mathf.Deg2Rad) * size.x * 0.5f;
                float y1 = Mathf.Sin((i + 1) * dtAngle * Mathf.Deg2Rad) * size.x * 0.5f;

                Vector3 p0t = center + new Vector3(x0, size.y*0.5f, y0);
                Vector3 p0b = center + new Vector3(x0, -size.y * 0.5f, y0);

                Vector3 p1t = center + new Vector3(x1, size.y * 0.5f, y1);
                Vector3 p1b = center + new Vector3(x1, -size.y * 0.5f, y1);

                GL.Vertex(p0t);
                GL.Vertex(p0b);

                GL.Vertex(p1t);
                GL.Vertex(p1b);

                GL.Vertex(p0t);
                GL.Vertex(p1t);

                GL.Vertex(p0b);
                GL.Vertex(p1b);

            }
            GL.End();
            GL.PopMatrix();
        }
    }
}