using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Range(2, 24)]
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
    [SerializeField] int flattenPeaksCutoff;
    [SerializeField] int flattenPeakLookRange;

    [SerializeField] Material material;
    [SerializeField] public MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRender;
    [SerializeField] MeshCollider meshCollider;
    [SerializeField] public float meshScale;

    [SerializeField] bool flatTilesMesh;

    [SerializeField] bool autoUpdate;



    public void GenerateMap()
    {
        noiseSettings.seed = Random.Range(10, 5000);
        float[,] noise = GenerateMapNoise();

        Texture texture = TextureGenerator.TextureFromNoiseMap(noise);
        material.mainTexture = texture;

        Mesh mesh = flatTilesMesh ? MeshGenerator.GenerateTerrainMeshFlatTiles(noise, meshScale).CreateMesh() : MeshGenerator.GenerateTerrainMesh(noise, meshScale).CreateMesh();
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
        meshRender.sharedMaterial.mainTexture = texture;
    }

    public float[,] GenerateMapNoise () {
        float[,] noise = Noise.GenerateNoiseMap(size, noiseSettings, Vector2.zero);
        AddEffectsToNoise(noise);

        return noise;
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
                if (flattenPeaks && x > flattenPeakLookRange - 1 && x < size - flattenPeakLookRange && y > flattenPeakLookRange - 1 && y < size - flattenPeakLookRange) // if not an edge
                {
                    int lowNeighborCount = 0;
                    for (int i = -flattenPeakLookRange; i < flattenPeakLookRange; i++)
                    {
                        for (int j = -flattenPeakLookRange; j < flattenPeakLookRange; j++)
                        {
                            if (noise[x + i, y + j] < noise[x, y])
                            {
                                lowNeighborCount++;
                            }
                        }
                    }
                    if (lowNeighborCount >= flattenPeaksCutoff)
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














