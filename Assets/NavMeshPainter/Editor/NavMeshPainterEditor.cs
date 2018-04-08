using UnityEngine;
using UnityEditor;
using System.Collections;
using ASL.NavMeshPainter;
using UnityEditor.AI;

[CustomEditor(typeof(NavMeshPainter))]
public class NavMeshPainterEditor : Editor
{
    public enum ToolType
    {
        None = -1,
        Paint,
        Erase,
        Mapping,
        Bake,
    }

    

    private class Styles
    {
        public GUIStyle buttonLeft = "ButtonLeft";
        public GUIStyle buttonMid = "ButtonMid";
        public GUIStyle buttonRight = "ButtonRight";
        public GUIStyle command = "Command";
        public GUIStyle box = "box";
        public GUIStyle toolbar = "Toolbar";
        public GUIStyle boldLabel = "BoldLabel";

        public GUIContent[] toolIcons = new GUIContent[]
        {
            EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSplat", "|Paint the NavMesh"),
            EditorGUIUtility.IconContent("TerrainInspector.TerrainToolPlants", "|Erase the NavMesh"),
            EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSmoothHeight", "|Sampling from texture"),
            EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSettings", "|Bake Setting")
        };
    }

    private static NavMeshPainterEditor.Styles styles;

    private static System.Reflection.MethodInfo intersectRayMesh
    {
        get
        {
            if (m_IntersectRayMesh == null)
            {
                var tp = typeof(HandleUtility);
                m_IntersectRayMesh = tp.GetMethod("IntersectRayMesh",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            }
            return m_IntersectRayMesh;
        }
    }
    private static System.Reflection.MethodInfo m_IntersectRayMesh;


    private Material m_DebugMaterial;

    private NavMeshPainter m_Target;

    private bool m_IsPainting;

    private bool m_IsDrawing;

    private ToolType m_ToolType = ToolType.None;

    [MenuItem("Tools/Create NavMeshPainter")]
    static void Init()
    {
        if (Selection.gameObjects == null || Selection.gameObjects.Length == 0)
            return;
        string savePath = EditorUtility.SaveFilePanel("保存NavMeshPainter", "", "", "asset");
        savePath = FileUtil.GetProjectRelativePath(savePath);
        if (string.IsNullOrEmpty(savePath))
            return;

        NavMeshPainter painter = new GameObject("[NavMeshPainter]").AddComponent<NavMeshPainter>();
        painter.gameObject.hideFlags = HideFlags.DontSave;

        NavMeshPainterData asset = NavMeshPainterData.CreateInstance<NavMeshPainterData>();
        asset.Create(Selection.gameObjects, true, 30, 0.03125f, 5);
        //asset.Create(Selection.gameObjects, true, 30, 0.125f);

        AssetDatabase.CreateAsset(asset, savePath);
        painter.painter = asset;
    }

    void OnEnable()
    {
        m_Target = (NavMeshPainter) target;
    }

    void OnSceneGUI()
    {
        if (styles == null)
        {
            styles = new Styles();
        }
        if (m_DebugMaterial == null)
        {
            m_DebugMaterial = new Material(Shader.Find("Unlit/NavMeshDebug"));
            m_DebugMaterial.hideFlags = HideFlags.HideAndDontSave;
            if (m_Target.painter)
                m_DebugMaterial.SetFloat("_CellSize", Mathf.Sqrt(m_Target.painter.Area*200));
            else
                m_DebugMaterial.SetFloat("_CellSize", 2.5f);
        }
        if (m_Target.painter)
        {
            //m_DebugMaterial.SetVector("_BrushPos");
            m_Target.painter.DrawMesh(m_DebugMaterial);
        }
        if (m_IsPainting)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            RaycastHit hit;
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                m_IsDrawing = true;
            }
            if (Event.current.type == EventType.MouseUp)
            {
                m_IsDrawing = false;
            }
            if (m_Target.painter && m_Target.painter.renderMesh)
            {
                if (RayCastInSceneView(m_Target.painter.renderMesh, out hit))
                {
                    m_Target.brush.position = hit.point;
                    //m_DebugMaterial.SetColor("_BrushColor", new Color(0, 0.5f, 1, 0.5f));
                    m_DebugMaterial.SetVector("_BrushPos", m_Target.brush.position);
                    m_DebugMaterial.SetVector("_BrushSize", new Vector2(m_Target.brush.radius, m_Target.brush.maxHeight));

                    if (m_IsDrawing && Event.current.type == EventType.MouseDrag)
                    {
                        if (m_Target.painter)
                            m_Target.painter.Draw(m_Target.brush);
                    }
                }
                else
                {
                    //m_DebugMaterial.SetColor("_BrushColor", new Color(0, 0.5f, 1, 0.0f));
                }
            }
        }

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (styles == null)
        {
            styles = new Styles();
        }
        DrawToolsGUI();
        switch (m_ToolType)
        {
            case ToolType.None:

                break;
            case ToolType.Paint:
                DrawPaintSettingGUI();
                break;
        }

