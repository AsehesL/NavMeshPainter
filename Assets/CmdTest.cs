using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CmdTest : MonoBehaviour
{
    public Renderer testRenderer;

    private CommandBuffer m_CommandBuffer;

    private Camera m_Camera;

    private Material m_Material;

	void Start ()
	{
	    m_Material = new Material(Shader.Find("Unlit/CmdShader"));

        m_Camera = GetComponent<Camera>();

	    m_CommandBuffer = new CommandBuffer();

	    m_Camera.AddCommandBuffer(CameraEvent.AfterImageEffectsOpaque,
	        m_CommandBuffer);

#if UNITY_EDITOR
        Camera[] cam = UnityEditor.SceneView.GetAllSceneCameras();
        if (cam != null && cam.Length > 0)
        {
            foreach (var c in cam)
                c.AddCommandBuffer(CameraEvent.AfterImageEffectsOpaque,
           m_CommandBuffer);
        }
#endif
    }

    void OnDestroy()
    {
#if UNITY_EDITOR
        Camera[] cam = UnityEditor.SceneView.GetAllSceneCameras();
        if (cam != null && cam.Length > 0)
        {
            foreach(var c in cam)
                c.RemoveCommandBuffer(CameraEvent.AfterImageEffectsOpaque,
           m_CommandBuffer);
        }
#endif
        m_Camera.RemoveCommandBuffer(CameraEvent.AfterImageEffectsOpaque,
           m_CommandBuffer);
        m_CommandBuffer.Release();
        m_CommandBuffer = null;
    }

    void OnPostRender()
    {

        m_CommandBuffer.Clear();

        m_CommandBuffer.DrawRenderer(testRenderer, m_Material);
    }
}
