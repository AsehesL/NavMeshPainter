using UnityEngine;
using System.Collections;
using ASL.NavMeshPainter;

public class NavMeshPainter : MonoBehaviour
{

    public NavMeshPainterData painter;

    public NavMeshBrush brush;

    void OnDrawGizmosSelected()
    {
        if (brush != null)
            brush.DrawGizmos(Color.blue);
        if (painter != null)
            painter.DrawGizmos(Color.green);
    }
}
