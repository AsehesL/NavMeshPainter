using UnityEngine;
using System.Collections;
using ASL.NavMeshPainter;

public enum PaintToolType
{
    Brush,
    Line,
    Box,
    Sphere,
    Cylinder
}

public class NavMeshPainter : MonoBehaviour
{

    public PaintToolType paintTool;

    public NavMeshPainterData painter;

    public NavMeshBrush brush;

    public IPaintingTool GetPaintingTool()
    {
        switch (paintTool)
        {
            case PaintToolType.Brush:
                return brush;

        }
        return null;
    }

    void OnDrawGizmosSelected()
    {
        if (brush != null)
            brush.DrawToolGizmos();
        if (painter != null)
            painter.DrawGizmos(Color.green);
    }
}
