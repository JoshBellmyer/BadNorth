using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class MapGenerator : MonoBehaviour {

	[SerializeField] private TerrainGenerator terrainGenerator;
	[SerializeField] private GameObject meshObject;
	[SerializeField] private NavMeshSurface surface;


	private void Start () {
		GenerateMap();
	}

	// Generate a full map, including terrain shape, tiles, and slopes
	public void GenerateMap () {
		MeshFilter filter = meshObject.GetComponent<MeshFilter>();
		MeshRenderer renderer = meshObject.GetComponent<MeshRenderer>();
		float meshScale = terrainGenerator.meshScale;

		terrainGenerator.GenerateMap();
		float[,] noise = terrainGenerator.GenerateMapNoise();
		TileData tileData = new TileData(noise, meshScale);

		filter.mesh = TilePlacer.PlaceTiles(tileData, Game.instance.testTileSet, terrainGenerator.meshFilter);
		renderer.material = Game.instance.testTileSet.material;
		terrainGenerator.meshFilter.gameObject.transform.localScale = new Vector3(1, 1, -1);

		float offset = (noise.GetLength(0) / 2.0f) - 0.5f;
		meshObject.transform.position += new Vector3(-offset, 0, -offset);
		
		surface.BuildNavMesh();
		terrainGenerator.meshFilter.GetComponent<MeshRenderer>().enabled = false;
	}
}