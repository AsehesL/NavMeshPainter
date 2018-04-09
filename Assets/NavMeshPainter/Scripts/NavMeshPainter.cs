using UnityEngine;
using System.Collections;
using ASL.NavMeshPainter;

public enum PaintingToolType
{
    Brush,
    Line,
    Box,
    Sphere,
    Cylinder
}

public class NavMeshPainter : MonoBehaviour
{

    public PaintingToolType paintTool;

    public NavMeshPainterData painter;

    public NavMeshBrushTool brush;
    public NavMeshLineTool lineTool;
    public NavMeshBoxFillTool boxFillTool;
    public NavMeshCylinderFillTool cylinderFillTool;
    public NavMeshSphereFillTool sphereFillTool;

    public IPaintingTool GetPaintingTool()
    {
        switch (paintTool)
        {
            case PaintingToolType.Brush:
                return brush;
            case PaintingToolType.Box:
                return boxFillTool;
            case PaintingToolType.Cylinder:
                return cylinderFillTool;
            case PaintingToolType.Line:
                return lineTool;
            case PaintingToolType.Sphere:
                return sphereFillTool;
        }
        return null;
    }

    public void ResetState()
    {
        if (brush != null)
            brush.ResetState();
        if (lineTool != null)
            lineTool.ResetState();
        if (boxFillTool != null)
            boxFillTool.ResetState();
        if (cylinderFillTool != null)
            cylinderFillTool.ResetState();
        if (sphereFillTool != null)
            sphereFillTool.ResetState();
    }

    void OnDrawGizmosSelected()
    {
        if (painter != null)
            painter.DrawGizmos(Color.green);
    }
}
