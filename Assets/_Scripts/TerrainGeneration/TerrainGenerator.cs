using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField]
    NoiseSettings noiseSettings;

    public bool autoUpdate;

    [SerializeField]
    Material mapMaterial;

    float[,] noise;

    public void GenerateMap()
    {
        noise = Noise.GenerateNoiseMap(10, noiseSettings, Vector2.zero);
        Texture texture = TextureGenerator.TextureFromNoiseMap(noise);

        mapMaterial.mainTexture = texture;

    }
}
