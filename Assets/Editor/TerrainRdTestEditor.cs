//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//
//[CustomEditor(typeof(TerrainRdTest))]
//public class TerrainRdTestEditor : Editor
//{
//
//    TerrainRdTest m_Target;
//
//    void OnEnable()
//    {
//        m_Target = (TerrainRdTest) target;
//
//        if (m_Target.terrain != null)
//        {
//            Debug.Log(m_Target.terrain.terrainData.size);
//        }
//    }
//
//    void OnSceneGUI()
//    {
//        if (m_Target.terrain == null)
//            return;
//        Handles.color = Color.red;
//
//        Vector3 p0 = m_Target.terrain.transform.position;
//        Vector3 p1 = m_Target.terrain.transform.position + new Vector3(m_Target.terrain.terrainData.size.x, 0, 0);
//        Vector3 p2 = m_Target.terrain.transform.position + new Vector3(m_Target.terrain.terrainData.size.x, 0, m_Target.terrain.terrainData.size.z);
//        Vector3 p3 = m_Target.terrain.transform.position + new Vector3(0, 0, m_Target.terrain.terrainData.size.z);
//
//        Vector3 max = Vector3.Max(p0, Vector3.Max(p1, Vector3.Max(p2, p3)));
//        Vector3 min = Vector3.Min(p0, Vector3.Min(p1, Vector3.Min(p2, p3)));
//
//        Ray rayLeftTop = HandleUtility.GUIPointToWorldRay(new Vector2(0, 0));
//        Ray rayLeftBottom = HandleUtility.GUIPointToWorldRay(new Vector2(0, SceneView.currentDrawingSceneView.position.height));
//        Ray rayRightTop = HandleUtility.GUIPointToWorldRay(new Vector2(SceneView.currentDrawingSceneView.position.width, 0));
//        Ray rayRightBottom = HandleUtility.GUIPointToWorldRay(new Vector2(SceneView.currentDrawingSceneView.position.width, SceneView.currentDrawingSceneView.position.height));
//
//        bool raycast = false;
//
//        Vector3 rmax = new Vector3(-Mathf.Infinity, -Mathf.Infinity, -Mathf.Infinity);
//        Vector3 rmin = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
//
//        Vector3 hitPoint;
//        if (Raycast(m_Target.terrain.transform.position, Vector3.up, rayLeftTop, out hitPoint))
//        {
//            raycast = true;
//            rmax = Vector3.Max(rmax, hitPoint);
//            rmin = Vector3.Min(rmin, hitPoint);
//        }
//
//
//        Handles.DrawLine(p0, p1);
//        Handles.DrawLine(p1, p2);
//        Handles.DrawLine(p2, p3);
//        Handles.DrawLine(p3, p0);
//    }
//
//    private bool Raycast(Vector3 planePos, Vector3 planeNormal, Ray ray, out Vector3 hitPoint)
//    {
//        float distance = 0;
//        hitPoint = Vector3.zero;
//        float ndotd = Vector3.Dot(planeNormal, ray.direction);
//        if (ndotd >= 0)
//            return false;
//        distance = (Vector3.Dot(planePos, planeNormal) - Vector3.Dot(ray.origin, planeNormal))/ndotd;
//        if (distance >= 0)
//        {
//            hitPoint = ray.origin + ray.direction*distance;
//            return true;
//        }
//        return false;
//    }
//
//    private Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
//    {
//        value.x = Mathf.Clamp(value.x, min.x, max.x);
//        value.y = Mathf.Clamp(value.y, min.y, max.y);
//        value.z = Mathf.Clamp(value.z, min.z, max.z);
//        return value;
//    }
//}
