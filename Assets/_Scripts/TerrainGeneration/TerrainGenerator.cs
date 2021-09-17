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
    [SerializeField] float cornerFallOffDistance;
    [SerializeField] bool applyTerraces;
    [SerializeField] int terraceFrequency;
    [SerializeField] bool flattenPeaks;
    [SerializeField] [Range(0,8)] int flattenPeaksCutoff;

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
        // flatten after loop
        List<int> toFlattenX = new List<int>();
        List<int> toFlattenY = new List<int>();

        // main loop
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                // island fall off
                float distFromCenterX = Mathf.Abs(x - size / 2);
                float distFromCenterY = Mathf.Abs(y - size / 2);
                float distFromCenter = Mathf.Sqrt(distFromCenterX * distFromCenterX + distFromCenterY * distFromCenterY);
                noise[x, y] *= Mathf.InverseLerp(size / 2, 0, distFromCenter);

                // corner fall off
                if (applyCornerFallOff)
                {
                    noise[x, y] *= Mathf.InverseLerp(cornerFallOffDistance, 0, x);
                    noise[x, y] *= Mathf.InverseLerp(cornerFallOffDistance, 0, y);
                }

                // scale height
                noise[x, y] *= scaleHeight;

                // terraces
                if (applyTerraces)
                {
                    noise[x, y] = Mathf.Floor(noise[x, y] * terraceFrequency) / terraceFrequency;
                }

                // flatten peaks
                if (flattenPeaks && x != 0 && x != size - 1 && y != 0 && y != size - 1)
                {
                    int lowNeighborCount = 0;
                    for (int i = -1; i < 1; i++)
                    {
                        for (int j = -1; j < 1; j++)
                        {
                            if (noise[x + i, y + j] <= noise[x, y])
                            {
                                lowNeighborCount++;
                            }
                        }
                    }
                    if (lowNeighborCount > flattenPeaksCutoff)
                    {
                        toFlattenX.Add(x); // flatten later
                        toFlattenY.Add(y);
                    }
                }
            }
        }

        // flatten now
        for(int i=0; i<toFlattenX.Count; i++)
        {
            noise[toFlattenX[i], toFlattenY[i]] -= 1 / (float)terraceFrequency;
        }
    }
}
