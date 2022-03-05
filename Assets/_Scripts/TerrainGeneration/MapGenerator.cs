using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class MapGenerator : MonoBehaviour {

	[Header("References")]
	public GameObject[] otherMeshes;
	[SerializeField] private TerrainGenerator terrainGenerator;
	[SerializeField] private GameObject meshObject;
	[SerializeField] private NavMeshSurface surface;


	private void Start () {
		GenerateMap();
	}

	// Generate a full map, including terrain shape, tiles, and slopes
	public void GenerateMap () {
		Game.mapGenerator = this;

		MeshFilter filter = meshObject.GetComponent<MeshFilter>();
		MeshRenderer renderer = meshObject.GetComponent<MeshRenderer>();
		float meshScale = terrainGenerator.settings.meshScale;

		terrainGenerator.GenerateMap();
	}
}