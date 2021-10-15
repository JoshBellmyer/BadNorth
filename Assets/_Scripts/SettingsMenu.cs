using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] Text cursorSensitivity;
    [SerializeField] Text zoomSensitivity;
    [SerializeField] Text rotateSensitivity;

    [SerializeField] UIController uiController;
    [SerializeField] Player player;

    private void OnEnable()
    {
        LoadSettingsValues();
        uiController.SelectSomething();
    }

    public void LoadSettingsValues()
    {
        cursorSensitivity.text = player.settings.cursorSensitivity + "";
        zoomSensitivity.text = player.settings.zoomSensitivity + "";
        rotateSensitivity.text = player.settings.rotateSensitivity + "";
    }

    public void SaveSettings()
    {
        player.settings.cursorSensitivity = float.Parse(cursorSensitivity.text);
        player.settings.zoomSensitivity = float.Parse(zoomSensitivity.text);
        player.settings.rotateSensitivity = float.Parse(rotateSensitivity.text);
        player.settings.Save();
    }

    public void Back()
    {
        gameObject.SetActive(false);
        pauseMenu.SetActive(true);
    }
}
