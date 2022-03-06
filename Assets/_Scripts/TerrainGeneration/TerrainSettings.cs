using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TerrainSettings")]
public class TerrainSettings : ScriptableObject
{
    [Range(2, 24)]
    public int size;

    public bool randomizeSeed;
    [SerializeField] int seed;
    public int Seed
    {
        get 
        {
            if (randomizeSeed)
            {
                seed = Random.Range(int.MinValue, int.MaxValue);
            }
            return seed; 
        }
        set => seed = value;
    }

    public NoiseSettings noiseSettings;

    public float scaleHeight;
    public bool applyTerraces;
    public int terraceFrequency;
    public bool flattenPeaks;
    public int flattenPeaksCutoff;
    public int flattenPeakLookRange;
    public float meshScale;
    public bool flatTilesMesh;

    public TileSet tileSet;
}
