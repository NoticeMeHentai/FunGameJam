using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WifiPoint : MonoBehaviour
{
    public List<WifiPoint> mWifiPoints = new List<WifiPoint>();
    //Editor utilites
    public float mRadius = 3f;
    [HideInInspector]public Color mColorToGizmos = Color.blue;



    private SphereCollider mSphereCollider;
    private SphereCollider _Collider { get { if (mSphereCollider == null) mSphereCollider = GetComponentInChildren<SphereCollider>(); return mSphereCollider; } }


    /// <summary>
    /// Returns a random point among the next points available
    /// </summary>
    /// <returns></returns>
    public WifiPoint GetRandomNextWifiPoint()
    {
        if (mWifiPoints.Count > 0)
            return mWifiPoints[Random.Range(0, mWifiPoints.Count)];
        return this;
    }
    

    public void RemoveWifiPoint(WifiPoint wifiPoint)
    {
        if (mWifiPoints.Contains(wifiPoint))
            mWifiPoints.Remove(wifiPoint);
    }

    private void OnDrawGizmos()
    {
        _Collider.radius = mRadius;
        Gizmos.color = mColorToGizmos;
        Gizmos.DrawWireSphere(transform.position, mRadius);

        Gizmos.color = Color.red;
        for(int i = 0; i < mWifiPoints.Count; i++)
        {
            Gizmos.DrawLine(transform.position, mWifiPoints[i].transform.position);
        }
    }
}
