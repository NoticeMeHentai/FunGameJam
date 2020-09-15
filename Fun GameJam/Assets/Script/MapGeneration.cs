using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    [InstanceButton(typeof(MapGeneration), nameof(RegenerateTerrain))]
    public Texture2D mHeightmap;
    public Texture2D mMaskmap;

    public Material mMaskMaterial;

    public float mHeight = 1.0f;

    public float mMapSizeX = 10.0f;
    public float mMapSizeY = 10.0f;
    public int mQuadDensityPerUnit = 10;


    private Mesh mTerrain;
    public int mNbrObjectToSpawn = 10;
    public GameObject mCubeTest;
    private Vector2 mCurrentpoint;
    private Vector3 mHalfExtents = new Vector3(1f, 0, 1f);
    private int mNbrSpawned = 0;
    private int mNbrFailed = 0;
    private RaycastHit mHit;

    GameObject[] mObjectsSpawned;

    private bool mSpawned = false;
    public int mMaxTryIteration = 5;

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
        if(mObjectsSpawned != null)
        {
            for (int i = 0; i < mObjectsSpawned.Length; i++)
            {
                if(mObjectsSpawned[i] != null)
                {
                    DestroyImmediate(mObjectsSpawned[i]);
                }
            }
        }
        mObjectsSpawned = new GameObject[mNbrObjectToSpawn];
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



        for (int i = 0; i < mNbrObjectToSpawn; i++)
        {
            int nbrOfTry = 0;
            while(!mSpawned && nbrOfTry < mMaxTryIteration)
            {
                mCurrentpoint = new Vector2(Random.Range(0, mMapSizeX), Random.Range(0, mMapSizeY));
                Vector3 mCurrentPosition = transform.position + new Vector3(mCurrentpoint.x, 0.0f, mCurrentpoint.y)
                                           + new Vector3(-mMapSizeX / 2, 15, -mMapSizeY / 2);
                if (!Physics.BoxCast(mCurrentPosition + Vector3.up * 10, mHalfExtents, -Vector3.up, Quaternion.identity, 50.0f) 
                    && Physics.Raycast(mCurrentPosition, -Vector3.up, out mHit, 50.0f))
                {

                    //Renderer renderer = mHit.transform.GetComponent<MeshRenderer>();
                    Vector2 pixelUV = mHit.textureCoord;
                    pixelUV.x *= -mMaskmap.width;
                    pixelUV.y *= -mMaskmap.height;
                    Color color = mMaskmap.GetPixel(Mathf.FloorToInt(pixelUV.x), Mathf.FloorToInt(pixelUV.y));
                    if (color.r >= 0.9f)
                    {
                        mObjectsSpawned[i] = Instantiate(mCubeTest, mCurrentPosition, Quaternion.identity);
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
        Debug.Log("failed : " + mNbrFailed + "| spawned : " + mNbrSpawned);
        mNbrFailed = 0;
        mNbrSpawned = 0;
        //
    }
}
