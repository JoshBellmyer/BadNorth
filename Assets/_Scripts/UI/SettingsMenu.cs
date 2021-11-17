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

    float cursorSensitivityVisual;
    float zoomSensitivityVisual;
    float rotateSensitivityVisual;

    protected new void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        LoadSettingsValues();
    }

    public void LoadSettingsValues()
    {
        cursorSensitivityVisual = player.settings.cursorSensitivity;
        zoomSensitivityVisual = player.settings.zoomSensitivity;
        rotateSensitivityVisual = player.settings.rotateSensitivity;
        cursorSensitivity.text = string.Format("{0:0.00}", cursorSensitivityVisual);
        zoomSensitivity.text = string.Format("{0:0.00}", zoomSensitivityVisual);
        rotateSensitivity.text = string.Format("{0:0.00}", rotateSensitivityVisual);
    }

    public void SaveSettings()
    {
        player.settings.cursorSensitivity = cursorSensitivityVisual;
        player.settings.zoomSensitivity = zoomSensitivityVisual;
        player.settings.rotateSensitivity = rotateSensitivityVisual;
        player.settings.Save();
    }

    public void Back()
    {
        playerUIManager.SwitchMenu(typeof(PauseMenu));
        LoadSettingsValues(); // revert changes if neccessary
    }

    public void IncreaseCursorSensitivity()
    {
        cursorSensitivityVisual += CURSOR_SENSITIVITY_STEP;
        cursorSensitivity.text = string.Format("{0:0.00}", cursorSensitivityVisual);
    }

    public void DecreaseCursorSensitivity()
    {
        cursorSensitivityVisual -= CURSOR_SENSITIVITY_STEP;
        cursorSensitivity.text = string.Format("{0:0.00}", cursorSensitivityVisual);
    }

    public void IncreaseZoomSensitivity()
    {
        zoomSensitivityVisual += ZOOM_SENSITIVITY_STEP;
        zoomSensitivity.text = string.Format("{0:0.00}", zoomSensitivityVisual);
    }

    public void DecreaseZoomSensitivity()
    {
        zoomSensitivityVisual -= ZOOM_SENSITIVITY_STEP;
        zoomSensitivity.text = string.Format("{0:0.00}", zoomSensitivityVisual);
    }

    public void IncreaseRotateSensitivity()
    {
        rotateSensitivityVisual += ROTATE_SENSITIVITY_STEP;
        rotateSensitivity.text = string.Format("{0:0.00}", rotateSensitivityVisual);
    }

    public void DecreaseRotateSensitivity()
    {
        rotateSensitivityVisual -= ROTATE_SENSITIVITY_STEP;
        rotateSensitivity.text = string.Format("{0:0.00}", rotateSensitivityVisual);
    }
}
