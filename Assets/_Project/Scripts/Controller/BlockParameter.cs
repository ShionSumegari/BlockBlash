using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockParameter : MonoBehaviour
{
    public float scale;
    public Vector3 position;

    public bool IsGray;

    public int[,] grids;

    public void GrayStatus(bool isgray)
    {
        IsGray = isgray;
        foreach(Transform tr in transform)
        {
            if (isgray)
                tr.GetComponent<BlockDrag>().ShowGrayBlock();
            else
                tr.GetComponent<BlockDrag>().ClearGrayBlock();
        }
    }
}
