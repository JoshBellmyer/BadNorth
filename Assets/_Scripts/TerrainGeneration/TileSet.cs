using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TileSet", menuName = "ScriptableObjects/TileSet", order = 1)]
public class TileSet : ScriptableObject {

	public TopType topType;
	public int topLayers;
	public int topMinLayer;
	public TileObject cube;
	public TileObject fullRamp;
	public TileObject halfRamp;
	public TileObject halfRampRaised;
	public TileObject miscellaneous;

	public Material material;
	public Color sandColor;

	public GameObject PickTile(TileData data, Vector3Int position, ref int rotation, bool simple = false)
    {
		switch (data.tileTypes[position.x, position.y, position.z])
        {
			case TileType.Cube:
				return cube.DetermineMeshArrangement(data, this, position, ref rotation, simple);
			case TileType.FullRamp:
				return fullRamp.DetermineMeshArrangement(data, this, position, ref rotation, simple);
			case TileType.HalfRamp:
				return halfRamp.DetermineMeshArrangement(data, this, position, ref rotation, simple);
			case TileType.HalfRampRaised:
				return halfRampRaised.DetermineMeshArrangement(data, this, position, ref rotation, simple);
			case TileType.Miscellaneous:
				return miscellaneous.DetermineMeshArrangement(data, this, position, ref rotation, simple);
			default:
				Debug.LogError("No object found of type " + data.tileTypes[position.x, position.y, position.z]);
				return null;
		}
	}

	public enum TopType {
		None = 0,
		Height = 1,
		AllTops = 2,
	}
}
