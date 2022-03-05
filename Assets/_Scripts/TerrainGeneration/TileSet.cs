using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TileSet", menuName = "ScriptableObjects/TileSet", order = 1)]
public class TileSet : ScriptableObject {

	public TopType topType;
	public int topLayers;
	public int topMinLayer;
	public TileGroup[] models;
	public Material material;
	public Color sandColor;

	public GameObject PickTile(TileData data, Vector3Int position, ref int rotation, bool simple = false)
    {
		bool isTop = topType == TopType.AllTops && data.tileTypes[position.x, position.y + 1, position.z] == TileType.None;
		isTop |= topType == TopType.Height && position.y > data.maxHeight - topLayers && topLayers > 0 && position.y >= topMinLayer;

		GameObject ret = null;
		foreach (TileGroup group in models)
        {
			rotation = ret == null ? group.requirements.GetRotationalFit(data.tileTypes, position) : rotation;
			if (rotation == -1) continue;

			if (ret != null)
			{
				Debug.LogWarning("Found multiple matches in a tileset");
				Debug.Log(group.variations[0].name);
				Debug.Log(ret.name);
				break;
			}
			ret = simple ? group.simple : group.RandomVariation(20, isTop);
        }

		//if (ret == null)
		//{
		//	Debug.LogError("Uh oh");
		//	Debug.Log(string.Format("{0} {1} {2}", data.tileTypes[position.x - 1, position.y, position.z - 1], data.tileTypes[position.x, position.y, position.z - 1], data.tileTypes[position.x + 1, position.y, position.z - 1]));
		//	Debug.Log(string.Format("{0} {1} {2}", data.tileTypes[position.x - 1, position.y, position.z], data.tileTypes[position.x, position.y, position.z], data.tileTypes[position.x + 1, position.y, position.z]));
		//	Debug.Log(string.Format("{0} {1} {2}", data.tileTypes[position.x - 1, position.y, position.z + 1], data.tileTypes[position.x, position.y, position.z + 1], data.tileTypes[position.x + 1, position.y, position.z + 1]));
		//	Debug.Log(data.tileTypes[position.x, position.y + 1, position.z] + ", " + rotation);
		//}
		return ret;
	}

	public enum TopType {
		None = 0,
		Height = 1,
		AllTops = 2,
	}
}


[System.Serializable]
public class TileGroup {

	public GameObject[] variations;
	public GameObject[] tVariations;
	public GameObject simple;

	public TilePlacementRequirementGroup requirements;

    public GameObject RandomVariation (int varChance, bool isTop) {
		if (isTop) return RandomVariationTop(varChance);
		int chance = TerrainGenerator.random.Next(0, 100);

		if (chance < varChance && variations.Length > 1) {
			int rand = TerrainGenerator.random.Next(0, variations.Length - 1);

			return variations[rand + 1];
		}

		return variations[0];
	}

	private GameObject RandomVariationTop (int varChance) {
		if (tVariations.Length < 1) {
			return RandomVariation(varChance, false);
		}

		int chance = TerrainGenerator.random.Next(0, 100);

		if (chance < varChance && tVariations.Length > 1) {
			int rand = TerrainGenerator.random.Next(0, tVariations.Length - 1);

			return tVariations[rand + 1];
		}

		return tVariations[0];
	}
}