using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController), typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{

    #region Exposed variables
    [Min(0f)]public float mMaxWalkSpeed = 10f;
    [Min(0f)] public float mRunSpeedRatio = 2.5f;
    [Range(0.2f,1f)]public float mAccelerationTime = 0.5f;
    [Range(10f,30f)]public float mRotationSpeed = 30f;
    #endregion

    #region Private/Local variables
    private float mCurrentSpeedRatio = 0;
    private bool mIsFrozen = false;
    /// The pack of information I'll get when raycasting down 
    private RaycastHit mDownHitInfo;
    /// The direction the player is currently looking at
    private Vector3 mDirection;
    private CharacterController mCharacterController;
    private Rigidbody mRigidbody;
    private float mMaxRunSpeed = 0;





    #endregion

    #region Properties
    private static PlayerMovement sInstance;
    //public static Vector3 Position = sInstance.transform.position;
    public static Vector3 GetPosition()
    {
        return sInstance.transform.position;
    }

    private Rigidbody _Rigidbody { get { if (mRigidbody == null) mRigidbody = GetComponentInChildren<Rigidbody>(); return mRigidbody; } }
    public static float SlowRatio { get; set; } = 0;

    public delegate void Notify();
    public static Notify OnFreeze;

    #endregion


    private void Awake()
    {
        if(sInstance != null)
        {
            Debug.Log("[Player] There was already an instance, wtf");
            Destroy(sInstance);
        }
        sInstance = this;
        this.tag = "Player";
        gameObject.layer = LayerMask.NameToLayer("Player");

        mCharacterController = GetComponent<CharacterController>();
        if (mCharacterController == null)
        {
            mCharacterController = gameObject.AddComponent<CharacterController>();
            mCharacterController.center = 0.9f * Vector3.up;
        }
        mCharacterController.height = 2;
        mCharacterController.radius = 0.25f;

        mMaxRunSpeed = mMaxWalkSpeed * mRunSpeedRatio;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Start()
    {
    }

    

    private void Update()
    {

        if (!mIsFrozen)
        {
            MoveAndRotate();
        }

    }

    private void FixedUpdate()
    {
        Shader.SetGlobalVector("PlayerPosition", transform.position);
        
    }

    #region Input Functions
    /// <summary>
    /// Moves the character depending on the axis values
    /// </summary>
    private void MoveAndRotate()
    {
        Vector2 movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));


        float lerpSpeed = mCurrentSpeedRatio;

        movementInput = movementInput.LimitMagnitude(1f);

        //Smooth lineear gradient movement
        if (movementInput.sqrMagnitude > 0.2) //If acceleration
        {
            lerpSpeed = MathHelper.HardIn(Mathf.Clamp(lerpSpeed + movementInput.sqrMagnitude * Time.deltaTime / mAccelerationTime, 0f, movementInput.sqrMagnitude), FunctionsCurves.Xexp2);
        }
        else if (lerpSpeed > 0.05f) //If not giving any input and it's still slowing down, slow down
        {
            lerpSpeed = MathHelper.EasyIn(Mathf.Clamp01(lerpSpeed - Time.deltaTime / mAccelerationTime), FunctionsCurves.Xexp3);
            //lerpSpeed = Mathf.Clamp01(lerpSpeed - Time.deltaTime / _AccelerationTime);
            if (lerpSpeed <= 0.1f) lerpSpeed = 0f;
        }
        float mCurrentSpeed = mMaxWalkSpeed *lerpSpeed * (1 - SlowRatio) * Input.GetAxis("Run")>0.2f?mMaxRunSpeed:mMaxWalkSpeed;

        //Delta movement calculus
        mDirection = CameraManager.RightDirection * movementInput.x + CameraManager.ForwardDirection * movementInput.y;

        mDirection.y = 0;
        //mDirection = Vector3.Normalize(mDirection);


        //Rotation
        if (mDirection.magnitude > 0.01f)
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(mDirection), mRotationSpeed * Time.deltaTime);
        mDirection *= mCurrentSpeed * Time.deltaTime;


        //Gravity simulation
        Vector3 startingPoint = transform.position + Vector3.up * 0.4f;
        if (Physics.Raycast(startingPoint, Vector3.down, out mDownHitInfo, 50f, MathHelper.GroundLayerMask) && Vector3.Distance(mDownHitInfo.point, startingPoint) > 0)
        {
            Debug.DrawLine(startingPoint, mDownHitInfo.point, Color.red);
            mDirection += Vector3.up * Physics.gravity.y * Time.deltaTime; //Gravity is negative, hence the adding
            mDirection += Vector3.up * 0.02f; //Small offset so that quads or effects placed aroung the player won't be hidden beneath the floor
        }

        //If there's a "Object reference" error leading here, then it means something went wrong when instantiating the characters
        if (mCurrentSpeed > 0)
        {
            Vector3 smoothDelta = (Vector3.Lerp(transform.position, transform.position + mDirection, 45f * Time.deltaTime)) - transform.position;
            mCharacterController.Move(smoothDelta);
        }
    }




    #endregion

    private void Unfreeze()
    {
        mIsFrozen = false;
    }
    public static void Freeze(bool value)
    {
        sInstance.mIsFrozen = true;
        if(OnFreeze!=null)OnFreeze();
    }
}
