using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Range(2, 16)]
    [SerializeField]
    int size;
    [SerializeField]
    NoiseSettings noiseSettings;

    [SerializeField] float scaleHeight;
    [SerializeField] bool applyCornerFallOff;
    [SerializeField] float cornerFallOffPower;
    [SerializeField] bool applyTerraces;
    [SerializeField] int terraceFrequency;

    public bool autoUpdate;

    [SerializeField] Material material;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRender;

    public void GenerateMap()
    {
        float[,] noise = Noise.GenerateNoiseMap(size, noiseSettings, Vector2.zero);
        AddEffectsToNoise(noise);

        Texture texture = TextureGenerator.TextureFromNoiseMap(noise);
        material.mainTexture = texture;

        meshFilter.sharedMesh = MeshGenerator.GenerateTerrainMesh(noise).CreateMesh();
        meshRender.sharedMaterial.mainTexture = texture;
    }

    public void AddEffectsToNoise(float[,] noise)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float effect = 1;

                // island fall off
                float distFromCenterX = Mathf.Abs(x - size / 2);
                float distFromCenterY = Mathf.Abs(y - size / 2);
                float distFromCenter = Mathf.Sqrt(distFromCenterX * distFromCenterX + distFromCenterY * distFromCenterY);
                effect *= Mathf.InverseLerp(size / 2, 0, distFromCenter);

                // corner fall off
                if (applyCornerFallOff)
                {
                    effect *= Mathf.Pow(Mathf.InverseLerp(size, 0, x), cornerFallOffPower);
                    effect *= Mathf.Pow(Mathf.InverseLerp(0, size, y), cornerFallOffPower);
                }

                // scale height
                effect *= Mathf.Pow(scaleHeight, applyCornerFallOff ? cornerFallOffPower : 1);

                // terraces
                if (applyTerraces)
                {
                    effect = Mathf.Floor(effect * terraceFrequency) / terraceFrequency;
                }

                noise[x, y] *= effect;
            }
        }
    }
}
