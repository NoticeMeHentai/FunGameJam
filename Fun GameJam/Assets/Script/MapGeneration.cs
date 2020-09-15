using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public int mMapSizeX = 10;
    public int mMapSizeY = 10;
    public int mNbrObjectToSpawn = 10;
    public GameObject CubeTest;
    private Vector2 mCurrentpoint;
    private Vector3 mHalfExtents = new Vector3(0.49f, 0, 0.49f);
    private int mNbrSpawned = 0;
    private int mNbrFailed = 0;
    private bool Done = false;
    void Start()
    {
        if (!Done)
        {
            Done = true;
            for (int i = 0; i < mNbrObjectToSpawn; i++)
            {
                mCurrentpoint = new Vector2(Random.Range(0, (float)mMapSizeX), Random.Range(0, (float)mMapSizeY));
                Vector3 mCurrentPosition = transform.position + new Vector3(mCurrentpoint.x, 0.0f, mCurrentpoint.y);
                if (Physics.BoxCast(GetComponent<Collider>().bounds.center - Vector3.up * 10, transform.localScale, Vector3.up, Quaternion.identity, 20.0f))
                {
                    mNbrFailed++;
                }
                else
                {
                    Instantiate(CubeTest, mCurrentPosition, Quaternion.identity);
                    mNbrSpawned++;
                }
            }
            Debug.Log("failed : " + mNbrFailed + "| spawned : " + mNbrSpawned);
        }
    }

    
    void FixedUpdate()
    {

    }
}
