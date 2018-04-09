using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using ASL.NavMesh;
using ASL.NavMesh.Editor;
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
        public GUIContent checkerboardIcon = EditorGUIUtility.IconContent("NavMeshPainter/checkerboard.png");
    }

    public static NavMeshPainterEditor.Styles styles;

    private NavMeshPainter m_Target;

    private ToolType m_ToolType = ToolType.None;

    private Dictionary<System.Type, NavMeshToolEditor> m_ToolEditors;

    private bool m_ShowCheckerBoard = true;

    private Texture2D m_RoadMask;

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

        float minSize = m_Target.GetMinSize();
        NavMeshEditorUtils.SetCheckerBoardCellSize(minSize);
        NavMeshEditorUtils.SetMaskTexture(null);
    }

    void OnSceneGUI()
    {
        if (styles == null)
        {
            styles = new Styles();
        }
        if(m_ShowCheckerBoard)
            NavMeshEditorUtils.DrawCheckerBoard(m_Target.GetRenderMesh(), Matrix4x4.identity);

        switch (m_ToolType)
        {
            case ToolType.Erase:
            case ToolType.Paint:
                DrawPaintingToolSceneGUI();
                break;
            case ToolType.Mapping:
                DrawMappingSceneGUI();
                break;
        }

    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
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

        EditorGUILayout.Space();
    }

    private void DrawToolsGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUI.BeginChangeCheck();
        m_ShowCheckerBoard = GUILayout.Toggle(m_ShowCheckerBoard, styles.checkerboardIcon, styles.command);
        if (EditorGUI.EndChangeCheck())
        {
            float minSize = m_Target.GetMinSize();
            NavMeshEditorUtils.SetCheckerBoardCellSize(minSize);
        }

        EditorGUILayout.Space();

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

        GUILayout.EndHorizontal();

        var tooleditor = GetPaintingToolEditor(m_Target.GetPaintingTool());
        if (tooleditor != null)
        {
            tooleditor.DrawGUI();
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
        GUILayout.Label("Mapping Setting", styles.boldLabel);
        GUILayout.BeginVertical(styles.box);
        GUILayout.Label("Mask Texture", styles.boldLabel);

        EditorGUI.BeginChangeCheck();
        m_RoadMask = EditorGUILayout.ObjectField("Mask", m_RoadMask, typeof (Texture2D), false) as Texture2D;
        if (EditorGUI.EndChangeCheck())
            NavMeshEditorUtils.SetMaskTexture(m_RoadMask);

        if (GUILayout.Button("ApplyMask"))
        {
            ApplyMask();
        }
        if (GUILayout.Button("Bake"))
        {
            Bake();
        }

        GUILayout.EndVertical();
    }

    private void DrawBakeSettingGUI()
    {
        
    }

    private void DrawPaintingToolSceneGUI()
    {
        IPaintingTool tool = m_Target.GetPaintingTool();
        if (tool == null)
            return;
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));


        if (m_Target.painter && m_Target.painter.renderMesh)
        {
            var tooleditor = GetPaintingToolEditor(tool);
            tooleditor.DrawSceneGUI(m_Target);
        }
    }

    private void DrawMappingSceneGUI()
    {
        NavMeshEditorUtils.DrawMask(m_Target.GetRenderMesh(), Matrix4x4.identity);
    }

    private void ApplyPaint(IPaintingTool tool)
    {
        if (tool != null)
        {
            if (m_ToolType == ToolType.Paint)
                m_Target.Draw(tool);
            else if (m_ToolType == ToolType.Erase)
                m_Target.Erase(tool);
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
                            m_ToolEditors[a.navMeshToolType].SetApplyAction(new System.Action<IPaintingTool>(ApplyPaint));
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
        Mesh mesh = m_Target.GenerateMesh();
        if (mesh)
        {
            MeshFilter mf = new GameObject("StForBake").AddComponent<MeshFilter>();
            //mf.gameObject.hideFlags = HideFlags.HideAndDontSave;
            mf.gameObject.isStatic = true;
            mf.sharedMesh = mesh;

            MeshRenderer mr = mf.gameObject.AddComponent<MeshRenderer>();
            mr.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
            mr.sharedMaterial.hideFlags = HideFlags.HideAndDontSave;
            mr.sharedMaterial.SetColor("_Color", Color.red);

            NavMeshBuilder.BuildNavMesh();
            DestroyImmediate(mf.gameObject);
        }
    }

    private void ApplyMask()
    {
        if (m_RoadMask == null)
            return;
        RenderTexture rt = RenderTexture.GetTemporary(m_RoadMask.width, m_RoadMask.height, 0);
        Graphics.Blit(m_RoadMask, rt);

        RenderTexture active = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D cont = new Texture2D(rt.width, rt.height);
        cont.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        cont.Apply();
        RenderTexture.active = active;

        RenderTexture.ReleaseTemporary(rt);

        m_RoadMask = null;
        m_Target.SamplingFromTexture(cont);

        DestroyImmediate(cont);
        NavMeshEditorUtils.SetMaskTexture(null);
    }

}
