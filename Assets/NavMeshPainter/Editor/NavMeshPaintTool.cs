﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ASL.NavMesh;

public class NavMeshPaintTool : EditorWindow
{


    private NavMeshPainterData m_Painter;

    [MenuItem("Tools/NavMeshPainter")]
    static void Init()
    {
        NavMeshPaintTool win = NavMeshPaintTool.GetWindow<NavMeshPaintTool>();
        win.titleContent = new GUIContent("NavMeshPainter");
    }

    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    void OnDisable()
    {
        if (SceneView.onSceneGUIDelegate != null) SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        
    }

    void OnGUI()
    {
        m_Painter =
            EditorGUILayout.ObjectField("绘制器", m_Painter, typeof (NavMeshPainterData), false) as NavMeshPainterData;
        if (GUILayout.Button("拾取选中物体并创建八叉树"))
        {
            
        }
    }
}
