using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NavMeshPaintTool : EditorWindow {

    [MenuItem("Tools/NavMeshPainter")]
    static void Init()
    {
        NavMeshPaintTool win = NavMeshPaintTool.GetWindow<NavMeshPaintTool>();
        win.titleContent = new GUIContent("NavMeshPainter");
    }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        
    }

    void OnSceneGUI(SceneView sceneView)
    {
        
    }

    void OnGUI()
    {
        
    }
}
