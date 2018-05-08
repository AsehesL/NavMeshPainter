using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ASL.NavMesh.Editor
{
    [CustomNavMeshToolEditor(typeof (NavMeshBoxTool))]
    public class NavMeshBoxToolEditor : NavMeshToolEditor
    {
        private bool m_IsDragging;

        public override void DrawGUI()
        {
            var t = target as NavMeshBoxTool;
            if (t == null)
                return;

            GUILayout.Label(NavMeshPainterEditor.styles.setting, NavMeshPainterEditor.styles.boldLabel);
            
                t.topHeight = Mathf.Max(0.001f,
                    EditorGUILayout.FloatField(NavMeshPainterEditor.styles.topHeight, t.topHeight));
                t.bottomHeight = Mathf.Max(0.001f,
                    EditorGUILayout.FloatField(NavMeshPainterEditor.styles.bottomHeight, t.bottomHeight));
        }

        protected override void OnRaycast(NavMeshPainter targetPainter, RaycastHit hit)
        {
            var t = target as NavMeshBoxTool;
            if (t == null)
                return;
            if (Event.current.type == EventType.MouseMove)
            {
                if (!m_IsDragging)
                {
                    t.beginPos = hit.point;
                }
            }
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if (!m_IsDragging)
                {
                    t.beginPos = hit.point;
                    t.endPos = hit.point;
                    m_IsDragging = true;
                }
            }
            if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
            {
                if (m_IsDragging)
                {
                    t.endPos = hit.point;
                }
            }
        }

        protected override void OnSceneGUI(NavMeshPainter targetPainter)
        {
            NavMeshEditorUtils.ClearBrush();
            var t = target as NavMeshBoxTool;
            if (t == null)
                return;
            if (m_IsDragging)
            {
                

                NavMeshEditorUtils.DrawBounds(t.bounds, Color.blue);
            }
            else
            {
                NavMeshEditorUtils.DrawWireCube(t.beginPos, new Vector3(1, t.bottomHeight + t.topHeight, 1), Color.blue);
            }
            if (Event.current.type == EventType.MouseUp)
            {
                if (m_IsDragging)
                {
                    m_IsDragging = false;
                    ApplyPaint();
                }
            }
        }
    }
}