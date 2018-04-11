using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ASL.NavMesh.Editor
{
    //[CustomPropertyDrawer(typeof(NavMeshBrushTool))]
    [CustomNavMeshToolEditor(typeof(NavMeshBrushTool))]
    public class NavMeshBrushToolEditor : NavMeshToolEditor
    {
        

        public override void DrawGUI()
        {
            var t = target as NavMeshBrushTool;
            if (t == null)
                return;

            GUILayout.Label(NavMeshPainterEditor.styles.setting, NavMeshPainterEditor.styles.boldLabel);

            if (t.brushType == NavMeshBrushTool.NavMeshBrushType.Box)
            {
                t.length = Mathf.Max(0.001f,
                    EditorGUILayout.FloatField(NavMeshPainterEditor.styles.xSize, t.length));
                t.width = Mathf.Max(0.001f,
                    EditorGUILayout.FloatField(NavMeshPainterEditor.styles.zSize, t.width));
                t.height = Mathf.Max(0,
                    EditorGUILayout.FloatField(NavMeshPainterEditor.styles.maxHeight, t.height));
            }else if (t.brushType == NavMeshBrushTool.NavMeshBrushType.Cylinder)
            {
                t.length = Mathf.Max(0.001f,
                    EditorGUILayout.FloatField(NavMeshPainterEditor.styles.radius, t.length));
                t.height = Mathf.Max(0,
                    EditorGUILayout.FloatField(NavMeshPainterEditor.styles.maxHeight, t.height));
            }
            else if (t.brushType == NavMeshBrushTool.NavMeshBrushType.Sphere)
            {
                t.length = Mathf.Max(0.001f,
                    EditorGUILayout.FloatField(NavMeshPainterEditor.styles.radius, t.length));
            }

            t.brushType = (NavMeshBrushTool.NavMeshBrushType)EditorGUILayout.EnumPopup(NavMeshPainterEditor.styles.brushType, t.brushType);
        }

        protected override void OnRaycast(NavMeshPainter targetPainter, RaycastHit hit)
        {
            var t = target as NavMeshBrushTool;
            if (t == null)
                return;
            t.position = hit.point;
            if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
            {
                ApplyPaint();
            }
        }

        protected override void OnSceneGUI(NavMeshPainter targetPainter)
        {
            var t = this.target as NavMeshBrushTool;
            if (t != null)
            {
                NavMeshEditorUtils.DrawBounds(t.bounds, Color.blue);
                NavMeshEditorUtils.DrawBrush(targetPainter.GetRenderMeshes(), Matrix4x4.identity, t.position, t.length, t.width, t.height,
                    t.brushType);
            }
        }
    }
}