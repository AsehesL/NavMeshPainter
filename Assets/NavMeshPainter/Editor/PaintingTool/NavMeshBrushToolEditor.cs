using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ASL.NavMeshPainter.Editor
{
    //[CustomPropertyDrawer(typeof(NavMeshBrushTool))]
    [CustomNavMeshToolEditor(typeof(NavMeshBrushTool))]
    public class NavMeshBrushToolEditor : NavMeshToolEditor
    {
        

        public override void OnGUI()
        {
            var t = target as NavMeshBrushTool;
            if (t == null)
                return;

            GUILayout.Label("Settings", NavMeshPainterEditor.styles.boldLabel);
            t.size = Mathf.Max(0.001f,
                EditorGUILayout.FloatField("Size", t.size));
            t.maxHeight = Mathf.Max(0,
                EditorGUILayout.FloatField("MaxHeight", t.maxHeight));

            t.brushType = (NavMeshBrushTool.BrushType)EditorGUILayout.EnumPopup("BrushType", t.brushType);
        }

        public override void OnSceneGUI(Material renderMaterial)
        {
            var t = target as NavMeshBrushTool;
            if (t == null)
                return;

            NavMeshEditorUtils.DrawBounds(t.Bounds, Color.blue);

            if (renderMaterial == null)
                return;
            renderMaterial.SetVector("_BrushPos", t.position);
            renderMaterial.SetVector("_BrushSize", new Vector3(t.size, t.maxHeight, (float)t.brushType));
            renderMaterial.SetColor("_BrushColor", new Color(0, 0.5f, 1, 0.5f));
        }
    }
}