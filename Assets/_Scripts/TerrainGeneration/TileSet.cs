using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TileSet", menuName = "ScriptableObjects/TileSet", order = 1)]
public class TileSet : ScriptableObject {

	public TileSetType type;
	public string tileSetName;
	public TopType topType;
	public int topLayers;
	public int topMinLayer;
	public TileGroup[] models;
	public Material material;
	public Color sandColor;


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

	public GameObject RandomVariation (int varChance) {
		int chance = Random.Range(0, 100);

		if (chance < varChance && variations.Length > 1) {
			int rand = Random.Range(0, variations.Length - 1);

			return variations[rand + 1];
		}

		return variations[0];
	}

	public GameObject RandomVariationTop (int varChance) {
		if (tVariations.Length < 1) {
			return RandomVariation(varChance);
		}

		int chance = Random.Range(0, 100);

		if (chance < varChance && tVariations.Length > 1) {
			int rand = Random.Range(0, tVariations.Length - 1);

			return tVariations[rand + 1];
		}

		return tVariations[0];
	}
}

public enum TileSetType
{
	Random = 0,
	A = 1,
	B = 2,
	C = 3,
}