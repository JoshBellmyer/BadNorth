using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : PlayerMenu
{
    public void OnMainMenuButton()
    {
        Game.instance.Unpause();
        Game.instance.SwitchToMainMenu();
    }

    public void OnPlayButton()
    {
        Game.instance.Unpause();
        playerUIManager.SwitchMenu(typeof(OverlayMenu));
    }

    public void OnSettingsButton()
    {
        playerUIManager.SwitchMenu(typeof(SettingsMenu));
    }
}
