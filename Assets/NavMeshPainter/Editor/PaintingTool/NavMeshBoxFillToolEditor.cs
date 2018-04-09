using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ASL.NavMeshPainter.Editor
{
    //[CustomPropertyDrawer(typeof (NavMeshBoxFillTool))]
    [CustomNavMeshToolEditor(typeof(NavMeshBoxFillTool))]
    public class NavMeshBoxFillToolEditor : NavMeshToolEditor
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