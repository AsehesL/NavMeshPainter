using UnityEngine;
using System.Collections;
using ASL.NavMesh;


/// <summary>
/// 导航网格绘制器
/// </summary>
public class NavMeshPainter : MonoBehaviour
{
    /// <summary>
    /// 绘制工具
    /// </summary>
    public enum PaintingToolType
    {
        /// <summary>
        /// 笔刷
        /// </summary>
        Brush,
        /// <summary>
        /// 画线
        /// </summary>
        Line,

        Box,

        Cylinder,
    }

    public NavMeshOcTree data
    {
        get { return m_Tree; }
    }

    /// <summary>
    /// 用于渲染的mesh
    /// </summary>
    public Mesh[] renderMeshs;

    public PaintingToolType paintTool;

    public Color navMeshWireColor = new Color(0, 1, 0, 0.5f);
    public Color previewColor = new Color(1, 0, 0, 0.5f);

    public string dataPath;

    public NavMeshBrushTool brush;
    public NavMeshLineTool lineTool;
    public NavMeshBoxTool boxTool;
    public NavMeshCylinderTool cylinderTool;
    public float lodDeltaDis = 20f;

    private NavMeshOcTree m_Tree;

    public IPaintingTool GetPaintingTool()
    {
        switch (paintTool)
        {
            case PaintingToolType.Brush:
                return brush;
            case PaintingToolType.Line:
                return lineTool;
            case PaintingToolType.Box:
                return boxTool;
            case PaintingToolType.Cylinder:
                return cylinderTool;
        }
        return null;
    }

    public void Load()
    {
        if (m_Tree == null && !string.IsNullOrEmpty(dataPath))
        {
            if (System.IO.File.Exists(dataPath))
            {
                m_Tree = NavMeshOcTree.Load(dataPath);

                renderMeshs = m_Tree.GenerateRenderMesh();
            }
            else
            {
                m_Tree = null;
                renderMeshs = null;
            }
        }
        else
        {
            if (!System.IO.File.Exists(dataPath))
            {
                m_Tree = null;
                dataPath = null;
                renderMeshs = null;
            }
        }
    }

    public void Reload(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            dataPath = path;
            m_Tree = NavMeshOcTree.Load(dataPath);

            renderMeshs = m_Tree.GenerateRenderMesh();
        }
        else
        {
            dataPath = null;
            m_Tree = null;
        }
    }

    public void Save()
    {
        if (m_Tree != null && !string.IsNullOrEmpty(dataPath))
            NavMeshOcTree.Save(m_Tree, dataPath);
    }

    public float GetMinSize()
    {
        if (m_Tree != null)
            return m_Tree.GetMinSize();
        return 0;
    }

    public void Draw(IPaintingTool tool)
    {
        if (m_Tree != null)
            m_Tree.Interesect(tool);
    }

    public void Erase(IPaintingTool tool)
    {
        if (m_Tree != null)
            m_Tree.Interesect(tool, true);
    }

    public Mesh[] GenerateMeshes()
    {
        if (m_Tree != null)
            return m_Tree.GenerateMeshes(previewColor);
        return null;
    }

    public void Clear()
    {
        if (m_Tree != null)
            m_Tree.Clear();
    }

    public void SamplingFromTexture(Texture2D texture)
    {
        if (m_Tree != null)
            m_Tree.SamplingFromTexture(texture);
    }

}
