using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalManager : MonoBehaviour
{
    public Transform mAntennaTransform;
    [Range(0f,20f)]public float mRotationAntennaSpeed = 10f;

    private Quaternion mPreviousRotation;
    private void LateUpdate()
    {
            mAntennaTransform.rotation = mPreviousRotation;
        Vector2 direction = new Vector2(Input.GetAxis("AntennaX"), Input.GetAxis("AntennaY"));

        //Antenna rotation
        if (direction.magnitude>0)
        {
            direction = direction.LimitMagnitude(1f);
            Vector3 worldDirection = (direction.x * CameraManager.RightDirection + direction.y * CameraManager.ForwardDirection).normalized;
            mAntennaTransform.rotation = Quaternion.Lerp(mPreviousRotation,
                    Quaternion.LookRotation(worldDirection), mRotationAntennaSpeed * Time.deltaTime);
            mPreviousRotation = mAntennaTransform.rotation; 
        }
    }
}
