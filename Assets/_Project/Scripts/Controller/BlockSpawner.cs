using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    /// <summary>
    /// Spacing equal board scale multiply grid cell size
    /// Json to store block values
    /// </summary>

    [SerializeField] private GridSearch gridSearch;
    [SerializeField] private float spacing;
    [SerializeField] private float scaleBlock;
    [SerializeField] private LoadJson json;
    [SerializeField] private GameObject blockObject;
    [SerializeField] private List<Vector2> listBlockPosition;

    public static bool advanced = false;

    public void SetMode(bool _advanced)
    {
        advanced = _advanced;
        CheckSpawnBlock();
    }

    public void CheckSpawnBlock()
    {
        if(transform.childCount <= 0)
        {
            if (!advanced)
                SpawnBlock();
            else SpawnBlockAdvaanced();
        }
        GameController.Instance.CheckBlock();
    }

    void SpawnBlock()
    {
        List<int> listBlock = GetRandomNumbers(0, json.gridCollection.grids.Count, 3);
        List<int> listColor = GetRandomNumbers(1, ColorCellManager.Instance.listCellColor.Count, 3);
        for(int i = 0; i <= 2; i++)
        {
            GameObject block = SpawnObjects(listBlock[i], listColor[i]);
            block.transform.SetParent(transform);

            block.transform.localScale = Vector3.one * scaleBlock;
            block.GetComponent<BlockParameter>().scale = scaleBlock;

            block.transform.localPosition = listBlockPosition[i];
            block.GetComponent<BlockParameter>().position = block.transform.position;
        }
    }

    void SpawnBlockAdvaanced()
    {
        List<(int[,], int)> listCombination = gridSearch.GridCombination();
        List<int> listBlock = new List<int>(3) { 0, 1, 2 };
        listBlock = listBlock.OrderBy(x => Random.value).ToList();
        List<int> listColor = GetRandomNumbers(1, ColorCellManager.Instance.listCellColor.Count, 3);
        for (int i = 0; i <= 2; i++)
        {
            GameObject block = SpawnObjects(listCombination, listBlock[i], listColor[i]);
            block.transform.SetParent(transform);

            block.transform.localScale = Vector3.one * scaleBlock;
            block.GetComponent<BlockParameter>().scale = scaleBlock;

            block.transform.localPosition = listBlockPosition[i];
            block.GetComponent<BlockParameter>().position = block.transform.position;
        }
    }

    GameObject SpawnObjects(int indexBlock, int indexColor)
    {
        var grid = json.gridCollection.grids[indexBlock];

        GameObject parentObject = new GameObject($"Grid_{grid.id}");
        parentObject.transform.position = Vector3.zero;
        parentObject.AddComponent<BlockParameter>();
        parentObject.GetComponent<BlockParameter>().grids = grid.data;

        List<Vector3> blockPositions = new List<Vector3>();

        int rows = grid.data.GetLength(0);
        int cols = grid.data.GetLength(1);

        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (grid.data[row,col] == 1)
                {
                    Vector3 position = new Vector3(col * spacing, -row * spacing, 0);
                    blockPositions.Add(position);

                    if (position.x < minX) minX = position.x;
                    if (position.x > maxX) maxX = position.x;
                    if (position.y < minY) minY = position.y;
                    if (position.y > maxY) maxY = position.y;
                }
            }
        }

        Vector3 gridCenter = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);

        foreach (var pos in blockPositions)
        {
            Vector3 adjustedPos = pos - gridCenter;
            GameObject block = Instantiate(blockObject, adjustedPos, Quaternion.identity);
            block.transform.SetParent(parentObject.transform);
            block.GetComponent<BlockDrag>().IndexCell = indexColor;
        }
        return parentObject;
    }

    GameObject SpawnObjects(List<(int[,], int)> listCombination, int indexBlock, int indexColor)
    {
        var grid = listCombination[indexBlock];

        GameObject parentObject = new GameObject($"Grid_{grid.Item2}");
        parentObject.transform.position = Vector3.zero;
        parentObject.AddComponent<BlockParameter>();
        parentObject.GetComponent<BlockParameter>().grids = grid.Item1;

        List<Vector3> blockPositions = new List<Vector3>();

        int rows = grid.Item1.GetLength(0);
        int cols = grid.Item1.GetLength(1);

        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (grid.Item1[row, col] == 1)
                {
                    Vector3 position = new Vector3(col * spacing, -row * spacing, 0);
                    blockPositions.Add(position);

                    if (position.x < minX) minX = position.x;
                    if (position.x > maxX) maxX = position.x;
                    if (position.y < minY) minY = position.y;
                    if (position.y > maxY) maxY = position.y;
                }
            }
        }

        Vector3 gridCenter = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);

        foreach (var pos in blockPositions)
        {
            Vector3 adjustedPos = pos - gridCenter;
            GameObject block = Instantiate(blockObject, adjustedPos, Quaternion.identity);
            block.transform.SetParent(parentObject.transform);
            block.GetComponent<BlockDrag>().IndexCell = indexColor;
        }
        return parentObject;
    }

    List<int> GetRandomNumbers(int min, int max, int count)
    {
        HashSet<int> uniqueNumbers = new HashSet<int>();

        while (uniqueNumbers.Count < count)
        {
            int randomNum = Random.Range(min, max);
            uniqueNumbers.Add(randomNum);
        }

        return new List<int>(uniqueNumbers);
    }

    public void ResetBlock()
    {
        foreach(Transform tr in transform) Destroy(tr.gameObject);
        Invoke(nameof(CheckSpawnBlock), 0.2f);
    }
}
