using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using ASL.NavMeshPainter;
using ASL.NavMeshPainter.Editor;
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

    public class Styles
    {
        public GUIStyle buttonLeft = "ButtonLeft";
        public GUIStyle buttonMid = "ButtonMid";
        public GUIStyle buttonRight = "ButtonRight";
        public GUIStyle command = "Command";
        public GUIStyle box = "box";
        public GUIStyle boldLabel = "BoldLabel";

        public GUIContent[] toolIcons = new GUIContent[]
        {
            EditorGUIUtility.IconContent("NavMeshPainter/add.png", "|Paint the NavMesh"),
            EditorGUIUtility.IconContent("NavMeshPainter/reduce.png", "|Erase the NavMesh"),
            EditorGUIUtility.IconContent("NavMeshPainter/texture.png", "|Sampling from texture"),
            EditorGUIUtility.IconContent("NavMeshPainter/nm.png", "|Bake Setting")
        };

        public GUIContent brushIcon = EditorGUIUtility.IconContent("NavMeshPainter/brush.png");
        public GUIContent eraserIcon = EditorGUIUtility.IconContent("NavMeshPainter/eraser.png");
        public GUIContent lineIcon = EditorGUIUtility.IconContent("NavMeshPainter/line.png");
        public GUIContent boxIcon = EditorGUIUtility.IconContent("NavMeshPainter/box.png");
        public GUIContent cylinderIcon = EditorGUIUtility.IconContent("NavMeshPainter/cylinder.png");
        public GUIContent sphereIcon = EditorGUIUtility.IconContent("NavMeshPainter/sphere.png");
    }

    public static NavMeshPainterEditor.Styles styles;

    //private NavMeshToolEditor m_NavMeshBrush;
    //private NavMeshToolEditor m_NavMeshLineTool;
    //private NavMeshToolEditor m_NavMeshBoxFillTool;
    //private NavMeshToolEditor m_NavMeshCylinderFillTool;
    //private NavMeshToolEditor m_NavMeshSphereFillTool;

    private NavMeshPainter m_Target;

    private ToolType m_ToolType = ToolType.None;

    private Dictionary<System.Type, NavMeshToolEditor> m_ToolEditors;

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

        if (m_Target)
            m_Target.ResetState();

