using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ASL.NavMesh.Editor
{
    [CustomEditor(typeof(NavMeshPainterData))]
    public class NavMeshPainterDataEditor : UnityEditor.Editor
    {
        private NavMeshPainterData m_Target;

        private class Styles
        {
            public GUIStyle boldLabel = "BoldLabel";

            public GUIContent refresh = new GUIContent("Refresh NavMeshPainterData");
            public GUIContent bounds = new GUIContent("Bounds");
            public GUIContent count = new GUIContent("Triangle Count");
            public GUIContent info = new GUIContent("Information");
            public GUIContent check = new GUIContent("Check Max Triangle Node Depth");
        }

        private static Styles styles
        {
            get
            {
                if (s_Styles == null)
                    s_Styles = new Styles();
                return s_Styles;
            }
        }

        private static Styles s_Styles;

        void OnEnable()
        {
            m_Target = (NavMeshPainterData) target;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Label(styles.info, styles.boldLabel);

            EditorGUILayout.BoundsField(styles.bounds, m_Target.ocTree.bounds);

            EditorGUILayout.FloatField(styles.count, m_Target.ocTree.count);

            if (GUILayout.Button(styles.check))
            {
                CheckMaxTriangleNodeCount();
            }

            if (GUILayout.Button(styles.refresh))
            {
                NavMeshPainterCreator.CreateWizard(null, m_Target);
            }
        }

        private void CheckMaxTriangleNodeCount()
        {
            m_Target.CheckMaxTriangleNodeCount();
        }
    }
}