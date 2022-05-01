using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TerrainGenerator : NetworkBehaviour, ISavable
{
    public TerrainSettings settings;
    public bool randomizeSeed;
    public int seed;
    public string tileSetName;
    private TileSet tileSet;

    public static System.Random random;
    private int generateMap;
    private int onlineSeed;
    private string onlineTileSetName;

    public NavMeshSurface surface;

    public MeshFilter navMeshFilter;
    public MeshCollider navMeshCollider;

    public MeshFilter tileMeshFilter;
    public MeshRenderer tileMeshRenderer;

    public GameObject[] otherMeshes;

    public void SetUpMap()
    {
        random = new System.Random(seed);
        LoadTileSet();

        if (Game.isHost && Game.online)
        {
            seed = randomizeSeed ? Random.Range(int.MinValue, int.MaxValue) : seed;
            SendGenerationInfoClientRpc(seed, tileSetName);
        }
        else if (!Game.online || !Application.isPlaying)
        {
            Debug.Log("single...");
            seed = randomizeSeed ? Random.Range(int.MinValue, int.MaxValue) : seed;
            StartCoroutine(GenerateMap(seed, tileSetName));
        }

        StartCoroutine(FindSandCoroutine());
    }

    private void Update () {
        if (generateMap > 0) {
            generateMap--;

            if (generateMap <= 0) {
                generateMap = 0;
                random = new System.Random(this.onlineSeed);
                StartCoroutine(GenerateMap(this.onlineSeed, this.onlineTileSetName));
            }
        }
    }

    /// <summary>
    /// Work around for FindObjectOfType not working during scene change.
    /// </summary>
    private IEnumerator FindSandCoroutine()
    {
        Sand sand = FindObjectOfType<Sand>();
        while (sand == null)
        {
            yield return null;
            sand = FindObjectOfType<Sand>();
        }
        sand.SetColor(tileSet.sandColor);
    }

    [ClientRpc]
    private void SendGenerationInfoClientRpc(int seedNew, string tileSetNameNew)
    {
        this.seed = seedNew;
        this.tileSetName = tileSetNameNew;
        this.onlineSeed = seedNew;
        this.onlineTileSetName = tileSetNameNew;
        // StartCoroutine(GenerateMap(seed, tileSetName));
        generateMap = 3;
    }

    private void LoadTileSet () {
        tileSet = Resources.Load<TileSet>("TileSets/" + tileSetName);
        if(tileSet == null)
        {
            var tileSets = Resources.LoadAll<TileSet>("TileSets/");
            tileSet = tileSets[random.Next(0, tileSets.Length)];
        }
    }

    public IEnumerator GenerateMap(int seedU, string tileSetNameU)
    {
        while (SceneManager.GetActiveScene().name != "Island") yield return null;

        // random = new System.Random(seed);

        float[,] heightMap = GenerateHeightMap(seedU);

        MeshData data = MeshGenerator.GenerateTerrainMeshFlatTiles(heightMap, settings.meshScale);

        Mesh m = data.CreateMesh();
        navMeshFilter.sharedMesh = m;
        navMeshCollider.sharedMesh = m;

        TileData tileData = new TileData(heightMap, settings.meshScale, settings.miscDensity);

        float offset = (heightMap.GetLength(0) / 2.0f) - 0.5f;
        tileMeshFilter.mesh = TilePlacer.PlaceTiles(tileData, tileSet, navMeshFilter, offset);
        tileMeshRenderer.material = tileSet.material;

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

    #region ISavable implementation

    public System.Type GetSaveDataType()
    {
        return typeof(TerrainGeneratorData);
    }

    public void OnFinishLoad()
    {
        SetUpMap();
    }

    public class TerrainGeneratorData : SaveData
    {
        public bool randomizeSeed;
        public int seed;
        public string tileSetName;
    }

    #endregion
}














