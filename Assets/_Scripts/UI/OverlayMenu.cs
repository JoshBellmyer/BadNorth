using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayMenu : PlayerMenu
{
    Canvas canvas;
    List<Image> unitImages;

    protected new void Start()
    {
        base.Start();
        canvas = GetComponent<Canvas>();
        SetUpUnitOptionImages();
    }

    void SetUpUnitOptionImages()
    {
        unitImages = new List<Image>();
        int numUnitTypes = Enum.GetValues(typeof(UnitType)).Length;

        int imageSize = 100;
        float offset = -numUnitTypes * imageSize / 2f;
        for (int i = 0; i < numUnitTypes; i++)
        {
            Image image = new GameObject().AddComponent<Image>(); // TODO: Add real images
            image.transform.SetParent(canvas.transform, false);
            image.rectTransform.anchorMax = new Vector2(0.5f, 1);
            image.rectTransform.anchorMin = new Vector2(0.5f, 1);
            image.rectTransform.pivot = new Vector2(0, 1);
            image.rectTransform.anchoredPosition = new Vector2(offset + imageSize * i, 0);
            image.rectTransform.sizeDelta = new Vector2(imageSize, imageSize);
            image.name = "Unit Selection";

            unitImages.Add(image);
        }
        SetSelectedUnitIndex(0);
    }

    public void SetSelectedUnitIndex(int index)
    {
        foreach (Image image in unitImages)
        {
            image.color = Color.white;
        }

        unitImages[index].color = Color.yellow;
    }
}
