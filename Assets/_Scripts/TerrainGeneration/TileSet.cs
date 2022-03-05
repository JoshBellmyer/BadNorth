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

	public GameObject PickTile(int index, TileData data, Vector3Int position)
    {
		bool isTop = topType == TopType.AllTops && data.tileTypes[position.x, position.y + 1, position.z] == TileType.None;
		isTop |= topType == TopType.Height && position.y > data.maxHeight - topLayers && topLayers > 0 && position.y >= topMinLayer;
		return models[index].RandomVariation(20, isTop);
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

	public List<TilePlacementRequirement> requirements;

	public GameObject RandomVariation (int varChance, bool isTop) {
		if (isTop) return RandomVariationTop(varChance);
		int chance = Random.Range(0, 100);

		if (chance < varChance && variations.Length > 1) {
			int rand = Random.Range(0, variations.Length - 1);

			return variations[rand + 1];
		}

		return variations[0];
	}

	private GameObject RandomVariationTop (int varChance) {
		if (tVariations.Length < 1) {
			return RandomVariation(varChance, false);
		}

		int chance = Random.Range(0, 100);

		if (chance < varChance && tVariations.Length > 1) {
			int rand = Random.Range(0, tVariations.Length - 1);

			return tVariations[rand + 1];
		}

		return tVariations[0];
	}
}