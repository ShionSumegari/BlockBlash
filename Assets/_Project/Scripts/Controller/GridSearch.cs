using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridSearch : MonoBehaviour
{
    [SerializeField] private LoadJson json;

    private int[,] baseGrid;

    private List<int[,]> candidateGrids;

    private SpawnGridController spawnGrid;

    void Start()
    {
        spawnGrid = GameController.Instance.spawnGrid;
        SetUpCandidteGrids();
    }

    public List<(int[,], int)> GridCombination()
    {
        SetUpBaseGrids();
        List<(int[,], int)> rankedCombinations = FindAllCombinations();
        return rankedCombinations;
    }

    public bool GridChecker(int[,] smallGrid)
    {
        SetUpBaseGrids();
        return CanPlaceSmallGrid(baseGrid, smallGrid);
    }

    void SetUpBaseGrids()
    {
        int x = 0;
        int y = 0;
        baseGrid = new int[spawnGrid.listCell.GetLength(1), spawnGrid.listCell.GetLength(0)];
        for (int i = spawnGrid.listCell.GetLength(1) - 1; i >= 0; i--)
        {
            y = 0;
            for(int j = 0; j < spawnGrid.listCell.GetLength(0); j++)
            {
                baseGrid[x, y] = spawnGrid.listCell[j, i].GetComponent<CellBoard>().IndexCell == 0 ? 0 : 1;
                y++;
            }
            x++;
        }
    }

    void SetUpCandidteGrids()
    {
        candidateGrids = new List<int[,]>();
        foreach(GridData data in json.gridCollection.grids)
        {
            candidateGrids.Add(data.data);
        }
    }

    #region COMBINATIONS
    List<(int[,], int)> FindAllCombinations()
    {
        List<(int[,], int)> validCombinations = new List<(int[,], int)>();

        foreach (var grid in candidateGrids)
        {
            int[,] newGrid = TryApplyGrid(baseGrid, grid);
            if (newGrid != null)
            {
                int onesCount = CountFullRowsAndCols(newGrid);
                validCombinations.Add((grid, onesCount));
            }
            else
            {
                validCombinations.Add((grid, -1));
            }
        }
        ShuffleList(validCombinations);
        // Sắp xếp từ cao đến thấp theo số hàng/cột toàn 1
        validCombinations = validCombinations.OrderByDescending(x => x.Item2).ToList();
        return validCombinations;
    }

    int[,] TryApplyGrid(int[,] baseGrid, int[,] smallGrid)
    {
        int[,] newGrid = (int[,])baseGrid.Clone();
        int rows = baseGrid.GetLength(0);
        int cols = baseGrid.GetLength(1);
        int sRows = smallGrid.GetLength(0);
        int sCols = smallGrid.GetLength(1);

        // Duyệt tất cả vị trí đặt hợp lệ
        for (int startX = 0; startX <= rows - sRows; startX++)
        {
            for (int startY = 0; startY <= cols - sCols; startY++)
            {
                if (CanPlaceGrid(newGrid, smallGrid, startX, startY))
                {
                    ApplyGridToPosition(newGrid, smallGrid, startX, startY);
                    return newGrid;
                }
            }
        }

        return null; // Không tìm được vị trí hợp lệ
    }

    bool CanPlaceGrid(int[,] targetGrid, int[,] smallGrid, int startX, int startY)
    {
        int sRows = smallGrid.GetLength(0);
        int sCols = smallGrid.GetLength(1);

        for (int i = 0; i < sRows; i++)
        {
            for (int j = 0; j < sCols; j++)
            {
                if (smallGrid[i, j] == 1 && targetGrid[startX + i, startY + j] == 1)
                {
                    return false; // Ô 1 của danh sách con không được đè lên ô 1 của danh sách 8x8
                }
            }
        }

        return true;
    }

    void ApplyGridToPosition(int[,] targetGrid, int[,] smallGrid, int startX, int startY)
    {
        int sRows = smallGrid.GetLength(0);
        int sCols = smallGrid.GetLength(1);

        for (int i = 0; i < sRows; i++)
        {
            for (int j = 0; j < sCols; j++)
            {
                if (smallGrid[i, j] == 1)
                {
                    targetGrid[startX + i, startY + j] = 1;
                }
            }
        }
    }

    int CountFullRowsAndCols(int[,] grid)
    {
        int count = 0;
        int size = grid.GetLength(0);

        for (int i = 0; i < size; i++)
        {
            bool fullRow = true;
            bool fullCol = true;

            for (int j = 0; j < size; j++)
            {
                if (grid[i, j] != 1) fullRow = false;
                if (grid[j, i] != 1) fullCol = false;
            }

            count += (fullRow ? 1 : 0) + (fullCol ? 1 : 0);
        }

        return count;
    }

    void ShuffleList<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
    #endregion

    #region CHECKER
    bool CanPlaceSmallGrid(int[,] baseGrid, int[,] smallGrid)
    {
        int baseRows = baseGrid.GetLength(0);
        int baseCols = baseGrid.GetLength(1);
        int smallRows = smallGrid.GetLength(0);
        int smallCols = smallGrid.GetLength(1);

        // Duyệt qua từng vị trí có thể đặt danh sách con vào
        for (int startX = 0; startX <= baseRows - smallRows; startX++)
        {
            for (int startY = 0; startY <= baseCols - smallCols; startY++)
            {
                if (CanFitAtPosition(baseGrid, smallGrid, startX, startY))
                {
                    return true; // Tìm được vị trí hợp lệ
                }
            }
        }
        return false; // Không có vị trí hợp lệ nào
    }

    bool CanFitAtPosition(int[,] baseGrid, int[,] smallGrid, int startX, int startY)
    {
        int smallRows = smallGrid.GetLength(0);
        int smallCols = smallGrid.GetLength(1);

        for (int i = 0; i < smallRows; i++)
        {
            for (int j = 0; j < smallCols; j++)
            {
                if (smallGrid[i, j] == 1 && baseGrid[startX + i, startY + j] == 1)
                {
                    return false; // Có ô 1 bị đè lên -> Không hợp lệ
                }
            }
        }
        return true; // Nếu không có ô 1 nào bị đè lên -> Hợp lệ
    }
    #endregion
}
