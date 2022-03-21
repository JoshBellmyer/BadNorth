using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Netcode;
using UnityEngine;

public class TerrainGenerator : NetworkBehaviour
{
    public TerrainSettings settings;

    public static System.Random random;

    public NavMeshSurface surface;

    public MeshFilter navMeshFilter;
    public MeshCollider navMeshCollider;

    public MeshFilter tileMeshFilter;
    public MeshRenderer tileMeshRenderer;

    public GameObject[] otherMeshes;

    NetworkVariable<int> seed = new NetworkVariable<int>();

    public void Start()
    {
        if (Game.isHost || !Application.isPlaying)
        {
            seed.Value = settings.randomizeSeed ? Random.Range(int.MinValue, int.MaxValue) : settings.defaultSeed;
        }

        GenerateMap(seed.Value);
    }

    public void GenerateMap(int seed)
    {

        random = new System.Random(seed);

        float[,] heightMap = GenerateHeightMap(seed);

        MeshData data = settings.flatTilesMesh ? MeshGenerator.GenerateTerrainMeshFlatTiles(heightMap, settings.meshScale) : MeshGenerator.GenerateTerrainMesh(heightMap, settings.meshScale);

        Mesh m = data.CreateMesh();
        navMeshFilter.sharedMesh = m;
        navMeshCollider.sharedMesh = m;

        TileData tileData = new TileData(heightMap, settings.meshScale);
        TileSet tileSet = settings.tileSet;

        tileMeshFilter.mesh = TilePlacer.PlaceTiles(tileData, tileSet, navMeshFilter);
        tileMeshRenderer.material = tileSet.material;

        float offset = (heightMap.GetLength(0) / 2.0f) - 0.5f;
        tileMeshFilter.transform.position = new Vector3(-offset, 0, -offset);

        surface.BuildNavMesh();
        navMeshFilter.GetComponent<MeshRenderer>().enabled = false;
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














