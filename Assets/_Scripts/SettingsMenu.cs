using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : PlayerMenu
{
    [SerializeField] Text cursorSensitivity;
    [SerializeField] Text zoomSensitivity;
    [SerializeField] Text rotateSensitivity;

    const float CURSOR_SENSITIVITY_STEP = 0.1f;
    const float ZOOM_SENSITIVITY_STEP = 0.01f;
    const float ROTATE_SENSITIVITY_STEP = 5f;

    protected new void Start()
    {
        base.Start();
        LoadSettingsValues();
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
        playerUIManager.SwitchMenu(typeof(PauseMenu));
        LoadSettingsValues(); // revert changes if neccessary
    }

    public void IncreaseCursorSensitivity()
    {
        cursorSensitivity.text = float.Parse(cursorSensitivity.text) + CURSOR_SENSITIVITY_STEP + "";
    }

    public void DecreaseCursorSensitivity()
    {
        cursorSensitivity.text = float.Parse(cursorSensitivity.text) - CURSOR_SENSITIVITY_STEP + "";
    }

    public void IncreaseZoomSensitivity()
    {
        zoomSensitivity.text = float.Parse(zoomSensitivity.text) + ZOOM_SENSITIVITY_STEP + "";
    }

    public void DecreaseZoomSensitivity()
    {
        zoomSensitivity.text = float.Parse(zoomSensitivity.text) - ZOOM_SENSITIVITY_STEP + "";
    }

    public void IncreaseRotateSensitivity()
    {
        rotateSensitivity.text = float.Parse(rotateSensitivity.text) + ROTATE_SENSITIVITY_STEP + "";
    }

    public void DecreaseRotateSensitivity()
    {
        rotateSensitivity.text = float.Parse(rotateSensitivity.text) - ROTATE_SENSITIVITY_STEP + "";
    }
}
