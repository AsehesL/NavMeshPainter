using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using ASL.NavMesh;
using ASL.NavMesh.Editor;

[CustomEditor(typeof(TerrainNavMeshPainter))]
public class TerrainNavMeshPainterEditor : NavMeshPainterEditorBase
{
   

    private TerrainNavMeshPainter m_Target;

    private TerrainNavMeshProjector m_Projector;

    [MenuItem("GameObject/NavMeshPainter/Create TerrainNavMeshPainter")]
    static void Create()
    {
        TerrainNavMeshPainter painter = new GameObject("TerrainNavMesh Painter").AddComponent<TerrainNavMeshPainter>();
    }

    void OnEnable()
    {
        m_Target = (TerrainNavMeshPainter) target;
        
        m_Projector = new TerrainNavMeshProjector();

        m_Projector.SetTerrain(m_Target.targetTerrain);

        m_Projector.SetMask(m_Target.maskTexture);
    }

    void OnDisable()
    {
        if (m_Projector != null)
            m_Projector.Destroy();
    }

    void OnSceneGUI()
    {
        if (m_Projector != null)
            m_Projector.Update();
    }

    public override void OnInspectorGUI()
    {
        //        base.OnInspectorGUI();
        //
        //        if (GUILayout.Button("Bake"))
        //        {
        //            Bake();
        //        }
        if (m_Target.targetTerrain == null || m_Target.maskTexture == null)
        {
            DrawNoDataGUI();
            return;
        }
        DrawToolsGUI();
        switch (toolType)
        {
            case ToolType.None:

                break;
            case ToolType.Paint:
                //DrawPaintSettingGUI();
                break;
            case ToolType.Erase:
                //DrawPaintSettingGUI(true);
                break;
            case ToolType.Mapping:
                //DrawTextureMappingGUI();
                break;
            case ToolType.Bake:
                //DrawBakeSettingGUI();
                break;
        }

        EditorGUILayout.Space();
    }

    private void DrawNoDataGUI()
    {
        EditorGUI.BeginChangeCheck();
        m_Target.targetTerrain =
            EditorGUILayout.ObjectField(styles.painterData, m_Target.targetTerrain, typeof(Terrain), false) as
                Terrain;
        if (EditorGUI.EndChangeCheck())
        {
            m_Projector.SetTerrain(m_Target.targetTerrain);
        }

        EditorGUI.BeginChangeCheck();
        m_Target.maskTexture =
            EditorGUILayout.ObjectField(styles.painterData, m_Target.maskTexture, typeof (Texture2D), false) as
                Texture2D;
        if (EditorGUI.EndChangeCheck())
        {
            m_Projector.SetMask(m_Target.maskTexture);
        }

        if (GUILayout.Button(styles.create))
        {
            //CreateNewNavMeshPainterData();
        }
        EditorGUILayout.HelpBox("No PainterData has been setted!", MessageType.Error);
    }

    protected override void ResetCheckerBoard()
    {
        base.ResetCheckerBoard();
    }

    private void Bake()
    {
        //List<NavMeshTriangle> triangleList = new List<NavMeshTriangle>();

        List<Vector3> vlist = new List<Vector3>();
        List<int> ilist = new List<int>();

        for (int i = 0; i < m_Target.meshHeight; i++)
        {
            for (int j = 0; j < m_Target.meshWidth; j++)
            {
                float u0 = ((float)j) / m_Target.meshWidth;
                float u1 = ((float)(j + 1)) / m_Target.meshWidth;

                float v0 = ((float)i) / m_Target.meshHeight;
                float v1 = ((float)(i + 1)) / m_Target.meshHeight;

                //float rx0 = u0 * m_Target.targetTerrain.terrainData.size.x;
                //float rx1 = u1 * m_Target.targetTerrain.terrainData.size.x;
                //float ry0 = v0 * m_Target.targetTerrain.terrainData.size.z;
                //float ry1 = v1 * m_Target.targetTerrain.terrainData.size.z;

                //float h00 = m_Target.targetTerrain.terrainData.GetInterpolatedHeight(u0, v0);
                //float h01 = m_Target.targetTerrain.terrainData.GetInterpolatedHeight(u0, v1);
                //float h11 = m_Target.targetTerrain.terrainData.GetInterpolatedHeight(u1, v1);
                //float h10 = m_Target.targetTerrain.terrainData.GetInterpolatedHeight(u1, v0);

                //Vector3 vertex0 = new Vector3(rx0, h00, ry0);
                //Vector3 vertex1 = new Vector3(rx0, h01, ry1);
                //Vector3 vertex2 = new Vector3(rx1, h11, ry1);
                //Vector3 vertex3 = new Vector3(rx1, h10, ry0);

                Vector2 uv0 = new Vector2(u0, v0);
                Vector2 uv1 = new Vector2(u0, v1);
                Vector2 uv2 = new Vector2(u1, v1);
                Vector2 uv3 = new Vector2(u1, v0);

                var triangle0 = new NavMeshTriangle(uv0, uv1, uv2);
                var triangle1 = new NavMeshTriangle(uv0, uv2, uv3);

                triangle0.Subdivide(m_Target.maxDepth);
                triangle1.Subdivide(m_Target.maxDepth);

                triangle0.SamplingFromTexture(m_Target.maskTexture, TextureBlendMode.Replace);
                triangle1.SamplingFromTexture(m_Target.maskTexture, TextureBlendMode.Replace);

                triangle0.GenerateMesh(vlist, ilist, m_Target.targetTerrain);
                triangle1.GenerateMesh(vlist, ilist, m_Target.targetTerrain);

                //triangleList.Add(triangle0);
                //triangleList.Add(triangle1);
            }
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(vlist);
        mesh.SetTriangles(ilist, 0);

        GameObject go = new GameObject("Test");
        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();

        mf.sharedMesh = mesh;
        mr.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
        mr.sharedMaterial.SetColor("_Color", Color.red);
    }
}
