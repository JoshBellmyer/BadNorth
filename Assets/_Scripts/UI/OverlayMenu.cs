using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayMenu : PlayerMenu
{
    [SerializeField] float selectionAnimationSpeed;
    [SerializeField] RectTransform cursorTransform;
    Canvas canvas;
    RectTransform canvasTransform;
    List<Image> unitImages;
    List<Vector2> imageLocations;
    Slider deployCooldownBar;
    private Image selectionImage;

    private static int IMAGE_SIZE = 100;

    private new void Awake()
    {
        base.Awake();
        canvas = GetComponent<Canvas>();
        canvasTransform = GetComponent<RectTransform>();
        SetUpUnitOptionImages();
        deployCooldownBar = transform.Find("DeployCooldown").GetComponent<Slider>();
        player.OnCooldownUpdated += UpdateCooldownBar;
        player.OnSelectedUnitIndexChanged += SetSelectedUnitIndex;
    }

    public void MoveCursor(Vector2 delta)
    {
        if (player == null) return;
        cursorTransform.localPosition += new Vector3(delta.x, delta.y) * player.settings.cursorSensitivity;
        float clampX = Mathf.Clamp(cursorTransform.localPosition.x, -canvasTransform.rect.width / 2, canvasTransform.rect.width / 2);
        float clampY = Mathf.Clamp(cursorTransform.localPosition.y, -canvasTransform.rect.height / 2, canvasTransform.rect.height / 2);
        cursorTransform.localPosition = new Vector3(clampX, clampY);
    }

    public Tuple<bool, RaycastHit> CastFromCursor(int layerMask)
    {
        RaycastHit hit;
        // Debug.Log(canvasTransform);
        Vector3 cursorPosition = cursorTransform.localPosition + new Vector3(canvasTransform.rect.width / 2, canvasTransform.rect.height / 2);
        Vector3 viewPortPosition = new Vector3(cursorPosition.x / canvasTransform.rect.width, cursorPosition.y / canvasTransform.rect.height); // view port is normalized
        Ray cursorSelectRay = player.camera.ViewportPointToRay(viewPortPosition);

        bool hitSomething = Physics.Raycast(cursorSelectRay, out hit, Mathf.Infinity, layerMask);

        return new Tuple<bool, RaycastHit>(hitSomething, hit);
    }

    private void UpdateCooldownBar(float value, float total)
    {
        deployCooldownBar.value = 1f - value / total;
    }

    void SetUpUnitOptionImages()
    {
        unitImages = new List<Image>();
        imageLocations = new List<Vector2>();
        int numUnitTypes = Enum.GetValues(typeof(UnitType)).Length;
        float offset = -2 * IMAGE_SIZE;

        for (int i = 0; i < numUnitTypes; i++)
        {
            Image image = new GameObject().AddComponent<Image>();
            UnitData unitData = UnitDataLoader.GetUnitData((UnitType)i);
            if (unitData != null) image.sprite = unitData.sprite;

            Vector2 position = new Vector2(offset + IMAGE_SIZE * i, 0);
            AddImageToCanvas(image);
            imageLocations.Add(position);
            unitImages.Add(image);
        }

        selectionImage = new GameObject().AddComponent<Image>();
        selectionImage.sprite = Resources.Load<Sprite>($"Textures/unit_selection");
        selectionImage.color = Color.white;
        AddImageToCanvas(selectionImage);
        selectionImage.rectTransform.anchoredPosition = new Vector2(0, 0);
        selectionImage.transform.localScale = new Vector3(1.1f, 1.1f, 1);

        SetSelectedUnitIndex(0);
    }

    private void AddImageToCanvas (Image image) {
        image.transform.SetParent(canvas.transform, false);
        image.rectTransform.anchorMax = new Vector2(0.5f, 1);
        image.rectTransform.anchorMin = new Vector2(0.5f, 1);
        image.rectTransform.pivot = new Vector2(0.5f, 1);
        image.rectTransform.sizeDelta = new Vector2(IMAGE_SIZE, IMAGE_SIZE);
    }

    public void SetSelectedUnitIndex(int index)
    {
        for (int i=0; i<unitImages.Count; i++)
        {
            int currentIndex = index + i - 2;
            if (currentIndex >= unitImages.Count)
            {
                currentIndex -= unitImages.Count;
            }
            if (currentIndex < 0)
            {
                currentIndex += unitImages.Count;
            }

            if(i == 1 || i == 2 || i == 3)
            {
                StartCoroutine(FadeIn(unitImages[currentIndex]));
            }
            else
            {
                StartCoroutine(FadeOut(unitImages[currentIndex]));
            }

            StartCoroutine(Slide(unitImages[currentIndex].rectTransform, imageLocations[i]));
        }
    }

    IEnumerator FadeIn(Image image)
    {
        Color color = image.color;
        if(color.a == 1)
        {
            yield break;
        }

        for (float f = 0; f < 1f; f += Time.deltaTime * selectionAnimationSpeed * 2)
        {
            color.a = f;
            image.color = color;
            yield return null;
        }
        color.a = 1;
        image.color = color;
    }

    IEnumerator FadeOut(Image image)
    {
        Color color = image.color;
        if (color.a == 0)
        {
            yield break;
        }

        for (float f = 1; f > 0f; f -= Time.deltaTime * selectionAnimationSpeed * 2)
        {
            color.a = f;
            image.color = color;
            yield return null;
        }
        color.a = 0;
        image.color = color;
    }

    IEnumerator Slide(RectTransform rectTransform, Vector2 targetLocation)
    {
        Vector2 startLocation = rectTransform.anchoredPosition;
        for(float f = 0f; f < 1f; f += Time.deltaTime * selectionAnimationSpeed)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(startLocation, targetLocation, f);
            yield return null;
        }
        rectTransform.anchoredPosition = targetLocation;
    }
}









