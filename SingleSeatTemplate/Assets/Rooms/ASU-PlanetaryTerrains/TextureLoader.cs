using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureLoader : MonoBehaviour
{
    public List<Texture> heights;
    public List<Texture> colors;
    public TileMaker tilemaker;
    // Start is called before the first frame update
    void Start()
    {
        //LoadIndex(0);
    }

    public void LoadIndex(int i)
    {
        if (i<heights.Count && i < colors.Count)
        {
            tilemaker.LoadTextures(heights[i], colors[i]);
        }
    }

    public void LoadIndex(int i, float scale)
    {
        if (i < heights.Count && i < colors.Count)
        {
            tilemaker.LoadTextures(heights[i], colors[i], scale);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            LoadIndex(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoadIndex(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadIndex(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LoadIndex(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            LoadIndex(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            LoadIndex(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            LoadIndex(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            LoadIndex(7);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            LoadIndex(8);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            LoadIndex(9);
        }
    }

}
