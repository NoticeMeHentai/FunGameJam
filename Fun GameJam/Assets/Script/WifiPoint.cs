using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WifiPoint : MonoBehaviour
{
    public List<WifiPoint> mWifiPoints = new List<WifiPoint>();
    //Editor utilites
    public float mRadius = 3f;
    [HideInInspector]public Color mColorToGizmos = Color.blue;
    [HideInInspector] public float mIDMask = 0f;
    [HideInInspector] public int mIDCounter = 0;



    private SphereCollider mSphereCollider;
    private SphereCollider _Collider { get { if (mSphereCollider == null) mSphereCollider = GetComponentInChildren<SphereCollider>(); return mSphereCollider; } }

    /// <summary>
    /// Returns a random point among the next points available
    /// </summary>
    /// <returns></returns>
    public WifiPoint GetRandomNextWifiPoint(WifiPoint origin)
    {
        if (mWifiPoints.Count > 1)
        {
            bool mHasFoundNewPoint = false;
            while (!mHasFoundNewPoint)
            {
                int randomIndex = Random.Range(0, mWifiPoints.Count);
                if (mWifiPoints[randomIndex] != origin)
                    return mWifiPoints[randomIndex];
            }
        }
        return origin;
            
    }

    public void Initialize(int IDCounter, float IDmask, List<WifiPoint> reference, float radius = 3f)
    {
        mIDCounter = IDCounter;
        mIDMask = IDmask;

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
        Gizmos.DrawWireSphere(transform.position, WifiManager.sRadius);

        Gizmos.color = Color.red;
        for(int i = 0; i < mWifiPoints.Count; i++)
        {
            Gizmos.DrawLine(transform.position, mWifiPoints[i].transform.position);
        }
    }
}
