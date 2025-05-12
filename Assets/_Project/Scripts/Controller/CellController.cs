using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CellController : MonoBehaviour
{
    [HideInInspector] public SpriteRenderer spriteCell;
    [HideInInspector] public int indexCell;
    public virtual int IndexCell
    {
        get { return indexCell; }
        set
        {
            indexCell = value;
            spriteCell.sprite = ColorCellManager.Instance.GetSpriteCell(indexCell);
        }
    }

}
