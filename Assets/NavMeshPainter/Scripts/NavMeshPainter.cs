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

    public PaintingToolType paintTool;

    public Color navMeshWireColor = new Color(0, 1, 0, 0.5f);
    public Color previewColor = new Color(1, 0, 0, 0.5f);

    public NavMeshPainterData data;

    public NavMeshBrushTool brush;
    public NavMeshLineTool lineTool;
    public NavMeshBoxTool boxTool;
    public NavMeshCylinderTool cylinderTool;

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

    public void Init()
    {
        if (data)
            data.Init();
    }

    public void Save()
    {
        if (data)
            data.Save();
    }

    public float GetMinSize()
    {
        if (data != null)
            return data.GetMinSize();
        return 0;
    }

    public Mesh[] GetRenderMeshes()
    {
        if (data != null)
            return data.renderMeshs;
        return null;
    }

    public void Draw(IPaintingTool tool)
    {
        if (data != null)
            data.Paint(tool);
    }

    public void Erase(IPaintingTool tool)
    {
        if (data != null)
            data.Erase(tool);
    }

    public Mesh[] GenerateMeshes()
    {
        if (data != null)
            return data.GenerateMeshes(previewColor);
        return null;
    }

    public void Clear()
    {
        if (data != null)
            data.Clear();
    }

    public void SamplingFromTexture(Texture2D texture)
    {
        if (data != null)
            data.SamplingFromTexture(texture);
    }

    void OnDrawGizmosSelected()
    {
        if (data != null)
            data.DrawGizmos(navMeshWireColor);
    }
}
