using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ASL.NavMeshPainter.Editor
{
//    [CustomPropertyDrawer(typeof (NavMeshSphereFillTool))]
    [CustomNavMeshToolEditor(typeof(NavMeshSphereFillTool))]
    public class NavMeshSphereFillToolEditor : NavMeshToolEditor
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