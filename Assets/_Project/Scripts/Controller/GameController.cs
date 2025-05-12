using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class GameController : Shion.Singleton<GameController>
{
    /// <summary>
    /// 
    /// </summary>

    public GridSearch gridSearch;
    public SpawnGridController spawnGrid;
    public Transform blockSpawner;
    public LayerMask blockMask;
    [SerializeField] private Vector2 touchBlockThreshold;
    [ReadOnly] public Transform currentBlock;
    [ReadOnly] public List<CellBoard> listCellTarget;

    protected Vector3 startPosBlock;
    protected int indexColor;
    protected float scaleBlock;
    protected List<CellBoard> allRowsColums;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckRaycastBlock();
        }
        if (Input.GetMouseButton(0))
        {
            if(currentBlock != null)
            {
                Vector2 newPosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) + touchBlockThreshold;
                currentBlock.position = newPosition;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (currentBlock != null)
            {
               
                if(listCellTarget.Count > 0)
                {
                    currentBlock.DOMove(TargetCell(), 0.1f).SetEase(Ease.InOutSine).OnComplete(() => { SetCell();EatAllRowsAndColums(); ResetState();  });
                }
                else
                {
                    currentBlock.DOScale(Vector3.one * scaleBlock, 0.2f).SetEase(Ease.InOutSine);
                    currentBlock.DOMove(startPosBlock, 0.2f).SetEase(Ease.InOutSine).OnComplete(()=> { SetCheckingBlock(false); ResetState(); });
                }
            }
        }
    }
    
    void CheckRaycastBlock()
    {
        Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero, Mathf.Infinity, blockMask);

        if (hit.collider != null)
        {
            BlockParameter blockParameter = hit.transform.parent.GetComponent<BlockParameter>();
            if (blockParameter.IsGray) return;
            indexColor = hit.transform.GetComponent<BlockDrag>().IndexCell;
            currentBlock = hit.transform.parent;
            scaleBlock = currentBlock.GetComponent<BlockParameter>().scale;
            startPosBlock = currentBlock.GetComponent<BlockParameter>().position;

            Vector2 newPosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) + touchBlockThreshold;
            currentBlock.position = newPosition;
            currentBlock.DOScale(Vector3.one, 0.1f).SetEase(Ease.InOutSine);

            SetCheckingBlock(true);
        }
    }

    void SetCheckingBlock(bool check)
    {
        if(currentBlock != null)
        {
            foreach (Transform tr in currentBlock)
                tr.GetComponent<BlockDrag>().Checking = check;
        }
    }

    public void HandleListCell()
    {
        // Clear fade and fake color of each cell   
        if(listCellTarget.Count > 0)
        {
            foreach(CellBoard cell in listCellTarget)
            {
                cell.ClearFadeColor();
            }

            //Clear Rows and Colums
            if (allRowsColums.Count > 0)
            {
                foreach (CellBoard cell in allRowsColums)
                    cell.ClearFakeColor();
            }
            allRowsColums.Clear();
        }
        // Reset cell
        listCellTarget = new List<CellBoard>();
        // Handle list cell
        foreach(Transform tr in currentBlock)
        {
            Transform cell = tr.GetComponent<BlockDrag>().CurentCell;
            if (cell != null && cell.GetComponent<CellBoard>().IndexCell == 0)
            {
                listCellTarget.Add(cell.GetComponent<CellBoard>());
            }
            else
            {
                listCellTarget.Clear();
                break;
            }
        }
        if(listCellTarget.Count > 0)
        {
            foreach (CellBoard cell in listCellTarget)
                cell.SetFadeColor(indexColor);

            // Set all Rows and Colums
            allRowsColums = AllRowsAndColums();
            if(allRowsColums.Count > 0)
            {
                foreach (CellBoard cell in allRowsColums)
                    if (cell.STATECELL != CellBoard.StateCell.FADE)
                        cell.SetFakeColor(indexColor);
            }
        }
    } 

    Vector3 TargetCell()
    {
        Vector3 targetMove = Vector3.zero;
        foreach(CellBoard cell in listCellTarget)
            targetMove += cell.transform.position;
        targetMove /= listCellTarget.Count;
        return targetMove;
    }

    void SetCell()
    {
        Destroy(currentBlock.gameObject);
        foreach (CellBoard cell in listCellTarget)
            cell.IndexCell = indexColor;
    }

    void ResetState()
    {
        indexColor = 0; 
        currentBlock = null;
        listCellTarget.Clear();
    }

    void EatAllRowsAndColums()
    {
        if (allRowsColums.Count > 0)
        {
            foreach (CellBoard cell in allRowsColums)
            {
                cell.SetFakeColor(indexColor);
                cell.ShowEatEffect();
            }
            Invoke(nameof(CheckSpawnBlock), 0.4f);
        }
        else
            Invoke(nameof(CheckSpawnBlock), 0.2f);
       allRowsColums.Clear();
    }

    void CheckSpawnBlock()
    {
        blockSpawner.GetComponent<BlockSpawner>().CheckSpawnBlock();
    }

    List<CellBoard> AllRowsAndColums()
    {
        List<CellBoard> allRowsColums = new List<CellBoard>();

        List<CellBoard> listHorizontal = ListHorizontal();
        List<CellBoard> listVertical = ListVertical(); 

        //Get all rows
        foreach(CellBoard cell in listHorizontal)
        {
            List<CellBoard> listClone = new List<CellBoard>();
            for(int i = 0; i < spawnGrid.listCell.GetLength(1); i++)
            {
                CellBoard _cell = spawnGrid.listCell[i, cell.gridPosition.y].GetComponent<CellBoard>();
                if (_cell.IndexCell != 0 || _cell.STATECELL == CellBoard.StateCell.FADE)
                    listClone.Add(_cell);
                else
                {
                    listClone.Clear();
                    break;
                }

            }
            foreach (CellBoard cellClone in listClone)
                allRowsColums.Add(cellClone);
        }

        //Get all colums
        foreach (CellBoard cell in listVertical)
        {
            List<CellBoard> listClone = new List<CellBoard>();
            for (int i = 0; i < spawnGrid.listCell.GetLength(0); i++)
            {
                CellBoard _cell = spawnGrid.listCell[cell.gridPosition.x, i].GetComponent<CellBoard>();
                if (_cell.IndexCell != 0 || _cell.STATECELL == CellBoard.StateCell.FADE)
                    listClone.Add(_cell);
                else
                {
                    listClone.Clear();
                    break;
                }

            }
            foreach (CellBoard cellClone in listClone)
                allRowsColums.Add(cellClone);
        }
        return allRowsColums;
    }

    List<CellBoard> ListHorizontal()
    {
        List<CellBoard> listHorizontal = new List<CellBoard>();
        foreach(CellBoard cell in listCellTarget)
        {
            if (!listHorizontal.Any(x => x.gridPosition.y == cell.gridPosition.y))
                listHorizontal.Add(cell);
        }
        return listHorizontal;
    }

    List<CellBoard> ListVertical()
    {
        List<CellBoard> listVertical = new List<CellBoard>();
        foreach (CellBoard cell in listCellTarget)
        {
            if (!listVertical.Any(y => y.gridPosition.x == cell.gridPosition.x))
                listVertical.Add(cell);
        }
        return listVertical;
    }

    public void CheckBlock()
    {
        bool checkLose = true;
        foreach(Transform tr in blockSpawner)
        {
            if (gridSearch.GridChecker(tr.GetComponent<BlockParameter>().grids))
            {
                tr.GetComponent<BlockParameter>().GrayStatus(false);
                checkLose = false;
            }
            else tr.GetComponent<BlockParameter>().GrayStatus(true);
        }
        if (checkLose) UiController.Instance.ShowLosePanel();
    }

    public void ResetGame()
    {
        spawnGrid.ResetGrid();
        blockSpawner.GetComponent<BlockSpawner>().ResetBlock();
    }
}
