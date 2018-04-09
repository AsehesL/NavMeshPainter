using UnityEngine;
using System.Collections;
using ASL.NavMesh;

public enum PaintingToolType
{
    Brush,
    Line,
}

public class NavMeshPainter : MonoBehaviour
{

    public PaintingToolType paintTool;

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

    public Mesh GetRenderMesh()
    {
        if (painter != null)
            return painter.renderMesh;
        return null;
    }

    public void Draw(IPaintingTool tool)
    {
        if (painter != null)
            painter.Draw(tool);
    }

    public void Erase(IPaintingTool tool)
    {
        if (painter != null)
            painter.Erase(tool);
    }

    public Mesh GenerateMesh()
    {
        if (painter != null)
            return painter.GenerateMesh();
        return null;
    }

    void OnDrawGizmosSelected()
    {
        if (painter != null)
            painter.DrawGizmos(Color.green);
    }
}
