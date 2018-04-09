using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ASL.NavMeshPainter.Editor
{
    //[CustomPropertyDrawer(typeof (NavMeshCylinderFillTool))]
    [CustomNavMeshToolEditor(typeof(NavMeshCylinderFillTool))]
    public class NavMeshCylinderFillToolEditor : NavMeshToolEditor
    {
        public override void OnGUI()
        {
        }

        public override void OnSceneGUI(Material renderMaterial)
        {
            renderMaterial.SetColor("_BrushColor", Color.clear);
        }
    }
}