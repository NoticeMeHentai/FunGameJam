using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif
public class SignalScanner : MonoBehaviour
{
    [Header("Antenna")]
    public Transform mAntennaTransform;
    [Range(0f,20f)]public float mRotationAntennaSpeed = 10f;

    [Header("Scanner")]
    public Transform mScannerPivot;
    public float mRefreshTimer = 0.5f;
    public float mScannerDuration = 0.25f;
    public Material mScannerMaterial;
    [Header("Scanner gameplay")]
    public Gradient mScannerColorGradient;
    [Tooltip("The minimal and maximal connection distance. Over max, you lose the signal, within min, it's at max.")]
    public Vector2 mMinMaxConnectionDistance = new Vector2(5f, 30f);
    [Tooltip("The ratio distance at which we have more wifi or not (shown with the wifi bars during scanner).")]
    public AnimationCurve mDistanceSignalCurve = new AnimationCurve();
    [Tooltip("The minimal and maximal angles at which the antenna will get more or less wifi.")]
    public Vector2 mMinMaxSignalAngles = new Vector2(10, 90);
    [Tooltip("The amount of signal we get depending on the angle of the antenna and the position of the signal.")]
    public AnimationCurve mAimingSignalCurve = new AnimationCurve();
    [Tooltip("The amount of download speed we get by distance within min and max distance.")]
    public AnimationCurve mDistanceSignalInfluence = new AnimationCurve();
    [Tooltip("The max download speed we can gain with distance.")]
    [Min(0f)]public float mMaxDownloadingSpeedByDistance = 40f;
    [Tooltip("The max download speed we can gain with aiming.")]
    [Min(0f)]public float mMaxDownloadingSpeedByAiming = 20f;
    [Tooltip("The max download both distance and signal combined.")]
    [Min(0f)] public float mOverallMaxDownloadingSpeed = 50f;
    //[Tooltip("The ratio in radius needed to reconnect properly after being cut out for a long time.")]
    //[Range(0f,0.5f)]public float mInnerRadiusRatioToBigReconnect = 0.2f;
    [Tooltip("The ratio in radius needed to reconnect when out for a little, not yet cut out")]
    [Range(0.5f, 1f)] public float mInnerRadiusRatioToDirectReconnect = 0.8f;
    public float mDisconnectionTimeOut = 5f;
    

    private Quaternion mPreviousAntennaRotation;
    private Quaternion mScannerRotation;
    private bool mIsScanning = false;
    private Vector3 _SignalPosition => WifiSignal.sCurrentPosition;
    private bool mMaxConnection = false;
    private bool mKeepScanning = true;
    private bool mIsDirectlyConnected = false;
    private bool mIsBigConnected = false;
    private float mTimeLeftUntilBigDisconnection = 0;
    private float mAngleDebug = 0;

    private Vector3 _DirectionTowardsSignal => (_SignalPosition - transform.position).normalized;
    private float _CurrentDistance => Vector3.Distance(transform.position, _SignalPosition);
    private float _CurrentAngle => Mathf.Acos(Vector3.Dot(_DirectionTowardsSignal, mAntennaTransform.forward));

    private static SignalScanner sInstance;

    private float mCurrentDownloadingSpeed;
    public static float sCurrentDownloadingSpeed => sInstance.mCurrentDownloadingSpeed;
    public static bool sIsBigConnected => sInstance.mIsBigConnected;
    public static float sTimeLeftUntilBigDisconnection => sInstance.mTimeLeftUntilBigDisconnection;
    public static float sDistanceToBigReconnect => sInstance.mMinMaxConnectionDistance.x;
    public static GameManager.Notify OnDirectDisconnection;
    public static GameManager.Notify OnDirectReconnection;
    public static GameManager.Notify OnBigDisconnection;
    public static GameManager.Notify OnBigReconnection;

    private void Awake()
    {
        sInstance = this;
        PlayerMovement.OnStun +=delegate { mKeepScanning = false; };
        PlayerMovement.OnUnfreeze += delegate { mKeepScanning = true; };
        mScannerMaterial.SetFloat("_Slider", 0);
        GameManager.OnGameReady += delegate { StartScan(); mIsDirectlyConnected = true; mIsBigConnected = true; };
        OnBigDisconnection += delegate { mIsBigConnected = false; mIsDirectlyConnected = false; mKeepScanning = false; };
        OnBigReconnection += delegate { mIsBigConnected = true;  mIsDirectlyConnected = true; mKeepScanning = true; };
        OnDirectReconnection += delegate { mIsDirectlyConnected = true; mIsBigConnected = true; StopCoroutine(nameof(DisconnectionCountdown));  };
        OnDirectDisconnection += delegate { mIsDirectlyConnected = false; mTimeLeftUntilBigDisconnection = mDisconnectionTimeOut; mCurrentDownloadingSpeed = 0f; StartCoroutine(nameof(DisconnectionCountdown)); };
        mPreviousAntennaRotation = mAntennaTransform.rotation;
    }
    private float mDownloadByDistance;
    private float mDownloadByAiming;
    private void Update()
    {
        float mDistanceRatio = _CurrentDistance / mMinMaxConnectionDistance.y;
        if (!mIsDirectlyConnected && mIsBigConnected && mDistanceRatio < mInnerRadiusRatioToDirectReconnect)
            OnDirectReconnection();
        else if (mIsDirectlyConnected && mIsBigConnected && _CurrentDistance > mMinMaxConnectionDistance.y)
            OnDirectDisconnection();
        else if (!mIsDirectlyConnected && !mIsBigConnected && _CurrentDistance < mMinMaxConnectionDistance.x)
            OnBigReconnection();
        else if (mIsDirectlyConnected)
        {
            mMaxConnection = _CurrentDistance < mMinMaxConnectionDistance.x;
            if (!mMaxConnection)
            {
                float mDownloadByDistance = Mathf.Clamp01(1-mDistanceSignalCurve.Evaluate(mMinMaxConnectionDistance.InverseLerp(_CurrentDistance)));
                float mDownloadByAiming = Mathf.Clamp01(1-mAimingSignalCurve.Evaluate(mMinMaxSignalAngles.InverseLerp(_CurrentAngle)));

                mCurrentDownloadingSpeed = Mathf.Min(mDownloadByDistance * mMaxDownloadingSpeedByDistance + mDownloadByAiming * mMaxDownloadingSpeedByAiming, mOverallMaxDownloadingSpeed);
            }
            else mCurrentDownloadingSpeed = mOverallMaxDownloadingSpeed;
        }
    }

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

