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
        float offset = -numUnitTypes * IMAGE_SIZE;

        for (int i = 0; i < numUnitTypes; i++)
        {
            Image image = new GameObject().AddComponent<Image>();
            UnitData unitData = UnitDataLoader.GetUnitData((UnitType)i);
            if (unitData != null) image.sprite = unitData.sprite;

            float wrapAround = 0;
            if(i > numUnitTypes / 2)
            {
                wrapAround = offset;
            }

            Vector2 position = new Vector2(wrapAround + IMAGE_SIZE * i, 0);
            AddImageToCanvas(image, position);
            imageLocations.Add(position);
            unitImages.Add(image);
        }

        selectionImage = new GameObject().AddComponent<Image>();
        selectionImage.sprite = Resources.Load<Sprite>($"Textures/unit_selection");
        selectionImage.color = Color.yellow;
        AddImageToCanvas(selectionImage, new Vector2(0, 0));

        SetSelectedUnitIndex(0);
    }

    private void AddImageToCanvas (Image image, Vector2 position) {
        image.transform.SetParent(canvas.transform, false);
        image.rectTransform.anchorMax = new Vector2(0.5f, 1);
        image.rectTransform.anchorMin = new Vector2(0.5f, 1);
        image.rectTransform.pivot = new Vector2(0.5f, 1);
        image.rectTransform.anchoredPosition = position;
        image.rectTransform.sizeDelta = new Vector2(IMAGE_SIZE, IMAGE_SIZE);
    }

    public void SetSelectedUnitIndex(int index)
    {
        for (int i=0; i<unitImages.Count; i++)
        {
            int currentIndex = index + i;
            if (currentIndex >= unitImages.Count)
            {
                currentIndex -= unitImages.Count;
            }

            float moveDistance = Mathf.Abs(unitImages[currentIndex].rectTransform.anchoredPosition.x - imageLocations[i].x);
            if(moveDistance > IMAGE_SIZE)
            {
                StartCoroutine(FadeAndMove(unitImages[currentIndex], imageLocations[i]));
            }
            else
            {
                StartCoroutine(Slide(unitImages[currentIndex].rectTransform, imageLocations[i]));
            }
        }
    }

    IEnumerator FadeAndMove(Image image, Vector2 targetLocation)
    {
        RectTransform rectTransform = image.rectTransform;
        Vector2 startLocation = rectTransform.anchoredPosition;
        float f = 1f;
        Color color = image.color;
        for (; f > 0f; f -= Time.deltaTime * selectionAnimationSpeed * 2)
        {
            color.a = f;
            image.color = color;
            yield return null;
        }
        rectTransform.anchoredPosition = targetLocation;
        for (; f < 1f; f += Time.deltaTime * selectionAnimationSpeed * 2)
        {
            color.a = f;
            image.color = color;
            yield return null;
        }
        color.a = 1;
        image.color = color;
    }

    IEnumerator Slide(RectTransform rectTransform, Vector2 targetLocation) // TODO: slide behind other images
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