//        m_NavMeshBrush = serializedObject.FindProperty("brush");
//        m_NavMeshLineTool = serializedObject.FindProperty("lineTool");
//        m_NavMeshBoxFillTool = serializedObject.FindProperty("boxFillTool");
//        m_NavMeshCylinderFillTool = serializedObject.FindProperty("cylinderFillTool");
//        m_NavMeshSphereFillTool = serializedObject.FindProperty("sphereFillTool");
    }

    void OnSceneGUI()
    {
        if (styles == null)
        {
            styles = new Styles();
        }
//        if (m_DebugMaterial == null)
//        {
//            m_DebugMaterial = new Material((Shader)EditorGUIUtility.Load("NavMeshPainter/Shader/NavMeshDebug.shader"));
//            
//            m_DebugMaterial.hideFlags = HideFlags.HideAndDontSave;
//            if (m_Target.painter)
//                m_DebugMaterial.SetFloat("_CellSize", Mathf.Sqrt(m_Target.painter.Area*200));
//            else
//                m_DebugMaterial.SetFloat("_CellSize", 2.5f);
//        }
        if (m_Target.painter)
        {
            //m_DebugMaterial.SetVector("_BrushPos");
            //m_Target.painter.DrawMesh(m_DebugMaterial);
            NavMeshEditorUtils.DrawCheckerBoard(m_Target.painter.renderMesh, Matrix4x4.identity);
        }
        DrawPaintingToolSceneGUI();

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
            case ToolType.Erase:
                DrawPaintSettingGUI(true);
                break;
            case ToolType.Mapping:
                DrawTextureMappingGUI();
                break;
            case ToolType.Bake:
                DrawBakeSettingGUI();
                break;
        }
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
        }
        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();
    }

    private void DrawPaintSettingGUI(bool erase = false)
    {
        GUILayout.Label("Paint Setting", styles.boldLabel);
        GUILayout.BeginVertical(styles.box);
        GUILayout.Label("Paint Tool", styles.boldLabel);

        GUILayout.BeginHorizontal();

        var brushIcon = erase ? styles.eraserIcon : styles.brushIcon;
        m_Target.paintTool = GUILayout.Toggle(m_Target.paintTool == PaintingToolType.Brush, brushIcon,
            styles.buttonLeft, GUILayout.Width(35))
            ? PaintingToolType.Brush
            : m_Target.paintTool;

        m_Target.paintTool = GUILayout.Toggle(m_Target.paintTool == PaintingToolType.Line, styles.lineIcon,
           styles.buttonMid, GUILayout.Width(35))
           ? PaintingToolType.Line
           : m_Target.paintTool;

        m_Target.paintTool = GUILayout.Toggle(m_Target.paintTool == PaintingToolType.Box, styles.boxIcon,
           styles.buttonMid, GUILayout.Width(35))
           ? PaintingToolType.Box
           : m_Target.paintTool;

        m_Target.paintTool = GUILayout.Toggle(m_Target.paintTool == PaintingToolType.Sphere, styles.sphereIcon,
           styles.buttonMid, GUILayout.Width(35))
           ? PaintingToolType.Sphere
           : m_Target.paintTool;

        m_Target.paintTool = GUILayout.Toggle(m_Target.paintTool == PaintingToolType.Cylinder, styles.cylinderIcon,
           styles.buttonRight, GUILayout.Width(35))
           ? PaintingToolType.Cylinder
           : m_Target.paintTool;

        GUILayout.EndHorizontal();

        var tooleditor = GetPaintingToolEditor(m_Target.GetPaintingTool());
        if (tooleditor != null)
        {
            tooleditor.OnGUI();
        }
        //EditorGUILayout.PropertyField(tooleditor);

        if (GUILayout.Button("Bake"))
        {
            Bake();
        }

        GUILayout.EndVertical();
    }

    private void DrawTextureMappingGUI()
    {
        
    }

    private void DrawBakeSettingGUI()
    {
        
    }

    private void DrawPaintingToolSceneGUI()
    {
        if (IsToolHasSceneGUI(m_ToolType))
        {
            IPaintingTool tool = m_Target.GetPaintingTool();
            if (tool == null)
                return;
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            RaycastHit hit;
            
            
            if (m_Target.painter && m_Target.painter.renderMesh)
            {
                if (NavMeshEditorUtils.RayCastInSceneView(m_Target.painter.renderMesh, out hit))
                {
                    var tooleditor = GetPaintingToolEditor(tool);
                    if (tooleditor != null)
                        tooleditor.OnSceneGUI(NavMeshEditorUtils.GLMaterial);

                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        if (tool.OnMouseDown(hit.point))
                            ApplyPaint(tool);
                    }

                    if (Event.current.type == EventType.MouseDrag)
                    {
                        if(tool.OnMouseDrag(hit.point))
                            ApplyPaint(tool);
                    }
                    if (Event.current.type == EventType.MouseMove)
                    {
                        tool.OnMouseMove(hit.point);
                    }
                }
            }

            if (Event.current.type == EventType.MouseUp)
            {
                if(tool.OnMouseUp())
                    ApplyPaint(tool);
            }
        }
    }
    

    private bool IsToolHasSceneGUI(ToolType toolType)
    {
        if (toolType == ToolType.Paint)
            return true;
        if (toolType == ToolType.Erase)
            return true;
        return false;
    }

    private void ApplyPaint(IPaintingTool tool)
    {
        if (m_Target.painter != null && tool != null)
        {
            if (m_ToolType == ToolType.Paint)
                m_Target.painter.Draw(tool);
            else if (m_ToolType == ToolType.Erase)
                m_Target.painter.Erase(tool);
        }
    }

    private NavMeshToolEditor GetPaintingToolEditor(IPaintingTool tool)
    {
        System.Type tooltype = tool.GetType();
        NavMeshToolEditor editor = null;
        if (m_ToolEditors == null)
        {
            m_ToolEditors = new Dictionary<System.Type, NavMeshToolEditor>();
            System.Reflection.Assembly assembly = this.GetType().Assembly;
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(typeof (NavMeshToolEditor)))
                {
                    var attributes = type.GetCustomAttributes(typeof (CustomNavMeshToolEditorAttribute), false);
                    foreach (var att in attributes)
                    {
                        CustomNavMeshToolEditorAttribute a = att as CustomNavMeshToolEditorAttribute;
                        if (a == null)
                            continue;
                        if (!m_ToolEditors.ContainsKey(a.navMeshToolType))
                        {
                            m_ToolEditors[a.navMeshToolType] = (NavMeshToolEditor)System.Activator.CreateInstance(type);

                        }
                    }
                }
            }
        }
        if (m_ToolEditors.ContainsKey(tooltype))
        {
            editor = m_ToolEditors[tooltype];
            editor.SetTool(tool);
        }
        return editor;
    }

    private void Bake()
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

    

}
