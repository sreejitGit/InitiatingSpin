using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : BaseScreenUI
{
    public override void Show()
    {
        base.Show();
        playButtonPressed = false;
    }

    public override void Hide()
    {
        base.Hide();
    }

    bool playButtonPressed = false;
    public void PlayButtonPressed()
    {
        if (playButtonPressed)
        {
            return;
        }
        playButtonPressed = true;
        GameEvents.EnterGame();
    }

    public void ResetSavedData()
    {
        GameManager.ResetSavedData();
    }
}
