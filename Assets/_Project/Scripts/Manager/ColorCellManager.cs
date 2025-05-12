using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class CellColor
{
    [ReadOnly]
    public int indexColor;
    public Sprite cellSprite;
}

public class ColorCellManager : Shion.Singleton<ColorCellManager>
{
    public List<CellColor> listCellColor;

    public Sprite GetSpriteCell(int indexCell)
    {
        return listCellColor[indexCell].cellSprite;
    }

    private void OnValidate()
    {
        for(int i = 0; i < listCellColor.Count; i++)
            listCellColor[i].indexColor = i;
    }
}
