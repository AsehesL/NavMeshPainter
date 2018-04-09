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

            if (t.brushType == NavMeshBrushType.Box)
            {
                t.xSize = Mathf.Max(0.001f,
                    EditorGUILayout.FloatField(NavMeshPainterEditor.styles.xSize, t.xSize));
                t.zSize = Mathf.Max(0.001f,
                    EditorGUILayout.FloatField(NavMeshPainterEditor.styles.zSize, t.zSize));
                t.maxHeight = Mathf.Max(0,
                    EditorGUILayout.FloatField(NavMeshPainterEditor.styles.maxHeight, t.maxHeight));
            }else if (t.brushType == NavMeshBrushType.Cylinder)
            {
                t.xSize = Mathf.Max(0.001f,
                    EditorGUILayout.FloatField(NavMeshPainterEditor.styles.radius, t.xSize));
                t.maxHeight = Mathf.Max(0,
                    EditorGUILayout.FloatField(NavMeshPainterEditor.styles.maxHeight, t.maxHeight));
            }
            else if (t.brushType == NavMeshBrushType.Sphere)
            {
                t.xSize = Mathf.Max(0.001f,
                    EditorGUILayout.FloatField(NavMeshPainterEditor.styles.radius, t.xSize));
            }

            t.brushType = (NavMeshBrushType)EditorGUILayout.EnumPopup(NavMeshPainterEditor.styles.brushType, t.brushType);
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
                NavMeshEditorUtils.DrawBounds(t.Bounds, Color.blue);
                NavMeshEditorUtils.DrawBrush(targetPainter.GetRenderMesh(), Matrix4x4.identity, t.position, t.xSize, t.zSize, t.maxHeight,
                    t.brushType);
            }
        }
    }
}