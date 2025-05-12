using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDrag : CellController
{
    public LayerMask cellMask;
    [SerializeField] Sprite grayBlock;
    [HideInInspector] public bool checking;
    public bool Checking
    {
        get { return checking; }
        set
        {
            checking = value;
            spriteCell.sortingOrder = checking ? 2 : 1;
        }
    }

    [HideInInspector] public Transform curentCell;
    public Transform CurentCell
    {
        get { return curentCell; }
        set
        {
            if(curentCell != value)
            {
                curentCell = value;
                GameController.Instance.HandleListCell();
            }
        }
    }
    private void Awake()
    {
        spriteCell = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Checking)
        {
            CheckRaycastCell();
        }
    }

    void CheckRaycastCell()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero, Mathf.Infinity, cellMask);

        if (hit.collider != null)
        {
            CurentCell = hit.transform;
        }
        else
        {
            CurentCell = null;
        }
    }

    public void ShowGrayBlock()
    {
        spriteCell.sprite = grayBlock;
    }

    public void ClearGrayBlock()
    {
        IndexCell = indexCell;
    }
}
