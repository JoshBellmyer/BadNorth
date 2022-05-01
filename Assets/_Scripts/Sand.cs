using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Sand : NetworkBehaviour
{
    [SerializeField] private float risingSpeed;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private MeshRenderer waveRenderer;

    public static float height;
    private float textureOffset;


    // Start is called before the first frame update
    void Start()
    {
        Clock.instance.clockFinished += OnClockFinished;
    }

    public void SetColor(Color c)
    {
        waveRenderer.material.color = c;
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

        waveRenderer.material.SetTextureOffset("_MainTex", new Vector2(0, textureOffset));
    }

    private void OnClockFinished()
    {
        StartCoroutine("RaiseSand");
    }


    public IEnumerator RaiseSand()
    {
        if (!Game.online || Game.isHost)
        {
            while (true)
            {
                if (!Game.instance.isPaused)
                {
                    transform.position += new Vector3(0, risingSpeed * Time.deltaTime, 0);
                }

                yield return null;
            }
        }
        else
        {
            Debug.LogWarning("Raising sand as a client is not allowed");
        }
    }
}
