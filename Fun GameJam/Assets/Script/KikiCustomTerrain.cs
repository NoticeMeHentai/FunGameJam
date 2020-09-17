using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KikiCustomTerrain : MonoBehaviour
{
    [InstanceButton(typeof(KikiCustomTerrain),nameof(RegenerateTerrain))]
    public Texture2D Heightmap;
    public Texture2D Maskmap;

    public float Height = 1.0f;

    public float SizeX = 10.0f;
    public float SizeZ = 10.0f;
    public int QuadDensityPerUnit = 10;


    private Mesh terrain;

    public static Vector2 sRandomPivot = new Vector2();
    public static float sRandomRotation = 0;
    public static Texture2D sMaskMap;


    private void RegenerateTerrain()
    {
        Debug.Log("Building mesh");
        if (terrain != null)
            DestroyImmediate(terrain);

        terrain = new Mesh();
        terrain.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        int xCount = Mathf.CeilToInt(SizeX * QuadDensityPerUnit);
        int zCount = Mathf.CeilToInt(SizeZ * QuadDensityPerUnit);

        int numVertices = (xCount + 1) * (zCount + 1);
        int numTriangles = xCount * zCount * 2;

        Vector3[] vertices = new Vector3[numVertices];
        Color[] colors = new Color[numVertices];
        Vector2[] uvs = new Vector2[numVertices];
        int[] triangles = new int[numTriangles * 3];

        float randomOffset = Random.Range(-1f, 1f);
        sRandomRotation = Random.Range(-10f, 10f)*Mathf.Deg2Rad;
        float sin2 = Mathf.Sin(sRandomRotation);
        float con2 = Mathf.Cos(sRandomRotation);
        Vector2 middleMap = new Vector2(0.5f, 0.5f);
        sRandomPivot = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

        bool heightMapUnreadable = false;
        bool maskMapUnreadable = false;
        sMaskMap = Maskmap;



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



                Vector2 uvPos = new Vector2(fxPos, fzPos);
                uvPos = uvPos.Rotate(sRandomRotation, sRandomPivot);
                //uvPos += new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                

                if (Heightmap != null)
                {
                    try
                    {
                        float target = Heightmap.GetPixelBilinear(1.0f - uvPos.x, 1.0f - uvPos.y).g;
                        height = Height * target;
                    }
                    catch (System.Exception)
                    {
                        heightMapUnreadable = true;
                    }
                }

                if (Maskmap != null)
                {
                    try
                    {
                        color = Maskmap.GetPixelBilinear(1.0f - uvPos.x, 1.0f - uvPos.y);
                    }
                    catch (System.Exception)
                    {
                        maskMapUnreadable = true;
                    }
                }

                position.y = height;
                position.x = (fxPos - 0.5f) * SizeX;
                position.z = (fzPos - 0.5f) * SizeZ;

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

        terrain.vertices = vertices;
        terrain.triangles = triangles;
        terrain.colors = colors;
        terrain.uv = uvs;

        terrain.RecalculateNormals();
        terrain.RecalculateTangents();
        terrain.RecalculateBounds();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = terrain;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
            meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = terrain;
    }
}
