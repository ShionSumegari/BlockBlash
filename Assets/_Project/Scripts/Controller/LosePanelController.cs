using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LosePanelController : MonoBehaviour
{
    public void ShowPanel()
    {
        gameObject.SetActive(true);
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
}
