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
		
		terrainGenerator.GenerateMap();
		float[,] noise = terrainGenerator.GenerateMapNoise();
		TileData tileData = new TileData(noise, meshScale);

		filter.mesh = TilePlacer.PlaceTiles(tileData, Game.instance.testTileSet);
		renderer.material = Game.instance.testTileSet.material;

		float offset = (noise.GetLength(0) / 2.0f) - 0.5f;
		meshObject.transform.position += new Vector3(-offset, 0, -offset);
	}
}