using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBoard : CellController
{
    [ReadOnly]
    public Vector2Int gridPosition;

    public enum StateCell
    {
        HIDE, FADE, CLEAR
    }
    [HideInInspector]
    public StateCell stateCell;
    public StateCell STATECELL
    {
        get { return stateCell; }
        set
        {
            stateCell = value;
            switch (stateCell)
            {
                case StateCell.HIDE:
                    SetStateCell(0);
                    break;
                case StateCell.FADE:
                    SetStateCell(80);
                    break;
                case StateCell.CLEAR:
                    SetStateCell(255);
                    break;
                default:
                    break;
            }
        }
    }

    public override int IndexCell 
    { 
        get => base.IndexCell; 
        set 
        { 
            base.IndexCell = value; 
            STATECELL = base.IndexCell == 0 ? StateCell.HIDE : StateCell.CLEAR; 
        } 
    }

    private void Awake()
    {
        spriteCell = GetComponent<SpriteRenderer>();
    }

    public void SetGridPosion(Vector2Int _gridPosition)
    {
        gridPosition = _gridPosition;
    }
    public void SetStateCell(byte alpha)
    {
        spriteCell.color = new Color32(255, 255, 255, alpha);
    }

    public void SetFadeColor(int indexColor)
    {
        STATECELL = StateCell.FADE;
        spriteCell.sprite = ColorCellManager.Instance.GetSpriteCell(indexColor);
    }
    public void ClearFadeColor()
    {
        IndexCell = 0;
    }

    public void SetFakeColor(int indexColor)
    {
        spriteCell.sprite = ColorCellManager.Instance.GetSpriteCell(indexColor);
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void ClearFakeColor()
    {
        IndexCell = indexCell;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ShowEatEffect()
    {
        transform.DOScale(Vector3.one * 1.15f, 0.15f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine).OnComplete(() => {
            IndexCell = 0;
            ClearFakeColor();
        });
    }
}
