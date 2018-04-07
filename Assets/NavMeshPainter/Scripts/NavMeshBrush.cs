using UnityEngine;
using System.Collections;

namespace ASL.NavMeshPainter
{
    [System.Serializable]
    public class NavMeshBrush
    {

        public float size;
        public Vector3 position;
        public float maxHeight;

        public Bounds bounds
        {
            get { return new Bounds(position, new Vector3(size*2, maxHeight*2, size*2)); }
        }

        public void DrawGizmos(Color color)
        {
            bounds.DrawBounds(color);
        }
    }
}