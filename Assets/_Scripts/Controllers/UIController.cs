using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] List<string> unitOptions; // TODO: determine type

    List<Image> unitImages;

    PlayerController playerController;
    private MultiplayerEventSystem eventSystem;

    string SelectedUnitType
    {
        get => unitOptions[_selectedUnitIndex];
    }

    int SelectedUnitIndex
    {
        get => _selectedUnitIndex;
        set
        {
            _selectedUnitIndex = value;
            if (_selectedUnitIndex >= unitOptions.Count)
            {
                _selectedUnitIndex = 0;
            }
            if (_selectedUnitIndex < 0)
            {
                _selectedUnitIndex = unitOptions.Count - 1;
            }

            foreach(Image image in unitImages)
            {
                image.color = Color.white;
            }

            unitImages[_selectedUnitIndex].color = Color.yellow;
        }
    }

    int _selectedUnitIndex;

    private void Start()
    {
        SetUpUnitOptionImages();

        playerController = GetComponent<PlayerController>();
        SelectedUnitIndex = 0;
        eventSystem = GetComponent<MultiplayerEventSystem>();
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    void SetUpUnitOptionImages()
    {
        unitImages = new List<Image>();

        int imageSize = 100;
        float offset = -unitOptions.Count * imageSize / 2f;
        for(int i=0; i<unitOptions.Count; i++)
        {
            Image image = new GameObject().AddComponent<Image>(); // TODO: Add real images
            image.transform.SetParent(canvas.transform, false);
            image.transform.localPosition = new Vector3(offset + imageSize * i, 150, 0);
            image.rectTransform.sizeDelta = new Vector2(imageSize, imageSize);
            image.rectTransform.pivot = Vector2.zero;
            image.name = "Unit Selection";

            unitImages.Add(image);
        }
    }

    public void OnDeployUnit(InputAction.CallbackContext context)
    {
        if (Game.instance.IsPlayerRegistered(playerController))
        {
            if (context.performed)
            {
                Debug.Log("Deploying: " + SelectedUnitType);
                playerController.Boat.SetSail();
            }
        }
    }

    public void OnUnitSelectChange(InputAction.CallbackContext context)
    {
        if (Game.instance.IsPlayerRegistered(playerController))
        {
            if (context.performed)
            {
                SelectedUnitIndex += (int)context.ReadValue<float>();
            }
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (Game.instance.IsPlayerRegistered(playerController)) // Accounts for Unity bug, see https://forum.unity.com/threads/player-input-manager-adds-an-extra-player-with-index-1.1039000/
        {
            if (context.performed)
            {
                Game.instance.Pause();

                playerController.SetControlsActivated(true);
                playerController.SetActionMap("UI");
                pauseMenu.SetActive(true);

                eventSystem.SetSelectedGameObject(FindObjectOfType<Selectable>().gameObject, new BaseEventData(eventSystem));
            }
        }
    }

    public void OnUnpause(InputAction.CallbackContext context)
    {
        if (Game.instance.IsPlayerRegistered(playerController))
        {
            if (context.performed)
            {
                Game.instance.Unpause();
                ClearUI();
            }
        }
    }

    public void ClearUI()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }
}
