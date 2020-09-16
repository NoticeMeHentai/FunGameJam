using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectToSpawn
{
    public GameObject mObject;
    public int mNbrOfObject;
    public int mRandomXYDegreeMax = 10;
}
public class MapGeneration : MonoBehaviour
{
    [InstanceButton(typeof(MapGeneration), nameof(RegenerateTerrain))]
    public Texture2D mHeightmap;
    public Texture2D mMaskmap;

    public GameObject mFence;
    public GameObject mElectricPillarAndWire;
    public GameObject mElectricPillar;

    public Material mMaskMaterial;

    public float mHeight = 1.0f;

    public float mMapSizeX = 10.0f;
    public float mMapSizeY = 10.0f;

    public float mGameZoneRatio = 0.5f;

    public int mQuadDensityPerUnit = 10;

    private Mesh mTerrain;

    [SerializeField]
    public ObjectToSpawn[] mObjectClass;

    private Vector3 mHalfExtents = new Vector3(1f, 0, 1f);
    private int mNbrDestroyed = 0;
    private int mNbrFailed = 0;
    private int mNbrSpawned = 0;
    private RaycastHit mHit;

    private bool mSpawned = false;
    public int mMaxTryIteration = 5;

    public LayerMask mSpawnObjectLayerMask;

    public int mFenceNbrOnSide = 20;
    public int mFenceRandomYSidesDegreeMax = 8;
    public float mFenceRandomXOffsetMax = 0.15f;

    public float mElectricPillarStartZoneRatio = 0.25f;
    public float mElectricPillarRandomYDegreeMax = 360;
    public float mElectricPillarFrontSize = 15;
    public int mElectricPillarNbr = 5;


