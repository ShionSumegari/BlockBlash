using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGridController : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject cellObject;
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector2 gridThreshold;

    public GameObject[,] listCell;

    private void Start()
    {
        listCell = new GameObject[gridSize.x, gridSize.y];

        // Set up grid
        SetGridPosition();
        SpawnGrid();
    }

    void SetGridPosition()
    {
        Vector2 targetPosition = gridThreshold;
        targetPosition.x -= grid.cellSize.x * gridSize.x / 2;
        targetPosition.y -= grid.cellSize.y * gridSize.y / 2;
        grid.transform.localPosition = targetPosition;
    }

    void SpawnGrid()
    {
        for(int x = 0; x < gridSize.x; x++)
        {
            for(int y =0; y < gridSize.y; y++)
            {
                Vector3Int cellPossition = new Vector3Int(x, y);
                Vector2 worldPosition = grid.GetCellCenterWorld(cellPossition);
                GameObject cell = Instantiate(cellObject, worldPosition, Quaternion.identity, grid.transform);
                CellBoard cellBoard = cell.GetComponent<CellBoard>();
                cellBoard.SetGridPosion((Vector2Int)cellPossition);
                cellBoard.IndexCell = 0;
                listCell[x, y] = cell;
            }
        }
    }

    public void ResetGrid()
    {
        foreach(GameObject goj in listCell)
        {
            goj.GetComponent<CellBoard>().IndexCell = 0;
        }
    }
}
