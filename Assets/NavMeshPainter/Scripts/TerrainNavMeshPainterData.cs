using UnityEngine;
using System.Collections;

namespace ASL.NavMesh
{
    public class TerrainNavMeshPainterData : ScriptableObject
    {
        public TerrainData terrainData;

        public Texture2D navMeshMask;

        public int maxDepth { get { return m_MaxDepth; } }

        [SerializeField] private int m_MaxDepth;
    }
}