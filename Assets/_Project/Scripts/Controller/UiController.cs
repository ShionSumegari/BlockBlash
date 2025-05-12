using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiController : Shion.Singleton<UiController>
{
    public LosePanelController losePanelController;

    public void ShowLosePanel()
    {
        losePanelController.ShowPanel();
    }

    public void ClickHomeButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ClickReplayButton()
    {
        GameController.Instance.ResetGame();
        losePanelController.HidePanel();
    }
}
