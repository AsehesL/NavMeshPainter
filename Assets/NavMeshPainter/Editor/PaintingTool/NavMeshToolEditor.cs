using UnityEngine;
using System.Collections;

namespace ASL.NavMesh.Editor
{
    public class NavMeshToolEditor
    {
        protected IPaintingTool target;

        private System.Action<IPaintingTool> m_ApplyAction;

        public void SetApplyAction(System.Action<IPaintingTool> applyAction)
        {
            m_ApplyAction = applyAction;
        }

        public void SetTool(IPaintingTool tool)
        {
            target = tool;
        }

        public virtual void DrawGUI() { }

        public void DrawSceneGUI(NavMeshPainter targetPainter)
        {
            RaycastHit hit;
            if (NavMeshEditorUtils.RayCastInSceneView(targetPainter.GetRenderMesh(), out hit))
            {
                OnRaycast(targetPainter, hit);
            }
            OnSceneGUI(targetPainter);
        }

        protected virtual void OnSceneGUI(NavMeshPainter targetPainter)
        {
            
        }

        protected virtual void OnRaycast(NavMeshPainter targetPainter, RaycastHit hit)
        {
            
        }

        protected void ApplyPaint()
        {
            if (m_ApplyAction != null && target != null)
                m_ApplyAction(target);
        }
    }
}