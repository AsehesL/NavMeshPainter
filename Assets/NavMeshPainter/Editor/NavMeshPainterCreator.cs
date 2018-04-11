using UnityEngine;
using UnityEditor;
using System.Collections;
using ASL.NavMesh.Editor;

namespace ASL.NavMesh.Editor
{
    public class NavMeshPainterCreator : ScriptableWizard
    {

        private class Styles
        {

            public GUIContent angle = new GUIContent("Angle");
            public GUIContent maxDepth = new GUIContent("MaxDepth");
            public GUIContent create = new GUIContent("Create From Selection");
            public GUIContent containChilds = new GUIContent("Contain Childs");
            public GUIContent forceSet = new GUIContent("Force SetDepth");
        }

        private static Styles styles
        {
            get
            {
                if (s_Styles == null)
                    s_Styles = new Styles();
                return s_Styles;
            }
        }

        private static Styles s_Styles;

        private float m_Angle;
        private int m_MaxDepth = 4;
        private bool m_ForceSetDepth = false;
        private bool m_ContainChilds;

        private NavMeshPainterData m_Data;
        private NavMeshPainter m_Painter;

        public static void CreateWizard(NavMeshPainter painter = null, NavMeshPainterData data = null)
        {
            NavMeshPainterCreator creator = NavMeshPainterCreator.DisplayWizard<NavMeshPainterCreator>("NavMeshPainter");
            creator.minSize = new Vector2(400, 110);
            creator.maxSize = new Vector2(400, 110);
            creator.m_Data = data;
            creator.m_Painter = painter;
        }

        void OnGUI()
        {

            DrawAnglePreview(new Rect(5, 5, 100, 100), m_Angle);

            m_Angle = EditorGUI.Slider(new Rect(110, 5, position.width - 115, 20), styles.angle, m_Angle, 0, 180);

            m_ContainChilds = EditorGUI.Toggle(new Rect(110, 25, position.width - 115, 20), styles.containChilds,
                m_ContainChilds);

            m_MaxDepth = EditorGUI.IntSlider(new Rect(110, 45, position.width - 115, 20), styles.maxDepth, m_MaxDepth, 1,
                8);

            m_ForceSetDepth = EditorGUI.Toggle(new Rect(110, 65, position.width - 115, 20), styles.forceSet,
                m_ForceSetDepth);

            if (GUI.Button(new Rect(position.width - 155, position.height - 25, 150, 20), styles.create))
            {
                Create();
            }
        }

        private void Create()
        {
            if (Selection.gameObjects == null || Selection.gameObjects.Length == 0)
                return;
            if (m_Data != null)
            {
                m_Data.Create(Selection.gameObjects, m_ContainChilds, m_Angle*0.5f, m_MaxDepth, m_ForceSetDepth);
            }
            else
            {
                string savePath = EditorUtility.SaveFilePanel("Save NavMeshPainterData", "", "", "asset");
                savePath = FileUtil.GetProjectRelativePath(savePath);
                if (!string.IsNullOrEmpty(savePath))
                {

                    m_Data = NavMeshPainterData.CreateInstance<NavMeshPainterData>();
                    m_Data.Create(Selection.gameObjects, m_ContainChilds, m_Angle*0.5f, m_MaxDepth, m_ForceSetDepth);

                    AssetDatabase.CreateAsset(m_Data, savePath);
                    AssetDatabase.AddObjectToAsset(m_Data.renderMesh, m_Data);
                }

            }
            if (m_Painter != null)
            {
                m_Painter.data = m_Data;
                m_Painter = null;
            }
            m_Data = null;
            Close();
        }

        private void DrawAnglePreview(Rect rect, float angle)
        {
            GUI.Box(rect, string.Empty);

            DrawCircular(new Vector3(rect.x + rect.width*0.5f, rect.y + rect.height*0.6f, 0), rect.width*0.25f);
            DrawSector(new Vector3(rect.x + rect.width*0.5f, rect.y + rect.height*0.6f, 0), rect.width*0.45f, angle);
        }

        private void DrawCircular(Vector3 center, float radius)
        {
            Handles.color = Color.blue;

            float delta = 360*0.05f;

            for (int i = 0; i < 20; i++)
            {
                float ag = delta*i*Mathf.Deg2Rad;
                float nextag = delta*(i + 1)*Mathf.Deg2Rad;

                float x0 = radius*Mathf.Cos(ag);
                float y0 = radius*Mathf.Sin(ag);

                float x1 = radius*Mathf.Cos(nextag);
                float y1 = radius*Mathf.Sin(nextag);

                Handles.DrawLine(center + new Vector3(x0, y0, 0), center + new Vector3(x1, y1, 0));
            }
        }

        private void DrawSector(Vector3 center, float radius, float angle)
        {
            Handles.color = Color.green;

            float delta = 360*0.025f;
            int step = (int) (angle/delta);

            float dt = angle/step;

            float bgAngle = -90 - angle*0.5f;
            float edAngle = -90 + angle*0.5f;

            for (int i = 0; i < step; i++)
            {
                float ag = (bgAngle + dt*i)*Mathf.Deg2Rad;
                float nextag = (bgAngle + dt*(i + 1))*Mathf.Deg2Rad;

                float x0 = radius*Mathf.Cos(ag);
                float y0 = radius*Mathf.Sin(ag);

                float x1 = radius*Mathf.Cos(nextag);
                float y1 = radius*Mathf.Sin(nextag);

                Handles.DrawLine(center + new Vector3(x0, y0, 0), center + new Vector3(x1, y1, 0));
            }

            float xb = radius*Mathf.Cos(bgAngle*Mathf.Deg2Rad);
            float yb = radius*Mathf.Sin(bgAngle*Mathf.Deg2Rad);

            float xe = radius*Mathf.Cos(edAngle*Mathf.Deg2Rad);
            float ye = radius*Mathf.Sin(edAngle*Mathf.Deg2Rad);

            Handles.DrawLine(center, center + new Vector3(xb, yb, 0));
            Handles.DrawLine(center, center + new Vector3(xe, ye, 0));
        }
    }
}