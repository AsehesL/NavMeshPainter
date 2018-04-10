using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ASL.NavMesh.Editor
{
    public class TerrainNavMeshProjector
    {
        private Projector m_Projector;

        private Terrain m_Terrain;

        public TerrainNavMeshProjector()
        {
        }

        public void SetTerrain(Terrain terrain)
        {
            m_Terrain = terrain;

            if (terrain == null)
            {
                if (m_Projector != null)
                    m_Projector.gameObject.SetActive(false);
            }
            else
            {
                if (m_Projector == null)
                    CreateProjector();
                else
                    m_Projector.gameObject.SetActive(true);

                m_Projector.transform.eulerAngles = new Vector3(90, 0, 0);

                m_Projector.orthographic = true;
                m_Projector.nearClipPlane = 0;

                RefreshProjector();
            }
        }

        public void SetMask(Texture2D texture)
        {
            if (m_Projector && m_Projector.material)
            {
                m_Projector.material.SetTexture("_RoadMask", texture);
            }
        }

        public void Update()
        {
            RefreshProjector();
        }

        public void Destroy()
        {
            if (m_Projector != null)
                Object.DestroyImmediate(m_Projector.gameObject);
        }

        private void RefreshProjector()
        {
            if (m_Projector == null)
                return;
            m_Projector.transform.position = m_Terrain.transform.position +
                                             new Vector3(m_Terrain.terrainData.size.x * 0.5f, m_Terrain.terrainData.size.y,
                                                 m_Terrain.terrainData.size.z * 0.5f);
            m_Projector.farClipPlane = m_Terrain.terrainData.size.y;
            m_Projector.aspectRatio = m_Terrain.terrainData.size.x / m_Terrain.terrainData.size.z;
            m_Projector.orthographicSize = m_Terrain.terrainData.size.z * 0.5f;
        }

        private void CreateProjector()
        {
            m_Projector = new GameObject("[TerrainProjector]").AddComponent<Projector>();
            m_Projector.gameObject.hideFlags = HideFlags.HideAndDontSave;
            m_Projector.material =
                new Material((Shader) EditorGUIUtility.Load("NavMeshPainter/Shader/NavMeshProjector.shader"));
        }
    }
}