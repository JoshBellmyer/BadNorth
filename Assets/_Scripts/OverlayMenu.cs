using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayMenu : PlayerMenu
{
    Canvas canvas;
    List<Image> unitImages;

    public bool unitsVisible;

    protected new void Start()
    {
        base.Start();
        canvas = GetComponent<Canvas>();
        SetUpUnitOptionImages();
    }

    void SetUpUnitOptionImages()
    {
        unitImages = new List<Image>();

        int imageSize = 100;
        float offset = -player.unitOptions.Count * imageSize / 2f;
        for (int i = 0; i < player.unitOptions.Count; i++)
        {
            Image image = new GameObject().AddComponent<Image>(); // TODO: Add real images
            image.transform.SetParent(canvas.transform, false);
            image.transform.localPosition = new Vector3(offset + imageSize * i, 150, 0);
            image.rectTransform.sizeDelta = new Vector2(imageSize, imageSize);
            image.rectTransform.pivot = Vector2.zero;
            image.name = "Unit Selection";

            unitImages.Add(image);
        }
        SetSelectedUnitIndex(0);

        unitsVisible = true;
    }

    public void SetSelectedUnitIndex(int index)
    {
        foreach (Image image in unitImages)
        {
            image.color = Color.white;
        }

        unitImages[index].color = Color.yellow;
    }

    public void SetUnitsVisible (bool visible) {
        foreach (Image image in unitImages) {
            image.gameObject.SetActive(visible);
        }

        unitsVisible = visible;
    }
}
