using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TerrainNavMeshPainter))]
public class TerrainNavMeshPainterEditor : Editor
{
    private TerrainNavMeshPainter m_Target;

    void OnEnable()
    {
        m_Target = (TerrainNavMeshPainter) target;
    }
}
