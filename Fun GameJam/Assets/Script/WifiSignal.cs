using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WifiSignal : MonoBehaviour
{
    [Header("Speeds")]
    public Vector2 mMinMaxSpeed = new Vector2(5f, 10f);
    public AnimationCurve mSpeedChanceCurve = new AnimationCurve();
    [Header("Speed duration")]
    public Vector2 mMinMaxSpeedChangeInterval = new Vector2(2f, 5f);
    public AnimationCurve mSpeedIntervalChanceCurve = new AnimationCurve();
    [Header("Speed transition")]
    public Vector2 mMinMaxTransitionSpeedTime = new Vector2(0.5f, 1f);
    public AnimationCurve mSpeedTransitionChanceCurve = new AnimationCurve();
    [Header("Stuff")]
    public float mTriggerDistance = 1f;


    private WifiPoint mOriginPoint;
    private WifiPoint mWifiPointTarget;
    private bool mFreeze = true;
    private float mCurrentSpeed = 0f;
    private float mCurrentInterval = 0f;
    private Vector3 mCurrentDirection = Vector3.one;

    private float _CurrentDistance => Vector3.Distance(transform.position, mWifiPointTarget.transform.position);
    private void Awake()
    {
        GameManager.OnGameReady += Initialize;
        GameManager.OnDisconection += delegate { mFreeze = true; mCurrentSpeed = 0; CancelInvoke(nameof(SwitchSpeed)); };
        GameManager.OnReconnection += delegate { mFreeze = false; SwitchSpeed(); };
    }

    private void Initialize()
    {
        mFreeze = false;
        transform.position = WifiManager.sClosestWifiPoint.transform.position;
        mOriginPoint = WifiManager.sClosestWifiPoint;
        mWifiPointTarget = mOriginPoint.GetRandomNextWifiPoint(mOriginPoint);
        mCurrentDirection = (mWifiPointTarget.transform.position - mOriginPoint.transform.position).normalized;
        SwitchSpeed();
    }

    private void Update()
    {
        if (!mFreeze)
        {
            transform.position += mCurrentDirection * mCurrentSpeed * Time.deltaTime;
            if (_CurrentDistance < mTriggerDistance)
            {
                WifiPoint intermediary = mWifiPointTarget;
                mWifiPointTarget = mWifiPointTarget.GetRandomNextWifiPoint(mOriginPoint);
                mOriginPoint = intermediary;
                mCurrentDirection = (mWifiPointTarget.transform.position - mOriginPoint.transform.position).normalized;
            }
        }
    }
    private void SwitchSpeed() { StartCoroutine(SwitchSpeedCoroutine()); }

    private IEnumerator SwitchSpeedCoroutine()
    {
        float newSpeed = mMinMaxSpeed.Lerp(mSpeedChanceCurve.Evaluate(Random.value));
        float nextInterval = mMinMaxSpeedChangeInterval.Lerp(mSpeedIntervalChanceCurve.Evaluate(Random.value));
        float transitionTime = mMinMaxTransitionSpeedTime.Lerp(mSpeedTransitionChanceCurve.Evaluate(Random.value));
        float currentSpeed = mCurrentSpeed;
        Debug.LogFormat("[WifiSignal] New sitch speed: target speed {0}, target interval {1}, target transition {2}", newSpeed, nextInterval, transitionTime);
        float currentTime = 0;

        while (currentTime < transitionTime)
        {
            float progression = currentTime / transitionTime;
            mCurrentSpeed = Mathf.Lerp(currentSpeed, newSpeed, progression);
            currentTime += Time.deltaTime;
            yield return null;
        }
        mCurrentSpeed = newSpeed;
        mCurrentInterval = nextInterval;
        Invoke(nameof(SwitchSpeed), nextInterval);
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 1.5f);
    }
}
