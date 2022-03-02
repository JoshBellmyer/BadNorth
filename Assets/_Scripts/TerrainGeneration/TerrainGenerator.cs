using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public TerrainSettings settings;

    public MeshFilter meshFilter;

    public bool randomizeSeed;
    public int seed;

    public void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        seed = randomizeSeed ? Random.Range(int.MinValue, int.MaxValue) : seed;
        float[,] heightMap = GenerateHeightMap(seed);

        MeshData data = settings.flatTilesMesh ? MeshGenerator.GenerateTerrainMeshFlatTiles(heightMap, settings.meshScale) : MeshGenerator.GenerateTerrainMesh(heightMap, settings.meshScale);

        if(meshFilter == null)
        {
            meshFilter = new GameObject("TerrainMesh").AddComponent<MeshFilter>();
            meshFilter.gameObject.AddComponent<MeshRenderer>();
        }
        meshFilter.sharedMesh = data.CreateMesh();
    }

    public float[,] GenerateHeightMap (int seed) {
        float[,] noise = Noise.GenerateNoiseMap(settings.size, settings.noiseSettings, Vector2.zero, seed);
        AddEffectsToNoise(noise);

        return noise;
    }

    public void AddEffectsToNoise(float[,] noise)
    {
        // flatten after loop
        List<int> toFlattenX = new List<int>();
        List<int> toFlattenY = new List<int>();

        // main loop
        for (int x = 0; x < settings.size; x++)
        {
            for (int y = 0; y < settings.size; y++)
            {
                // island fall off
                float distFromCenterX = Mathf.Abs(x - settings.size / 2);
                float distFromCenterY = Mathf.Abs(y - settings.size / 2);
                float distFromCenter = Mathf.Sqrt(distFromCenterX * distFromCenterX + distFromCenterY * distFromCenterY);
                noise[x, y] *= Mathf.InverseLerp(settings.size / 2, 0, distFromCenter);

                // scale height
                noise[x, y] *= settings.scaleHeight;

                // terraces
                if (settings.applyTerraces)
                {
                    noise[x, y] = Mathf.Floor(noise[x, y] * settings.terraceFrequency) / settings.terraceFrequency;
                }

                // flatten peaks
                if (settings.flattenPeaks && x > settings.flattenPeakLookRange - 1 && x < settings.size - settings.flattenPeakLookRange && y > settings.flattenPeakLookRange - 1 && y < settings.size - settings.flattenPeakLookRange) // if not an edge
                {
                    int lowNeighborCount = 0;
                    for (int i = -settings.flattenPeakLookRange; i < settings.flattenPeakLookRange; i++)
                    {
                        for (int j = -settings.flattenPeakLookRange; j < settings.flattenPeakLookRange; j++)
                        {
                            if (noise[x + i, y + j] < noise[x, y])
                            {
                                lowNeighborCount++;
                            }
                        }
                    }
                    if (lowNeighborCount >= settings.flattenPeaksCutoff)
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
            noise[toFlattenX[i], toFlattenY[i]] -= 1 / (float)settings.terraceFrequency;
        }
    }
}














