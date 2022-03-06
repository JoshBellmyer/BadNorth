using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sand : MonoBehaviour
{
    [SerializeField] private float risingSpeed;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private MeshRenderer waveRenderer;
    private Material waveMaterial;

    public static float height;
    private float textureOffset;

    // Start is called before the first frame update
    void Start()
    {
        Clock.instance.clockFinished += OnClockFinished;
        TileSet tileSet = Game.instance.terrainSettings.tileSet;
        waveMaterial = waveRenderer.material;

        waveMaterial.color = tileSet.sandColor;
    }

    private void Update () {
        height = transform.position.y - 0.5f;

        if (Game.instance.isPaused) {
            return;
        }

        textureOffset += (Time.deltaTime * scrollSpeed);

        if (textureOffset > 1) {
            textureOffset -= 1;
        }

        waveMaterial.SetTextureOffset("_MainTex", new Vector2(0, textureOffset));
    }

    private void OnClockFinished()
    {
        StartCoroutine("RaiseSand");
    }


    public IEnumerator RaiseSand()
    {
        while (true)
        {
            if (!Game.instance.isPaused) {
                transform.position += new Vector3(0, risingSpeed * Time.deltaTime, 0);
            }
            
            yield return null;
        }
    }
}
