using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMaker : MonoBehaviour
{
    //public GameObject tilePrefab;
    public Material material;
//    public Material reverseMaterial;
    public int tileSize = 40; //width and height of tile (in number of vertices)
    public int numTiles = 10; //number of tiles in X and Y dimensions e.g., 5x5
    //public bool rescale = false;

    GameObject[,] tiles;
    int[] triangles;
    Vector3[] vertices;
    Vector2[] newUV;
    private Vector3[] normals;

    
    public void LoadTextures(Texture heightTexture, Texture colorTexture, float scale = 1)
    {
        material.SetTexture("_HeightMap", heightTexture);
        material.SetTexture("_MainTex", colorTexture);
        material.SetFloat("_length", heightTexture.height);
        material.SetFloat("_width", heightTexture.width);
        material.SetFloat("_scaleFactor", 0.04f);

        if (heightTexture.width > heightTexture.height) {
            transform.localScale = new Vector3(scale, scale, heightTexture.height * scale / heightTexture.width);
        }
        else
        {
            transform.localScale = new Vector3(heightTexture.width * scale / heightTexture.height, scale, scale);
        }
    }
    void MakeVerticesTriangles(float scale)
    {
        triangles = new int[2 * (tileSize - 1) * (tileSize - 1) * 3 * 2];//last *2 is added for the wall
        vertices = new Vector3[tileSize * tileSize + tileSize * tileSize];
        normals = new Vector3[tileSize * tileSize + tileSize * tileSize];
        for (int j = 0; j < tileSize; j++)
        {
            for (int i = 0; i < tileSize; i++)
            {
                vertices[j * tileSize + i] = new Vector3((i / (tileSize - 1.0f) - 0.5f) * scale, 0, (j / (tileSize - 1.0f) - 0.5f)*scale);
                normals[j * tileSize + i] = Vector3.up;
            }
        }
        for (int j = 0; j < tileSize - 1; j++)
        {
            for (int i = 0; i < tileSize - 1; i++)
            {
                int trisIndex = j * (tileSize - 1) + i;
                int index = j * (tileSize) + i;
                triangles[trisIndex * 2 * 3 + 0] = index;
                triangles[trisIndex * 2 * 3 + 1] = index + 1;
                triangles[trisIndex * 2 * 3 + 2] = index + tileSize;
                triangles[trisIndex * 2 * 3 + 3] = index + 1;
                triangles[trisIndex * 2 * 3 + 4] = index + tileSize + 1;
                triangles[trisIndex * 2 * 3 + 5] = index + tileSize;
            }
        }
        for (int k = 0; k < tileSize ; k++)
        {
            for (int i = 0; i < tileSize; i++)
            {
                float centerJ = tileSize / 2;
                float jLoc = (centerJ / (tileSize - 1.0f) - 0.5f) * scale;
                vertices[tileSize * tileSize + k * tileSize + i] = new Vector3((i / (tileSize - 1.0f) - 0.5f) * scale, (k / (tileSize - 1.0f)-0.5f)*5*scale, jLoc);
                normals[tileSize * tileSize + k * tileSize + i] = Vector3.up;
            }
        }

        int triangleOffset = 2 * (tileSize - 1) * (tileSize - 1) * 3;
        for (int j = 0; j < tileSize - 1; j++)
        {
            for (int i = 0; i < tileSize - 1; i++)
            {
                int trisIndex = j * (tileSize - 1) + i + (tileSize-1)*(tileSize-1);
                int index = j * (tileSize) + i + tileSize*tileSize;
                triangles[trisIndex * 2 * 3 + 0] = index;
                triangles[trisIndex * 2 * 3 + 1] = index + 1;
                triangles[trisIndex * 2 * 3 + 2] = index + tileSize;
                triangles[trisIndex * 2 * 3 + 3] = index + 1;
                triangles[trisIndex * 2 * 3 + 4] = index + tileSize + 1;
                triangles[trisIndex * 2 * 3 + 5] = index + tileSize;
                //triangles[triangleOffset + trisIndex*2*3 + 0] = tileSize * tileSize + index;
                //triangles[triangleOffset + trisIndex * 2 * 3 + 1] = tileSize * tileSize + index + 1;
                //triangles[triangleOffset + trisIndex * 2 * 3 + 2] = tileSize * tileSize + index + tileSize;
                //triangles[triangleOffset + trisIndex * 2 * 3 + 3] = tileSize * tileSize + index + 1;
                //triangles[triangleOffset + trisIndex * 2 * 3 + 4] = tileSize * tileSize + index + tileSize + 1;
                //triangles[triangleOffset + trisIndex * 2 * 3 + 5] = tileSize * tileSize + index + tileSize;
            }
        }
    }
        /// <summary>
        /// create a new tile game object
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        GameObject MakeTile(float x, float y, float width, float height, string label = "Tile")
    {
        Mesh newMesh = new Mesh();
        newMesh.SetVertices(vertices);
        newMesh.triangles = triangles;
        //        newMesh.RecalculateNormals();
        newMesh.normals = normals;
        if (newUV == null)
        {
            newUV = new Vector2[tileSize * tileSize * 2];
        }

        float newU, newV;
        float margin = 1.0f / (numTiles * tileSize) + .0001f;
        // Debug.Log("margins" + margin + ","+margin+"/"+tileSize+":"+width);

        int index = 0;
        //Debug.Log("newUV length: " + newUV.Length);
        for (int j = 0; j < tileSize; j++)
        {
            newV = y + j / (tileSize - 1.0f) * height;

            for (int i = 0; i < tileSize; i++)
            {
                newU = x + i / (tileSize - 1.0f) * width;
                newUV[index] = new Vector2(newU, newV);
                index++;
            }
        }
        index = 0;
        float centerJ = tileSize / 2;
        for (int k = 0; k < tileSize; k++)
        {
            newV = y + centerJ / (tileSize - 1.0f) * height;

            for (int i = 0; i < tileSize; i++)
            {
                newU = x + i / (tileSize - 1.0f) * width;
                newUV[index+tileSize*tileSize] = new Vector2(newU, newV);
                index++;
            }
        }

        newMesh.SetUVs(0, newUV);

        GameObject newMeshChild = new GameObject(label);
        newMeshChild.transform.parent = this.transform;
        newMeshChild.transform.localRotation = Quaternion.identity;
        newMeshChild.AddComponent<MeshFilter>();
        newMeshChild.AddComponent<MeshRenderer>();
        newMeshChild.GetComponent<MeshFilter>().mesh = newMesh;

        return newMeshChild;

    }
    //public GameObject centerTile;
    public Vector3 SpotAboveCenterTile() {
        //if (centerTile == null) {
        //    Debug.Log("no center tile");
        //    return Vector3.zero;
        //}
        ////Ray r = new Ray(centerTile.transform.position + Vector3.up * 1000f, Vector3.down);
        //Ray r = new Ray(Vector3.up * 1000f + centerTile.transform.position, Vector3.down);
        //RaycastHit hit;
        //if (centerTile.GetComponent<MeshCollider>().Raycast(r, out hit, 10000f)) {
        //    return hit.point;
        //}
        return Vector3.zero;
    }


    void Awake()
    {
        tiles = new GameObject[numTiles,numTiles];
        MakeVerticesTriangles(1f / numTiles);
        for (int j = 0; j < numTiles; j++)
        {
            for (int i = 0; i < numTiles; i++)
            {
                //float x, float y, float width, float height, string label = "Tile"
                GameObject tile = MakeTile((float)i * 1.0f / numTiles, (float)j * 1.0f / numTiles, 1.0f / numTiles, 1.0f / numTiles, "Tile_" + i + "_" + j);
                
                tile.transform.localPosition = new Vector3((i + 0.5f) / numTiles - 0.5f, 0, (j + 0.5f) / numTiles - 0.5f);
                tile.GetComponent<MeshRenderer>().material = material;
                tile.transform.localScale = Vector3.one;
                
                tiles[j, i] = tile;
                
            }
        }
    }
    //private void Update()
    //{
    //    //float base_exag = 0.04f;// 40 / 255f;
    //    //Texture2D tex = (Texture2D)material.GetTexture("_HeightMap");
    //    //Debug.Log("HeightMap" + tex);
    //    //for (int j = 0; j < numTiles; j++)
    //    //{
    //    //    for (int i = 0; i < numTiles; i++)
    //    //    {
    //    //        float z = tex.GetPixelBilinear((i+0.5f)/numTiles, (j+0.5f) / numTiles).grayscale * base_exag;
    //    //        tiles[j, i].transform.localPosition = new Vector3((i + 0.5f) / numTiles - 0.5f, (j + 0.5f) / numTiles - 0.5f, z);

    //    //    }
    //    //}
    //            /*
    //            Texture tex = material.GetTexture("_MainTex");  //the main diffuse texture
    //            float aspect = 1f * tex.height / tex.width;
    //            float scale = 1f;

    //            if (rescale)
    //            {
    //                scale = MakeAppearOnPlane.scale;
    //            }

    //            transform.localScale = new Vector3(scale, -aspect*scale, scale);
    //            */
    //        }

}
