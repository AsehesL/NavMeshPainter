using UnityEngine;
using System.Collections;

namespace ASL.NavMeshPainter.Editor
{
    public class NavMeshToolEditor
    {
        protected IPaintingTool target;

        public void SetTool(IPaintingTool tool)
        {
            target = tool;
        }

        public virtual void OnGUI() { }

        public virtual void OnSceneGUI(Material renderMaterial) { }
    }
}