        if (GUILayout.Button("Check"))
        {
            if (m_Target.painter)
            {
                m_Target.painter.Check();
            }
        }
        if (GUILayout.Button("CheckTriangle"))
        {
            if (m_Target.painter)
            {
                m_Target.painter.CheckTriangle();
            }
        }
        if (GUILayout.Button("Bake"))
        {
            if (m_Target.painter)
            {
                MeshFilter mf = new GameObject("StForBake").AddComponent<MeshFilter>();
                //mf.gameObject.hideFlags = HideFlags.HideAndDontSave;
                mf.gameObject.isStatic = true;
                mf.sharedMesh = m_Target.painter.GenerateMesh();

                MeshRenderer mr = mf.gameObject.AddComponent<MeshRenderer>();
                mr.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
                mr.sharedMaterial.hideFlags = HideFlags.HideAndDontSave;
                mr.sharedMaterial.SetColor("_Color", Color.red);

                NavMeshBuilder.BuildNavMesh();
                DestroyImmediate(mf.gameObject);

                
            }
        }
        m_IsPainting = GUILayout.Toggle(m_IsPainting, "绘制", GUI.skin.FindStyle("Button"));
    }

    private void DrawToolsGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        int selectedTool = (int)this.m_ToolType;
        int num = GUILayout.Toolbar(selectedTool, styles.toolIcons, styles.command, new GUILayoutOption[0]);
        if (num != selectedTool)
        {
            this.m_ToolType = (ToolType)num;
//            InspectorWindow.RepaintAllInspectors();
            //if (Toolbar.get != null)
            //{
            //    Toolbar.get.Repaint();
            //}
        }
        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();
    }

    private void DrawPaintSettingGUI()
    {
        GUILayout.Label("Paint Setting", styles.boldLabel);
        GUILayout.BeginVertical(styles.box);
        GUILayout.Label("Paint Tool", styles.boldLabel);

        GUILayout.BeginHorizontal();

        m_Target.paintTool = GUILayout.Toggle(m_Target.paintTool == PaintToolType.Brush, styles.toolIcons[0],
            styles.buttonLeft, GUILayout.Width(35))
            ? PaintToolType.Brush
            : m_Target.paintTool;

        m_Target.paintTool = GUILayout.Toggle(m_Target.paintTool == PaintToolType.Line, styles.toolIcons[0],
           styles.buttonMid, GUILayout.Width(35))
           ? PaintToolType.Line
           : m_Target.paintTool;

        m_Target.paintTool = GUILayout.Toggle(m_Target.paintTool == PaintToolType.Box, styles.toolIcons[0],
           styles.buttonMid, GUILayout.Width(35))
           ? PaintToolType.Box
           : m_Target.paintTool;

        m_Target.paintTool = GUILayout.Toggle(m_Target.paintTool == PaintToolType.Sphere, styles.toolIcons[0],
           styles.buttonMid, GUILayout.Width(35))
           ? PaintToolType.Sphere
           : m_Target.paintTool;

        m_Target.paintTool = GUILayout.Toggle(m_Target.paintTool == PaintToolType.Cylinder, styles.toolIcons[0],
           styles.buttonRight, GUILayout.Width(35))
           ? PaintToolType.Cylinder
           : m_Target.paintTool;

        GUILayout.EndHorizontal();

        GUILayout.Label("Settings", styles.boldLabel);
        IPaintingTool tool = m_Target.GetPaintingTool();
        if (tool != null)
            tool.DrawGUI();

        GUILayout.EndVertical();
    }


    private static bool RayCastInSceneView(Mesh mesh, out RaycastHit hit)
    {
        hit = default(RaycastHit);
        if (!mesh)
            return false;
        if (UnityEditor.Tools.viewTool != ViewTool.Pan)
            return false;
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        SceneView.RepaintAll();
        if (RaycastMesh(ray, mesh, Matrix4x4.identity, out hit))
            return true;
        return false;
    }
    private static bool RaycastMesh(Ray ray, Mesh mesh, Matrix4x4 matrix, out RaycastHit hit)
    {
        var parameters = new object[] { ray, mesh, matrix, null };
        bool result = (bool)intersectRayMesh.Invoke(null, parameters);
        hit = (RaycastHit)parameters[3];
        return result;
    }

}
