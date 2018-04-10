using UnityEngine;
using UnityEditor;
using System.Collections;

public class NavMeshPainterEditorBase : Editor
{

    protected enum ToolType
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
        public GUIContent blendMode = new GUIContent("BlendMode");
    }

    public static Styles styles
    {
        get
        {
            if (s_Styles == null)
                s_Styles = new Styles();
            return s_Styles;
        }
    }

    private static Styles s_Styles;

    protected ToolType toolType = ToolType.None;

    protected bool showCheckerBoard
    {
        get { return m_ShowCheckerBoard; }
    }

    private bool m_ShowCheckerBoard = true;

    protected void DrawToolsGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUI.BeginChangeCheck();
        m_ShowCheckerBoard = GUILayout.Toggle(showCheckerBoard, styles.checkerboardIcon, styles.command);
        if (EditorGUI.EndChangeCheck())
        {
            ResetCheckerBoard();
        }

        EditorGUILayout.Space();

        int selectedTool = (int)this.toolType;
        int num = GUILayout.Toolbar(selectedTool, styles.toolIcons, styles.command, new GUILayoutOption[0]);
        if (num != selectedTool)
        {
            this.toolType = (ToolType)num;
        }
        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();
    }

    protected virtual void ResetCheckerBoard() { }
}
