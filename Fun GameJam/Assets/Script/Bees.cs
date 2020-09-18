using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
#if UNITY_EDITOR
using UnityEditor; 
#endif

public class Bees : MonoBehaviour
{
    public GameObject mBeesParticlesSystem;
    public GameObject mBeesAnchor;

    private GameObject mBees;
    private bool PlayerSet = false;
    private float mDistanceDetection = 5.0f;
    public bool mDetectionStart = true;
    private float mBeeSpeed = 2.0f;
    private bool mReturn = false;
    public bool mHaveHitPlayer = false;
    private bool mCanAttack = true;
    public float mDistanceBeforeStun = 0.2f;

    private float mWaitTimeBeforeBack = 2.0f;
    private float mWaitTimeBeforeAttack = 8.0f;

    void Start()
    {
        mBees = Instantiate(mBeesParticlesSystem, mBeesAnchor.transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (mCanAttack && !mHaveHitPlayer && (PlayerMovement.Position - mBeesAnchor.transform.position).magnitude < mDistanceDetection)
        {
            mReturn = false;
            if (mDetectionStart)
            {
                mDetectionStart = false;
            }
            mBees.transform.position += (PlayerMovement.Position - mBees.transform.position).normalized * Time.deltaTime * mBeeSpeed;
        }
        else if (!mDetectionStart)
        {
            Debug.Log("test");
            mDetectionStart = true;
            StartCoroutine(WaitSomeTime(mWaitTimeBeforeBack));
        }
        if ((PlayerMovement.Position - mBees.transform.position).magnitude < mDistanceBeforeStun)
        {
            if (!mHaveHitPlayer)
            {
                mCanAttack = false;
                mHaveHitPlayer = true;
                PlayerMovement.Stun(true);
            }
        }
        if (mReturn)
        {
            mBees.transform.position += (mBeesAnchor.transform.position - mBees.transform.position).normalized * Time.deltaTime * mBeeSpeed;
        }
        if (mReturn && (mBeesAnchor.transform.position - mBees.transform.position).magnitude < 0.1f)
        {
            StartCoroutine(WaitSomeTime2(mWaitTimeBeforeAttack));
            mReturn = false;
            mHaveHitPlayer = false;
        }
    }
    IEnumerator WaitSomeTime(float TimeToWait)
    {
        Debug.Log("Coroutine!!!!");
        yield return new WaitForSeconds(TimeToWait);
        mReturn = true;
    }
    IEnumerator WaitSomeTime2(float TimeToWait)
    {
        Debug.Log("Coroutine!!!!");
        yield return new WaitForSeconds(TimeToWait);
        mCanAttack = true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.up, mDistanceDetection);
    } 
#endif
}
