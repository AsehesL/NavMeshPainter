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

        public GUIContent paintSetting = new GUIContent("Paint Setting");
        public GUIContent paintTool = new GUIContent("Paint Tool");
        public GUIContent bake = new GUIContent("Bake");
        public GUIContent mappingSetting = new GUIContent("Mapping Setting");
        public GUIContent maskTexture = new GUIContent("Mask Texture");
        public GUIContent mask = new GUIContent("Mask");
        public GUIContent applyMask = new GUIContent("ApplyMask");
        public GUIContent setting = new GUIContent("Settings");
        public GUIContent xSize = new GUIContent("XSize");
        public GUIContent zSize = new GUIContent("ZSize");
        public GUIContent maxHeight = new GUIContent("MaxHeight");
        public GUIContent radius = new GUIContent("Radius");
        public GUIContent brushType = new GUIContent("BrushType");
        public GUIContent width = new GUIContent("Width");
        public GUIContent painterData = new GUIContent("PainterData");
        public GUIContent create = new GUIContent("Create New Data");
        public GUIContent generateMesh = new GUIContent("Refresh Preview Mesh");
        public GUIContent wireColor = new GUIContent("WireColor");
        public GUIContent previewMeshColor = new GUIContent("PreviewMesh Color");
    }

    public static NavMeshPainterEditor.Styles styles
    {
        get
        {
            if (s_Styles == null)
                s_Styles = new Styles();
            return s_Styles;
        }
    }

    private static NavMeshPainterEditor.Styles s_Styles;

    private NavMeshPainter m_Target;

    private ToolType m_ToolType = ToolType.None;

    private Dictionary<System.Type, NavMeshToolEditor> m_ToolEditors;

    private bool m_ShowCheckerBoard = true;

    private Texture2D m_RoadMask;

    private Color m_PreviewMeshColor = Color.red;
    private GameObject m_PreviewMeshObj;

    private Material m_PreviewMaterial;

    [MenuItem("GameObject/NavMeshPainter/Create NavMeshPainter")]
    static void Create()
    {
        NavMeshPainter painter = new GameObject("NavMesh Painter").AddComponent<NavMeshPainter>();
    }

    void OnEnable()
    {
        m_Target = (NavMeshPainter) target;

        ResetCheckerBoardCellSize();
        NavMeshEditorUtils.SetMaskTexture(null);
    }

    void OnSceneGUI()
    {
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
        if (m_Target.painter == null)
        {
            DrawNoDataGUI();
            return;
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

    private void DrawNoDataGUI()
    {
        EditorGUI.BeginChangeCheck();
        m_Target.painter =
            EditorGUILayout.ObjectField(styles.painterData, m_Target.painter, typeof (NavMeshPainterData), false) as
                NavMeshPainterData;
        if(EditorGUI.EndChangeCheck())
            if (m_Target.painter != null)
                ResetCheckerBoardCellSize();
        if (GUILayout.Button(styles.create))
        {
            CreateNewNavMeshPainterData();
        }
        EditorGUILayout.HelpBox("No PainterData has been setted!", MessageType.Error);
    }

    private void DrawToolsGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUI.BeginChangeCheck();
        m_ShowCheckerBoard = GUILayout.Toggle(m_ShowCheckerBoard, styles.checkerboardIcon, styles.command);
        if (EditorGUI.EndChangeCheck())
        {
            ResetCheckerBoardCellSize();
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
        GUILayout.Label(styles.paintSetting, styles.boldLabel);
        GUILayout.BeginVertical(styles.box);
        GUILayout.Label(styles.paintTool, styles.boldLabel);

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

        if (GUILayout.Button(styles.bake))
        {
            Bake();
        }

        GUILayout.EndVertical();
    }

    private void DrawTextureMappingGUI()
    {
        GUILayout.Label(styles.mappingSetting, styles.boldLabel);
        GUILayout.BeginVertical(styles.box);
        GUILayout.Label(styles.maskTexture, styles.boldLabel);

        EditorGUI.BeginChangeCheck();
        m_RoadMask = EditorGUILayout.ObjectField(styles.mask, m_RoadMask, typeof (Texture2D), false) as Texture2D;
        if (EditorGUI.EndChangeCheck())
            NavMeshEditorUtils.SetMaskTexture(m_RoadMask);

        if (GUILayout.Button(styles.applyMask))
        {
            ApplyMask();
        }
        if (GUILayout.Button(styles.bake))
        {
            Bake();
        }

        GUILayout.EndVertical();
    }

    private void DrawBakeSettingGUI()
    {
        GUILayout.Label(styles.setting, styles.boldLabel);
        EditorGUI.BeginChangeCheck();
        m_Target.painter =
            EditorGUILayout.ObjectField(styles.painterData, m_Target.painter, typeof(NavMeshPainterData), false) as
                NavMeshPainterData;
        if (EditorGUI.EndChangeCheck())
            if (m_Target.painter != null)
                ResetCheckerBoardCellSize();

        m_Target.navMeshWireColor = EditorGUILayout.ColorField(styles.wireColor, m_Target.navMeshWireColor);
        m_PreviewMeshColor = EditorGUILayout.ColorField(styles.previewMeshColor, m_PreviewMeshColor);

        if (GUILayout.Button(styles.generateMesh))
        {
            RefreshPreviewMesh();
        }

        if (GUILayout.Button(styles.create))
        {
            CreateNewNavMeshPainterData();
        }
        if (GUILayout.Button(styles.bake))
        {
            Bake();
        }
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

    private void ResetCheckerBoardCellSize()
    {
        float minSize = m_Target.GetMinSize();
        NavMeshEditorUtils.SetCheckerBoardCellSize(minSize);
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
        if (RefreshPreviewMesh())
        {
            NavMeshBuilder.BuildNavMesh();
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

    private void CreateNewNavMeshPainterData()
    {
        NavMeshPainterCreator.CreateWizard(m_Target);
    }

    private bool RefreshPreviewMesh()
    {
        Mesh mesh = m_Target.GenerateMesh(m_PreviewMeshColor);
        if (mesh)
        {
            if (m_PreviewMeshObj == null)
            {
                m_PreviewMeshObj = new GameObject("PreviewMesh");
                m_PreviewMeshObj.transform.SetParent(m_Target.transform);
                m_PreviewMeshObj.hideFlags = HideFlags.HideAndDontSave;
                m_PreviewMeshObj.isStatic = true;
            }
            m_PreviewMeshObj.transform.position = Vector3.zero;
            m_PreviewMeshObj.transform.eulerAngles = Vector3.zero;
            MeshFilter mf = m_PreviewMeshObj.GetComponent<MeshFilter>();
            if (mf == null)
            {
                mf = m_PreviewMeshObj.AddComponent<MeshFilter>();
                mf.hideFlags = HideFlags.HideAndDontSave;
            }
            mf.sharedMesh = mesh;

            MeshRenderer mr = m_PreviewMeshObj.GetComponent<MeshRenderer>();
            if (mr == null)
            {
                mr = m_PreviewMeshObj.AddComponent<MeshRenderer>();
                mr.hideFlags = HideFlags.HideAndDontSave;
            }
            if (m_PreviewMaterial == null)
            {
                m_PreviewMaterial =
                    new Material((Shader) EditorGUIUtility.Load("NavMeshPainter/Shader/NavMeshRender.shader"));
                m_PreviewMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            mr.sharedMaterial = m_PreviewMaterial;
            return true;
        }
        return false;
    }
}