        if (mIsScanning)
        {
            mScannerPivot.rotation = mScannerRotation;
        }
    }
    private bool mCanReconnect = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!sIsBigConnected && !GameManager.sCountsAsPlaying && other.CompareTag("Signal")) OnBigReconnection();
    }

    private void StartScan()
    {
        StartCoroutine(ScanCoroutine());
    }
    float aiming;
    float curve;
    float angle;
    float ratio;
    private IEnumerator ScanCoroutine()
    {
        if (mKeepScanning)
        {
            mScannerMaterial.SetFloat("_Slider", 0);
            float currentTime = 0;
            mIsScanning = true;
            mScannerRotation = mAntennaTransform.rotation;
            mAngleDebug = _CurrentAngle * Mathf.Rad2Deg;
            float downloadByDistance = Mathf.Clamp01(mDistanceSignalCurve.Evaluate(1 - Mathf.Clamp01(mMinMaxConnectionDistance.InverseLerp(_CurrentDistance))));
            float downloadByAiming = Mathf.Clamp01(mAimingSignalCurve.Evaluate(1 - Mathf.Clamp01(mMinMaxSignalAngles.InverseLerp(_CurrentAngle * Mathf.Rad2Deg))));
            if (mMaxConnection) downloadByAiming = 1;
            aiming = downloadByAiming;
            curve = mAimingSignalCurve.Evaluate(1 - Mathf.Clamp01(mMinMaxSignalAngles.InverseLerp(_CurrentAngle * Mathf.Rad2Deg)));
            angle = _CurrentAngle * Mathf.Rad2Deg;
            ratio = mMinMaxSignalAngles.InverseLerp(_CurrentAngle * Mathf.Rad2Deg);
            //Debug.Log("New scan! Downloads distance and aiming: " + downloadByDistance + ", " + downloadByAiming + " the angle: " + _CurrentAngle * Mathf.Rad2Deg);

            mScannerMaterial.SetFloat("_MaxWifi", downloadByDistance);
            mScannerMaterial.SetColor("_Color", mScannerColorGradient.Evaluate(downloadByAiming));
            

            while (currentTime < mScannerDuration)
            {
                float progress = currentTime / mScannerDuration;
                mScannerMaterial.SetFloat("_Slider", progress);
                currentTime += Time.deltaTime;
                yield return null;
            }
            mScannerMaterial.SetFloat("_Slider", 1);
        }
        else yield return new WaitForSeconds(mScannerDuration);
            Invoke(nameof(StartScan), mRefreshTimer); 
    }

    private IEnumerator DisconnectionCountdown()
    {
        mTimeLeftUntilBigDisconnection = mDisconnectionTimeOut;
        while (mTimeLeftUntilBigDisconnection > 0)
        {
            mTimeLeftUntilBigDisconnection -= Time.deltaTime;
            yield return null;
        }
        OnBigDisconnection();
    }

    

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Handles.color = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.up, mMinMaxConnectionDistance.x);
        Handles.DrawWireDisc(transform.position, Vector3.up, mMinMaxConnectionDistance.y);
        Vector3 forward = mAntennaTransform.forward * mMinMaxConnectionDistance.y;
        Vector3 directionCloseLeft = Quaternion.AngleAxis(mMinMaxSignalAngles.x, Vector3.up) * forward;
        Vector3 directionCloseRight = Quaternion.AngleAxis(-mMinMaxSignalAngles.x, Vector3.up) * forward;
        Vector3 directionFarLeft = Quaternion.AngleAxis(mMinMaxSignalAngles.y, Vector3.up) * forward;
        Vector3 directionFarRight = Quaternion.AngleAxis(-mMinMaxSignalAngles.y, Vector3.up) * forward;

        Gizmos.DrawLine(transform.position, transform.position + directionCloseLeft);
        Gizmos.DrawLine(transform.position, transform.position + directionCloseRight);
        Gizmos.DrawLine(transform.position, transform.position + directionFarLeft);
        Gizmos.DrawLine(transform.position, transform.position + directionFarRight);
    } 
#endif
}
