using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileObject
{
	public GameObject simple;
	public MeshArrangement[] meshArrangements;

	public GameObject DetermineMeshArrangement(TileData data, TileSet tileSet, Vector3Int position, ref int rotation, bool simple = false)
	{
		bool isTop = tileSet.topType == TileSet.TopType.AllTops && data.tileTypes[position.x, position.y + 1, position.z] == TileType.None;
		isTop |= tileSet.topType == TileSet.TopType.Height && position.y > data.maxHeight - tileSet.topLayers && tileSet.topLayers > 0 && position.y >= tileSet.topMinLayer;

		foreach (MeshArrangement arrangment in meshArrangements)
		{
			rotation = arrangment.requirements.GetRotationalFit(data.tileTypes, position);
			if (rotation == -1) continue;

			return simple ? this.simple : arrangment.GetMesh(20, isTop);
		}
		return null;
	}
}

[System.Serializable]
public class MeshArrangement
{
	public GameObject[] variations;
	public GameObject[] tVariations;

	public TilePlacementRequirementGroup requirements;

	public GameObject GetMesh(int varChance, bool isTop)
	{
		if (isTop) return GetTopMesh(varChance);
		int chance = TerrainGenerator.random.Next(0, 100);

		if (chance < varChance && variations.Length > 1)
		{
			int rand = TerrainGenerator.random.Next(0, variations.Length - 1);

			return variations[rand + 1];
		}

		return variations[0];
	}

	private GameObject GetTopMesh(int varChance)
	{
		if (tVariations.Length < 1)
		{
			return GetMesh(varChance, false);
		}

		int chance = TerrainGenerator.random.Next(0, 100);

		if (chance < varChance && tVariations.Length > 1)
		{
			int rand = TerrainGenerator.random.Next(0, tVariations.Length - 1);

			return tVariations[rand + 1];
		}

		return tVariations[0];
	}
}