using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] List<string> unitOptions; // TODO: determine type

    List<Image> unitImages;

    PlayerController playerController;

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
    }

    void SetUpUnitOptionImages()
    {
        unitImages = new List<Image>();

        return;

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

    public void OnUnpause(InputAction.CallbackContext context)
    {
        if (Game.instance.IsPlayerRegistered(playerController))
        {
            if (context.performed)
            {
                Game.instance.Unpause();
            }
        }
    }
}
