using UnityEngine;
using System.Collections;
using ASL.NavMesh;


/// <summary>
/// 导航网格绘制器
/// </summary>
public class NavMeshPainter : MonoBehaviour
{

    public PaintingToolType paintTool;

    public Color navMeshWireColor = Color.green;

    public NavMeshPainterData painter;

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

    public float GetMinSize()
    {
        if (painter != null)
            return painter.GetMinSize();
        return 0;
    }

    public Mesh GetRenderMesh()
    {
        if (painter != null)
            return painter.renderMesh;
        return null;
    }

    public void Draw(IPaintingTool tool)
    {
        if (painter != null)
            painter.Paint(tool);
    }

    public void Erase(IPaintingTool tool)
    {
        if (painter != null)
            painter.Erase(tool);
    }

    public Mesh GenerateMesh(Color color)
    {
        if (painter != null)
            return painter.GenerateMesh(color);
        return null;
    }

    public void SamplingFromTexture(Texture2D texture, TextureBlendMode blendMode)
    {
        if (painter != null)
            painter.SamplingFromTexture(texture, blendMode);
    }

    void OnDrawGizmosSelected()
    {
        if (painter != null)
            painter.DrawGizmos(navMeshWireColor);
    }
}
