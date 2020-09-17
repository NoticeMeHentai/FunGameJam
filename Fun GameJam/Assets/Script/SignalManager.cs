using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalManager : MonoBehaviour
{
    [Header("Antenna")]
    public Transform mAntennaTransform;
    [Range(0f,20f)]public float mRotationAntennaSpeed = 10f;

    [Header("Scanner")]
    public Transform mScannerPivot;
    public float mRefreshTimer = 0.5f;
    public float mScannerDuraction = 0.25f;
    public Material mScannerMaterial;
    

    private Quaternion mPreviousAntennaRotation;
    private Quaternion mScannerRotation;
    private bool mIsScanning = false;
    private void LateUpdate()
    {
            mAntennaTransform.rotation = mPreviousAntennaRotation;
        Vector2 direction = new Vector2(Input.GetAxis("AntennaX"), Input.GetAxis("AntennaY"));

        //Antenna rotation
        if (direction.magnitude>0)
        {
            direction = direction.LimitMagnitude(1f);
            Vector3 worldDirection = (direction.x * CameraManager.RightDirection + direction.y * CameraManager.ForwardDirection).normalized;
            mAntennaTransform.rotation = Quaternion.Lerp(mPreviousAntennaRotation,
                    Quaternion.LookRotation(worldDirection), mRotationAntennaSpeed * Time.deltaTime);
            mPreviousAntennaRotation = mAntennaTransform.rotation; 
        }
    }

    //private IEnumerator ScanCoroutine()
    //{
    //    float currentTime = 0;
    //    mIsScanning = true;
    //    mScannerRotation = mAntennaTransform.rotation;
    //
    //    //while()
    //}
}
