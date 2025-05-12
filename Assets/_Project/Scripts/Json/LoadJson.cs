using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;

public class GridData
{
    public int id;
    public int[,] data; // Đọc đúng dạng danh sách
}

public class GridCollection
{
    public List<GridData> grids = new List<GridData>();
}

public class LoadJson : MonoBehaviour
{
    [HideInInspector] public GridCollection gridCollection;

    void Awake()
    {
        LoadJsonFromFile();
    }

    void LoadJsonFromFile()
    {
        TextAsset path = Resources.Load<TextAsset>("blocks");

        if (path != null)
        {
            string json = path.text;
            gridCollection = JsonConvert.DeserializeObject<GridCollection>(json);

            Debug.Log("JSON Loaded Successfully!");
            //PrintGridData();
        }
        else
        {
            Debug.LogError("JSON file not found at: " + path);
        }
    }

    void PrintGridData()
    {
        foreach (var grid in gridCollection.grids)
        {
            Debug.Log($"Grid ID: {grid.id}");
            foreach (var row in grid.data)
            {
                Debug.Log(string.Join(" ", row)); // Hiển thị hàng ngang
            }
        }
    }
}