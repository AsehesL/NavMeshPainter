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
    }

    public PaintingToolType paintTool;

    public Color navMeshWireColor = Color.green;

    public NavMeshPainterData data;

    public NavMeshBrushTool brush;
    public NavMeshLineTool lineTool;

    public IPaintingTool GetPaintingTool()
    {
        switch (paintTool)
        {
            case PaintingToolType.Brush:
                return brush;
            case PaintingToolType.Line:
                return lineTool;
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

    public Mesh GetRenderMesh()
    {
        if (data != null)
            return data.renderMesh;
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

    public Mesh GenerateMesh(Color color)
    {
        if (data != null)
            return data.GenerateMesh(color);
        return null;
    }

    public void SamplingFromTexture(Texture2D texture, TextureBlendMode blendMode)
    {
        if (data != null)
            data.SamplingFromTexture(texture, blendMode);
    }

    void OnDrawGizmosSelected()
    {
        if (data != null)
            data.DrawGizmos(navMeshWireColor);
    }
}
