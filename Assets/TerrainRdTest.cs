using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainRdTest : MonoBehaviour
{

    public Terrain terrain;

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Vector3 p0 = terrain.transform.position;
        Vector3 p1 = terrain.transform.position + new Vector3(terrain.terrainData.size.x, 0, 0);
        Vector3 p2 = terrain.transform.position + new Vector3(terrain.terrainData.size.x, 0, terrain.terrainData.size.z);
        Vector3 p3 = terrain.transform.position + new Vector3(0, 0, terrain.terrainData.size.z);

        Vector3 max = Vector3.Max(p0, Vector3.Max(p1, Vector3.Max(p2, p3)));
        Vector3 min = Vector3.Min(p0, Vector3.Min(p1, Vector3.Min(p2, p3)));

        Ray rayLeftTop = Camera.main.ScreenPointToRay(new Vector2(0, 0));
        Ray rayLeftBottom =
            Camera.main.ScreenPointToRay(new Vector2(0, Screen.height));
        Ray rayRightTop =
            Camera.main.ScreenPointToRay(new Vector2(Screen.width, 0));
        Ray rayRightBottom =
            Camera.main.ScreenPointToRay(new Vector2(Screen.width,
                Screen.height));

        bool raycast = false;

        Vector3 rmax = new Vector3(-Mathf.Infinity, -Mathf.Infinity, -Mathf.Infinity);
        Vector3 rmin = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

        Vector3 hitPoint;
        if (Raycast(terrain.transform.position, Vector3.up, rayLeftTop, out hitPoint))
        {
            raycast = true;
            rmax = Vector3.Max(rmax, hitPoint);
            rmin = Vector3.Min(rmin, hitPoint);
        }
        if (Raycast(terrain.transform.position, Vector3.up, rayLeftBottom, out hitPoint))
        {
            raycast = true;
            rmax = Vector3.Max(rmax, hitPoint);
            rmin = Vector3.Min(rmin, hitPoint);
        }
        if (Raycast(terrain.transform.position, Vector3.up, rayRightTop, out hitPoint))
        {
            raycast = true;
            rmax = Vector3.Max(rmax, hitPoint);
            rmin = Vector3.Min(rmin, hitPoint);
        }
        if (Raycast(terrain.transform.position, Vector3.up, rayRightBottom, out hitPoint))
        {
            raycast = true;
            rmax = Vector3.Max(rmax, hitPoint);
            rmin = Vector3.Min(rmin, hitPoint);
        }

        if (!raycast)
            return;

        rmax = Clamp(rmax, min, max);
        rmin = Clamp(rmin, min, max);

        Vector3 pd0 = rmin;
        Vector3 pd1 = new Vector3(rmin.x, rmin.y, rmax.z);
        Vector3 pd2 = new Vector3(rmax.x, rmin.y, rmax.z);
        Vector3 pd3 = new Vector3(rmax.x, rmin.y, rmin.z);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(pd0, pd1);
        Gizmos.DrawLine(pd1, pd2);
        Gizmos.DrawLine(pd2, pd3);
        Gizmos.DrawLine(pd3, pd0);
    }

    private bool Raycast(Vector3 planePos, Vector3 planeNormal, Ray ray, out Vector3 hitPoint)
    {
        float distance = 0;
        hitPoint = Vector3.zero;
        float ndotd = Vector3.Dot(planeNormal, ray.direction);
        if (ndotd >= 0)
            return false;
        distance = (Vector3.Dot(planePos, planeNormal) - Vector3.Dot(ray.origin, planeNormal))/ndotd;
        if (distance >= 0)
        {
            hitPoint = ray.origin + ray.direction*distance;
            return true;
        }
        return false;
    }

    private Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
    {
        value.x = Mathf.Clamp(value.x, min.x, max.x);
        value.y = Mathf.Clamp(value.y, min.y, max.y);
        value.z = Mathf.Clamp(value.z, min.z, max.z);
        return value;
    }
}
