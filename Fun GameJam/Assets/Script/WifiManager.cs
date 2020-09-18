using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[ExecuteInEditMode]
public class WifiManager : MonoBehaviour
{
    [BoxedHeader("Construction parameters")]
    [Tooltip("The max half-distance at which the wifi points should be sprayed out")]
    public float mDistance = 40f;
    [Tooltip("The amount of base wifi points to be sprayed at each line/colon")]
    public int mSquareAmount = 10;
    [Tooltip("The percentage of wifi points to be masked in the map")]
    [Range(0f, 0.5f)] public float mRandomMask = 0.2f;
    [Tooltip("The maximum of offset that can be applied to the position of a wifi point. For every unit, the wifi point can be offseted of a whole tile. A different offset is applied to each axis")]
    [Range(0f,1.5f)]public float mRandomMaxOffset = 0.2f;
    public AnimationCurve mOffsetIntensityCurve = new AnimationCurve();
    [BoxedHeader("Gameplay parameters")]
    [Tooltip("Distance at which the wifi points should be separated from")]
    [Min(0f)] public float mMinDistanceBetweenPoints = 3f;
    [Tooltip("The max distance the points will be linked to other points")]
    [Min(0f)] public float mMaxDistanceLinkBetweenPoints = 10f;

    [InstanceButton(typeof(WifiManager), nameof(PlaceWifiPoints))]
    public bool mActive = false;
    public GameObject mPrefab;
    [Disable, TextArea]public string mInfo = "Shift + Left Click = Add \n Shift + Right Click = Relay \n Ctrl + Left Click = Delete";

    private int mWifiLayerMask = 0;
    private int _WifiLayerMask { get { if (mWifiLayerMask == 0) mWifiLayerMask = 1 << LayerMask.NameToLayer("WifiPoints"); return mWifiLayerMask; } }



    private static WifiManager sInstance;
    private void OnEnable()
    {
        sInstance = this;
    }

    private void Awake()
    {
        GameManager.OnGamePreparation += PlaceWifiPoints;
    }

    public static bool sIsActive => sInstance.mActive;
    public static GameObject sPrefab => sInstance.mPrefab;
    public static WifiPoint sClosestWifiPoint { get; private set; }
    public static float sRadius { get { if (sInstance != null) return sInstance.mMinDistanceBetweenPoints; else return 2f; } }
    private void PlaceWifiPoints()
    {
        //Debug.Log("Starting placing wifi points");
        int childCount = transform.childCount;
        for (int i = 1; i < childCount; i++) DestroyImmediate(transform.GetChild(1).gameObject);

        int totalAmount = mSquareAmount * mSquareAmount;

        //Random IDs for random masking
        float[] ids = new float[totalAmount];
        List<float> test = new List<float>(5);
        float fraction = 1f / (totalAmount - 1);
        for (int i = 0; i < totalAmount; i++) ids[i] = fraction * i;
        System.Random rnd = new System.Random();
        rnd.Shuffle<float>(ids);
        //Debug.Log("IDs after randomization:" + ids.ToString());

        List<WifiPoint> wifiPoints = new List<WifiPoint>(totalAmount);

        #region Random Placing
        float singleDistance = 1f / (mSquareAmount - 1) * mDistance * 2;
        Vector3 startingPoint = transform.position - mDistance * Vector3.right - mDistance * Vector3.forward;
        for (int i = 0; i < mSquareAmount; i++)
            for (int j = 0; j < mSquareAmount; j++)
            {
                float randomX = mOffsetIntensityCurve.Evaluate(Random.value) * mRandomMaxOffset * (Random.value < 0.5 ? -1 : 1);
                float randomZ = mOffsetIntensityCurve.Evaluate(Random.value) * mRandomMaxOffset * (Random.value < 0.5 ? -1 : 1);

                Vector3 actualPosition = startingPoint + Vector3.right * (singleDistance * (j + randomX)) + Vector3.forward * (singleDistance * (i + randomZ));
                WifiPoint newPoint = Instantiate(mPrefab, actualPosition, Quaternion.identity, transform).GetComponent<WifiPoint>();
                newPoint.mIDMask = ids[i * mSquareAmount + j];
                newPoint.mIDCounter = i * mSquareAmount + j;
                wifiPoints.Add(newPoint);
                //Debug.LogFormat("Placed a new point at {0} with ID {1}, actual number {2}", actualPosition, newPoint.mIDMask, i * mSquareAmount + j);
            }
        #endregion

        #region Random Masking
        for (int i = 0; i < wifiPoints.Count; i++)
        {

            if (wifiPoints[i].mIDMask < mRandomMask)
            {
                //Debug.LogFormat("Destroying the point number {0} with an ID of {1}", i, wifiPoints[i].mIDMask);
                DestroyImmediate(wifiPoints[i].gameObject);
                wifiPoints.RemoveAt(i);
                i--;
            }
        }
    
        #endregion

        #region Removing points too close one another
        RaycastHit[] hitInfos;
        for (int i = 0; i < wifiPoints.Count; i++)
        {
            hitInfos = Physics.SphereCastAll(wifiPoints[i].transform.position, mMinDistanceBetweenPoints, Vector3.up, 0.1f, _WifiLayerMask);
            foreach (RaycastHit hit in hitInfos)
            {
                WifiPoint wifiRef = hit.collider.GetComponent<WifiPoint>();
                int ID = wifiRef.mIDCounter;
                //Debug.LogFormat("Point {0} was too close so it's been removed", ID);
                DestroyImmediate(hit.collider.gameObject);
                wifiPoints.Remove(wifiRef);
            }
        }
        #endregion

        #region Removing pounts out of range distance
        Vector2 maxXranges = Vector2.one * transform.position.x - Vector2.right * mDistance + Vector2.up * mDistance;
        Vector2 maxZranges = Vector2.one * transform.position.z - Vector2.right * mDistance + Vector2.up * mDistance;
        for (int i = 0; i < wifiPoints.Count; i++)
        {
            if (!maxXranges.WithinRange(wifiPoints[i].transform.position.x) || !maxZranges.WithinRange(wifiPoints[i].transform.position.z))
            {
                int ID = wifiPoints[i].mIDCounter;
                DestroyImmediate(wifiPoints[i].gameObject);
                wifiPoints.RemoveAt(i);
                i--;
                //Debug.LogFormat("Removed point {0} because it was out of range", ID);
            }
        }
        #endregion

        #region Link points
        float closestDistance = Vector3.Distance(wifiPoints[0].transform.position, transform.position);
        WifiPoint closestPoint = wifiPoints[0];
        for (int i = 0; i < wifiPoints.Count; i++)
        {
            WifiPoint currentPoint = wifiPoints[i];
            if (i != 0)
            {
                float currentDistance = Vector3.Distance(wifiPoints[i].transform.position, transform.position);
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    closestPoint = wifiPoints[i];
                }
            }
            List<WifiPoint> currentLinkedPoints = new List<WifiPoint>();
            for (int j = 0; j < wifiPoints.Count; j++)
            {
                WifiPoint currentSubPoint = wifiPoints[j];
                if (j != i && Vector3.Distance(currentSubPoint.transform.position, currentPoint.transform.position) < mMaxDistanceLinkBetweenPoints)
                    currentLinkedPoints.Add(currentSubPoint);
            }
            currentPoint.mWifiPoints = currentLinkedPoints;
        }

        sClosestWifiPoint = closestPoint;
        #endregion
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, (Vector3.right + Vector3.forward) * mDistance*2);
    }

}
