using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class TileSetLoader {

	static readonly string TILESET_DATA_PATH = "TileSets/";
	static Dictionary<TileSetType, TileSet> tileSetMap;

	public static int TileSetCount
    {
		get
		{
			if (tileSetMap == null)
			{
				LoadTileSetMap();
			}

			return tileSetMap.Count;
		}
    }

	public static void LoadTileSetMap()
	{
		tileSetMap = new Dictionary<TileSetType, TileSet>();
		TileSet[] tileSets = Resources.LoadAll<TileSet>(TILESET_DATA_PATH);
		foreach (TileSet tileSet in tileSets)
		{
			tileSetMap[tileSet.type] = tileSet;
		}
	}

	public static TileSet GetTileSet(TileSetType type)
    {
		if (tileSetMap == null)
		{
			LoadTileSetMap();
		}

		if(type == TileSetType.Random)
        {
			return GetRandomTileSet();
        }

		try
		{
			return tileSetMap[type];
		}
		catch (KeyNotFoundException)
		{
			return null;
		}
	}

	public static TileSet GetRandomTileSet()
    {
		List<TileSet> tileSets = GetTileSets();
		int rand = Random.Range(0, tileSets.Count);
		return tileSets[rand];
    }

	public static List<TileSet> GetTileSets()
    {
		return tileSetMap.Values.ToList();
    }
}