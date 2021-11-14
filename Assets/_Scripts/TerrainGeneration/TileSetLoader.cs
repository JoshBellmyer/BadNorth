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
		IEnumerable<string> files = Directory.GetFiles("Assets/Resources/" + TILESET_DATA_PATH)
			.Where(f => !f.Contains(".meta"))
			.Select(f => Path.GetFileNameWithoutExtension(f));
		foreach (string file in files)
		{
			TileSet tileSet = Resources.Load<TileSet>(TILESET_DATA_PATH + file);
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