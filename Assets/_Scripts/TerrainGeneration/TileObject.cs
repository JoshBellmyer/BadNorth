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

		if (meshArrangements.Length == 1)
		{
			rotation = TerrainGenerator.random.Next(0, 4) * 90;
			return simple ? this.simple : meshArrangements[0].GetMesh(isTop);
		}
		else
		{
			foreach (MeshArrangement arrangment in meshArrangements)
			{

				rotation = arrangment.requirements.GetRotationalFit(data.tileTypes, position);
				if (rotation == -1) continue;

				return simple ? this.simple : arrangment.GetMesh(isTop);
			}
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

	public GameObject GetMesh(bool isTop)
	{
		if (isTop) return GetTopMesh();
		int chance = TerrainGenerator.random.Next(0, variations.Length);
		return variations[chance];
	}

	private GameObject GetTopMesh()
	{
		if (tVariations.Length < 1)
		{
			return GetMesh(false);
		}
		int chance = TerrainGenerator.random.Next(0, tVariations.Length);
		return tVariations[chance];
	}
}