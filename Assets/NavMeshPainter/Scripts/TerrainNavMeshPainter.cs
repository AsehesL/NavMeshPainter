using UnityEngine;
using System.Collections;
using ASL.NavMesh;

public class TerrainNavMeshPainter : MonoBehaviour
{

    public Terrain targetTerrain;

    public NavMeshTextureBrushTool brush;

    public Texture2D maskTexture;

    public int meshWidth;

    public int meshHeight;

    public int maxDepth;
}