    private void RegenerateTerrain()
    {
        Debug.Log("Building mesh");
        if (mTerrain != null)
            DestroyImmediate(mTerrain);

        mTerrain = new Mesh();
        mTerrain.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        int xCount = Mathf.CeilToInt(mMapSizeX * mQuadDensityPerUnit);
        int zCount = Mathf.CeilToInt(mMapSizeY * mQuadDensityPerUnit);

        int numVertices = (xCount + 1) * (zCount + 1);
        int numTriangles = xCount * zCount * 2;
        if(transform.childCount >= 0)
        {
            mNbrDestroyed = 0;
            Debug.Log("Ihavechild");
            for (int i = transform.childCount -1 ; i > -1; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
                mNbrDestroyed++;
            }
        }
        Vector3[] vertices = new Vector3[numVertices];
        Color[] colors = new Color[numVertices];
        Vector2[] uvs = new Vector2[numVertices];
        int[] triangles = new int[numTriangles * 3];

        bool heightMapUnreadable = false;
        bool maskMapUnreadable = false;

        int index = 0;
        for (float z = 0.0f; z < zCount + 1; z++)
        {
            for (float x = 0.0f; x < xCount + 1; x++)
            {
                float fxPos = (x / xCount);
                float fzPos = (z / zCount);
                float height = 0.0f;
                Color color = Color.white;
                Vector3 position = new Vector3();

                if (mHeightmap != null)
                {
                    try
                    {
                        float target = mHeightmap.GetPixelBilinear(1.0f - fxPos, 1.0f - fzPos).g;
                        height = mHeight * target;
                    }
                    catch (System.Exception)
                    {
                        heightMapUnreadable = true;
                    }
                }

                if (mMaskmap != null)
                {
                    try
                    {
                        color = mMaskmap.GetPixelBilinear(1.0f - fxPos, 1.0f - fzPos);
                    }
                    catch (System.Exception)
                    {
                        maskMapUnreadable = true;
                    }
                }

                position.y = height;
                position.x = (fxPos - 0.5f) * mMapSizeX;
                position.z = (fzPos - 0.5f) * mMapSizeY;

                colors[index] = color;
                uvs[index] = new Vector2(fxPos, fzPos);
                vertices[index++] = position;
            }
        }

        if (heightMapUnreadable)
            Debug.LogError("Heightmap unreadable!");
        if (maskMapUnreadable)
            Debug.LogError("maskmap unreadable!");

        index = 0;
        for (int z = 0; z < zCount; z++)
        {
            for (int x = 0; x < xCount; x++)
            {
                int vertexCountPerLine = xCount + 1;

                //    X     X+1   X+2
                //Z    0 --- 1 --- 2 --- 
                //     |     |     |
                //Z+1  L+0 --L+1 --L+2 ---
                //     |     |     |

                // First triangle - anti clockwise
                triangles[index++] = x + z * vertexCountPerLine;       // 0 - top-left
                triangles[index++] = x + (z + 1) * vertexCountPerLine; // L+0 - bottom-left
                triangles[index++] = x + 1 + z * vertexCountPerLine; //   1 - top-right

                // Second triangle - anti clockwise
                triangles[index++] = x + (z + 1) * vertexCountPerLine; // bottom
                triangles[index++] = x + 1 + (z + 1) * vertexCountPerLine; // bottom right
                triangles[index++] = x + 1 + z * vertexCountPerLine; // right
            }
        }

        mTerrain.vertices = vertices;
        mTerrain.triangles = triangles;
        mTerrain.colors = colors;
        mTerrain.uv = uvs;

        mTerrain.RecalculateNormals();
        mTerrain.RecalculateTangents();
        mTerrain.RecalculateBounds();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mTerrain;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshRenderer.material = mMaskMaterial;

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
            meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mTerrain;

        GenerateElectricLines();

        for (int j = 0; j < mObjectClass.Length; j++)
        {
            for (int i = 0; i < mObjectClass[j].mNbrOfObject; i++)
            {

                int nbrOfTry = 0;

                while (!mSpawned && nbrOfTry < mMaxTryIteration)
                {
                    Vector3 currentPosition = transform.position + new Vector3(Random.Range(0, mMapSizeX* mGameZoneRatio), 0.0f, Random.Range(0, mMapSizeY* mGameZoneRatio)) + new Vector3(-(mMapSizeX* mGameZoneRatio) / 2, 15, -(mMapSizeY* mGameZoneRatio) / 2);
                    float randomRotationX = Random.Range(0, mObjectClass[j].mRandomXYDegreeMax);
                    float randomRotationY = Random.Range(0, 360);
                    float randomRotationZ = Random.Range(0, mObjectClass[j].mRandomXYDegreeMax);

                    if (!Physics.BoxCast(currentPosition + Vector3.up * 10, mHalfExtents, -Vector3.up, Quaternion.identity, 50.0f, mSpawnObjectLayerMask)
                        && Physics.Raycast(currentPosition, -Vector3.up, out mHit, 50.0f))
                    {
                        Vector2 pixelUV = mHit.textureCoord;
                        pixelUV.x *= -mMaskmap.width;
                        pixelUV.y *= -mMaskmap.height;
                        Color color = mMaskmap.GetPixel(Mathf.FloorToInt(pixelUV.x), Mathf.FloorToInt(pixelUV.y));

                        if (color.r >= 0.4f)
                        {
                            GameObject objectsSpawned = Instantiate(mObjectClass[j].mObject, mHit.point - Vector3.up * 0.5f, Quaternion.Euler(randomRotationX, randomRotationY, randomRotationZ));
                            objectsSpawned.transform.parent = transform;
                            mNbrSpawned++;
                            mSpawned = true;
                        }
                        else
                        {
                            nbrOfTry++;
                        }
                    }
                }
                if (nbrOfTry >= mMaxTryIteration)
                {
                    mNbrFailed++;
                }
                mSpawned = false;
            }
        }
        Debug.Log("Destroyed : "+ mNbrDestroyed +"| failed : " + mNbrFailed + "| spawned : " + mNbrSpawned);
        mNbrFailed = 0;
        mNbrSpawned = 0;
        GenerateFenceBounds();
    }
    public void GenerateFenceBounds()
    {
        for (int j = 0; j < 4;j++)
        {
            for (int i = 0; i < mFenceNbrOnSide; i++)
            {
                float randomRotationY = Random.Range(-mFenceRandomYSidesDegreeMax, mFenceRandomYSidesDegreeMax);
                float randomRotationSide = Random.Range(-mFenceRandomYSidesDegreeMax, mFenceRandomYSidesDegreeMax);
                float randomOffset = Random.Range(-mFenceRandomXOffsetMax, mFenceRandomXOffsetMax);

                if(j == 0)
                {
                    GameObject objectsSpawned = Instantiate(mFence, transform.position + new Vector3(randomOffset, 0.0f, 0.0f) + new Vector3(-mMapSizeX / 2, mHeight, -mMapSizeY / 2) + new Vector3(0.0f, 0.0f, (mMapSizeY/mFenceNbrOnSide) * i), Quaternion.Euler(0.0f, randomRotationY, randomRotationSide));
                    objectsSpawned.transform.parent = transform;

                }
                else if (j == 1)
                {
                    GameObject objectsSpawned = Instantiate(mFence, transform.position + new Vector3(randomOffset, 0.0f, 0.0f) + new Vector3(mMapSizeX / 2, mHeight, -mMapSizeY / 2) + new Vector3(0.0f, 0.0f, (mMapSizeY / mFenceNbrOnSide) * i), Quaternion.Euler(0.0f, randomRotationY, randomRotationSide));
                    objectsSpawned.transform.parent = transform;
                }
                else if (j == 2)
                {
                    GameObject objectsSpawned = Instantiate(mFence, transform.position + new Vector3(0.0f, 0.0f, randomOffset) + new Vector3(-mMapSizeX / 2, mHeight, -mMapSizeY / 2) + new Vector3((mMapSizeX / mFenceNbrOnSide) * i, 0.0f, 0.0f), Quaternion.Euler(randomRotationSide, 0.0f, 0.0f)*Quaternion.Euler(0.0f, 90 + randomRotationY, 0.0f));
                    objectsSpawned.transform.parent = transform;
                }
                else
                {
                    GameObject objectsSpawned = Instantiate(mFence, transform.position + new Vector3(0.0f, 0.0f, randomOffset) + new Vector3(-mMapSizeX / 2, mHeight, mMapSizeY / 2) + new Vector3((mMapSizeX / mFenceNbrOnSide) * i, 0.0f, 0.0f), Quaternion.Euler(randomRotationSide, 0.0f, 0.0f)*Quaternion.Euler(0.0f, 90 + randomRotationY, 0.0f));
                    objectsSpawned.transform.parent = transform;
                }
            }
        }
    }
    public void GenerateElectricLines()
    {
        float randomRotationY = Random.Range(0.0f, mElectricPillarRandomYDegreeMax);
        Vector3 currentPosition = transform.position + new Vector3(Random.Range(0, mMapSizeX * mElectricPillarStartZoneRatio), -0.5f, Random.Range(0, mMapSizeY * mElectricPillarStartZoneRatio)) + new Vector3(-(mMapSizeX * mElectricPillarStartZoneRatio) / 2, 0.0f, -(mMapSizeY * mElectricPillarStartZoneRatio) / 2);
        GameObject electricLine = new GameObject();
        electricLine.transform.position = currentPosition;
        electricLine.transform.rotation = Quaternion.Euler(0.0f, randomRotationY, 0.0f);
        electricLine.transform.parent = transform;
        for (int i = 0; i < mElectricPillarNbr; i++)
        {
            GameObject pillarSpawned = Instantiate(mElectricPillarAndWire, currentPosition + (-electricLine.transform.forward * mMapSizeX)+ (electricLine.transform.forward * mElectricPillarFrontSize* i) , electricLine.transform.rotation);
            pillarSpawned.transform.parent = transform;
            if (i == mElectricPillarNbr - 1)
            {
                GameObject finalPillarSpawned = Instantiate(mElectricPillar, currentPosition + (-electricLine.transform.forward * mMapSizeX) + (electricLine.transform.forward * mElectricPillarFrontSize * (i+1)), electricLine.transform.rotation);
                finalPillarSpawned.transform.parent = transform;
            }

        }
    }
}
