using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Bees : MonoBehaviour
{
    public GameObject mBeesParticlesSystem;
    public GameObject mBeesAnchor;

    private GameObject mBees;
    private bool PlayerSet = false;
    public GameObject mPlayer;
    private float mDistanceDetection = 5.0f;
    public bool mDetectionStart = true;
    private float mBeeSpeed = 2.0f;
    private bool mReturn = false;
    public bool mHaveHitPlayer = false;
    private bool mCanAttack = true;

    private float mWaitTimeBeforeBack = 2.0f;
    private float mWaitTimeBeforeAttack = 8.0f;

    void Start()
    {
        mBees = Instantiate(mBeesParticlesSystem, mBeesAnchor.transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if(!PlayerSet/* && playerRefAccess != null*/)
        {
            PlayerSet = true; 
            //mPlayer = playerRefAccess;
        }
        if (mPlayer != null)
        {
            if (mCanAttack && !mHaveHitPlayer && (mPlayer.transform.position - mBeesAnchor.transform.position).magnitude < mDistanceDetection)
            {
                mReturn = false;
                if (mDetectionStart)
                {
                    mDetectionStart = false;
                }
                mBees.transform.position += (mPlayer.transform.position - mBees.transform.position).normalized * Time.deltaTime * mBeeSpeed;
            }
            else if (!mDetectionStart)
            {
                Debug.Log("test");
                mDetectionStart = true;
                StartCoroutine(WaitSomeTime(mWaitTimeBeforeBack));
            }
            if ((mPlayer.transform.position - mBees.transform.position).magnitude < 0.2f)
            {
                if(!mHaveHitPlayer)
                {
                    mCanAttack = false;
                    mHaveHitPlayer = true;
                    //mPlayer.GetComponent<PlayerMovement>().mStun = true;
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
}
