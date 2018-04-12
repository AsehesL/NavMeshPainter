using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ASL.NavMesh.Editor
{
//    [CustomPropertyDrawer(typeof (NavMeshLineTool))]
    [CustomNavMeshToolEditor(typeof (NavMeshLineTool))]
    public class NavMeshLineToolEditor : NavMeshToolEditor
    {
        private enum State
        {
            None,
            Drag,
        }

        private State m_CurrentState;

        public override void DrawGUI()
        {
            GUILayout.Label(NavMeshPainterEditor.styles.setting, NavMeshPainterEditor.styles.boldLabel);
            var t = target as NavMeshLineTool;
            if (t == null)
                return;

            t.width = Mathf.Max(0.001f, EditorGUILayout.FloatField(NavMeshPainterEditor.styles.width, t.width));
            t.height = Mathf.Max(0.01f,
                        EditorGUILayout.FloatField(NavMeshPainterEditor.styles.maxHeight, t.height));
        }

        protected override void OnSceneGUI(NavMeshPainter targetPainter)
        {
            NavMeshEditorUtils.ClearBrush();
            var t = target as NavMeshLineTool;
            if (t == null)
                return;
            if (m_CurrentState == State.Drag)
            {
                DrawArea(t, Color.blue);
            }
            else
            {
                NavMeshEditorUtils.DrawWireCube(t.beginPos, new Vector3(t.width, t.height, t.width), Color.blue);
            }
            if (Event.current.type == EventType.MouseUp)
            {
                if (m_CurrentState == State.Drag)
                {
                    m_CurrentState = State.None;
                    ApplyPaint();
                }
            }
        }

        protected override void OnRaycast(NavMeshPainter targetPainter, RaycastHit hit)
        {
            var t = target as NavMeshLineTool;
            if (t == null)
                return;
            if (Event.current.type == EventType.MouseMove)
            {
                if (m_CurrentState == State.None)
                {
                    t.beginPos = hit.point;
                }
            }
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if (m_CurrentState == State.None)
                {
                    t.beginPos = hit.point;
                    t.endPos = hit.point;
                    m_CurrentState = State.Drag;
                }
            }
            if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
            {
                if (m_CurrentState == State.Drag)
                {
                    t.endPos = hit.point;
                }
            }
        }

        private void DrawArea(NavMeshLineTool tool, Color color)
        {

            Vector3 toEnd = tool.endPos - tool.beginPos;

            Vector3 hVector = Vector3.Cross(toEnd, Vector3.up).normalized;

            Vector3 p0 = tool.beginPos - hVector * tool.width * 0.5f - Vector3.up * tool.height;
            Vector3 p1 = tool.beginPos - hVector * tool.width * 0.5f + Vector3.up * tool.height;
            Vector3 p2 = tool.beginPos + hVector * tool.width * 0.5f + Vector3.up * tool.height;
            Vector3 p3 = tool.beginPos + hVector * tool.width * 0.5f - Vector3.up * tool.height;

            Vector3 p4 = tool.endPos - hVector * tool.width * 0.5f - Vector3.up * tool.height;
            Vector3 p5 = tool.endPos - hVector * tool.width * 0.5f + Vector3.up * tool.height;
            Vector3 p6 = tool.endPos + hVector * tool.width * 0.5f + Vector3.up * tool.height;
            Vector3 p7 = tool.endPos + hVector * tool.width * 0.5f - Vector3.up * tool.height;

            NavMeshEditorUtils.DrawLine(p0, p1, color);
            NavMeshEditorUtils.DrawLine(p1, p2, color);
            NavMeshEditorUtils.DrawLine(p2, p3, color);
            NavMeshEditorUtils.DrawLine(p3, p0, color);

            NavMeshEditorUtils.DrawLine(p4, p5, color);
            NavMeshEditorUtils.DrawLine(p5, p6, color);
            NavMeshEditorUtils.DrawLine(p6, p7, color);
            NavMeshEditorUtils.DrawLine(p7, p4, color);

            NavMeshEditorUtils.DrawLine(p0, p4, color);
            NavMeshEditorUtils.DrawLine(p1, p5, color);
            NavMeshEditorUtils.DrawLine(p2, p6, color);
            NavMeshEditorUtils.DrawLine(p3, p7, color);
        }
    }
}