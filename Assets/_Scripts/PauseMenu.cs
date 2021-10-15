using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject settingsMenu;
    [SerializeField] UIController uiController;

    private void OnEnable()
    {
        uiController.SelectSomething();
    }

    public void OnExitGameButton()
    {
        Game.instance.ExitGame();
    }

    public void OnPlayButton()
    {
        Game.instance.Unpause();
        gameObject.SetActive(false);
    }

    public void OnSettingsButton()
    {
        settingsMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}
