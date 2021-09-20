using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTester : MonoBehaviour {

	[SerializeField] private TerrainGenerator terrainGenerator;
	[SerializeField] private GameObject meshObject;
	[SerializeField] private float meshScale;


	private void Start () {
		MeshFilter filter = meshObject.GetComponent<MeshFilter>();
		MeshRenderer renderer = meshObject.GetComponent<MeshRenderer>();
	
		float[,] noise = terrainGenerator.GenerateMapNoise();
		TileData tileData = new TileData(noise, meshScale);

		filter.mesh = TilePlacer.PlaceTiles(tileData, Game.instance.testTileSet);
		renderer.material = Game.instance.testTileSet.material;
	}
}