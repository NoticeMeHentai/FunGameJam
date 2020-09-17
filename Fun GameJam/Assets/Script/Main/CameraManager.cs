using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [Range(2f, 100f)] public float mDistance = 15f;
    [Range(10f,60f)]public float mLerpSpeed = 45f;
    [SerializeField] private bool mFollowPlayers = true;
    public Vector3 mIdealAngle = new Vector3(135, 130, 180);
    [Tooltip("Transform used when resetting the camera pos/rot")][InstanceButton(typeof(CameraManager),nameof(ResetCamera))]
    public Transform mTarget;



    public bool mLockView = false;
    [NewLabel("Reset distance"), Tooltip("Distance at which the camera will be placed from the selected transform when resetting")]
    [Min(5f)] public float mResetViewDistance = 15f;


    private Camera mThisCamera;


    private Vector3 mPreviousPos;
    private static CameraManager sInstance;
    private readonly Vector3 mDiabloAngle = new Vector3(135, 130, 180);
    private Vector3 mRight = new Vector3();
    private Vector3 mForward = new Vector3();

    #region Properties
    //FollowPlayer
    public static bool FollowPlayers { get { return sInstance.mFollowPlayers; } set { sInstance.mFollowPlayers = value; } }
    public static Transform CameraTransform => sInstance.transform;
    public static Camera CameraComponent => sInstance.mThisCamera;
    public static Vector3 ForwardDirection => sInstance.mForward;
    public static Vector3 RightDirection => sInstance.mRight;

    #endregion

    #region Monobehaviour

    private void Awake()
    {
        sInstance = this;
        mForward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        mRight = transform.right;

    }
    void Start()
    {
        mThisCamera = GetComponent<Camera>();
    }




    void LateUpdate()
    {
        Vector3 newPos = PlayerMovement.Position - transform.forward * mDistance;
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * mLerpSpeed);
    }

    public void ResetCamera()
    {
        transform.position = mTarget.position;
        transform.rotation = Quaternion.Euler(mIdealAngle);
        transform.position -= transform.forward * mDistance;
    }
    #endregion
}

