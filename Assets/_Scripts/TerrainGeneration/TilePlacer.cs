using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePlacer : MonoBehaviour {

	// private static Dictionary<string, int> tileEdges;
	private static Dictionary<int, int> cubeEdges;
	private static Dictionary<string, int> rampEdges;

	private static TileData tileData;
	private static TileSet tileSet;
	private static List<CombineInstance> cInstances;
	private static List<CombineInstance> mInstances;
	private static List<GameObject> tempTiles;
	private static Mesh mesh;
	private static Mesh cMesh;

	public static GameObject[] otherMeshes;


	// Places tiles based on the given tileData and tileSet, and combines them into a single mesh
	public static Mesh PlaceTiles (TileData _tileData, TileSet _tileSet, MeshFilter colMesh) {
		tileData = _tileData;
		tileSet = _tileSet;

		if (cubeEdges == null) {
			InitializeEdges();
		}

		mesh = new Mesh();
		cMesh = new Mesh();
		cInstances = new List<CombineInstance>();

		mInstances = new List<CombineInstance>();
		CombineInstance combine = new CombineInstance();
		combine.mesh = colMesh.sharedMesh;
		combine.transform = Matrix4x4.identity;
		mInstances.Add(combine);

		foreach (TileLocation tileLoc in tileData.tileLocations) {
			//if (tileLoc.type != TileType.Cube) continue;
			PlaceTile(tileLoc.pos.x, tileLoc.pos.y, tileLoc.pos.z);
		}

		// tempObj.transform.localScale = new Vector3(1, 1, -1);

		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

		mesh.CombineMeshes(cInstances.ToArray());
		cMesh.CombineMeshes(mInstances.ToArray());
		colMesh.mesh = cMesh;
		colMesh.GetComponent<MeshCollider>().sharedMesh = cMesh;

		return mesh;
	}

	// Places a cube-shaped tile at the given x, y, z coords
	private static void PlaceCube (int x, int y, int z) {
		if (y < 1) {
			return;
		}

		Vector3Int pos = new Vector3Int(x, y, z);
		int front = GetEdge(pos, 0)[0] ? 0b1000 : 0b0000;
		int back = GetEdge(pos, 1)[0] ? 0b0100 : 0b0000;
		int left = GetEdge(pos, 2)[0] ? 0b0010 : 0b0000;
		int right = GetEdge(pos, 3)[0] ? 0b0001 : 0b0000;

		int edgeBool = (front | back | left | right);
		int[] index = new int[4];
		int tileIndex = -1;
		int rotation = 0;

		index[0] = edgeBool;
		index[1] = RotateEdges(edgeBool, 1);
		index[2] = RotateEdges(edgeBool, 2);
		index[3] = RotateEdges(edgeBool, 3);

		if (cubeEdges.ContainsKey(index[0])) {
			tileIndex = cubeEdges[index[0]];
		}
		else if (cubeEdges.ContainsKey(index[1])) {
			tileIndex = cubeEdges[index[1]];
			rotation = 1;
		}
		else if (cubeEdges.ContainsKey(index[2])) {
			tileIndex = cubeEdges[index[2]];
			rotation = 2;
		}
		else if (cubeEdges.ContainsKey(index[3])) {
			tileIndex = cubeEdges[index[3]];
			rotation = 3;
		}

		if (tileIndex >= 0) {
			PlaceTile(x, y, z);
		}
	}

	// Places a full slope tile at the given location
	private static void PlaceSlope (int x, int y, int z, int rotation, int slopeType) {
		Vector3Int pos = new Vector3Int(x, y, z);
		int front = GetEdge(pos, 0)[0] ? 0b1000 : 0b0000;
		int back = GetEdge(pos, 1)[0] ? 0b0100 : 0b0000;
		int left = GetEdge(pos, 2)[0] ? 0b0010 : 0b0000;
		int right = GetEdge(pos, 3)[0] ? 0b0001 : 0b0000;

		int rampType = 0;

		switch (slopeType) {
			case 0b0000:
				rampType = 1;
			break;

			case 0b0100:
				rampType = 2;
			break;

			case 0b1000:
				rampType = 3;
			break;
		}

		int frontR = GetEdge(pos, 0)[rampType] ? 0b1000 : 0b0000;
		int backR = GetEdge(pos, 1)[rampType] ? 0b0100 : 0b0000;
		int leftR = GetEdge(pos, 2)[rampType] ? 0b0010 : 0b0000;
		int rightR = GetEdge(pos, 3)[rampType] ? 0b0001 : 0b0000;

		front = (front | frontR);
		back = (back | backR);
		left = (left | leftR);
		right = (right | rightR);

		int edgeBool = (front | back | left | right);
		string index = $"{slopeType},{RotateEdges(edgeBool, rotation)}";

		if (rampEdges.ContainsKey(index)) {
			PlaceTile(x, y, z);
			PlaceMeshTile(x, y, z, rotation, rampEdges[index]);
		}
	}


	// Places a tile and adds it to the mesh
	private static void PlaceTile (int x, int y, int z) {
		int rotation = 0;
		GameObject tileObject = tileSet.PickTile(tileData, new Vector3Int(x, y, z), ref rotation);
		if (tileObject == null) return;

		Vector3 position = new Vector3(x, y - 0.5f, z) + tileObject.transform.GetChild(0).position;
		Vector3 eulerAngles = new Vector3(0, rotation, 0) + tileObject.transform.GetChild(0).localEulerAngles;

		GameObject meshObject = tileObject.transform.GetChild(0).gameObject;

		CombineInstance combine = new CombineInstance();
		combine.mesh = meshObject.GetComponent<MeshFilter>().sharedMesh;
		combine.transform = Matrix4x4.TRS(position, Quaternion.Euler(eulerAngles), Vector3.one);
		cInstances.Add(combine);
	}

	// Places a mesh tile and adds it to the collision mesh
	private static void PlaceMeshTile (int x, int y, int z, int rotation, int tileIndex) {
		int meshIndex = -1;

		switch (tileIndex) {
			case 6:
			case 7:
			case 8:
			case 9:
				meshIndex = 0;
			break;

			case 10:
			case 11:
			case 12:
			case 13:
				meshIndex = 2;
			break;

			case 14:
			case 15:
			case 16:
			case 17:
				meshIndex = 1;
			break;
		}

		if (meshIndex < 0) {
			return;
		}

		GameObject tileObject = otherMeshes[meshIndex];

		float offset = (tileData.sizeX / 2.0f) - 0.5f;
		Vector3 position = new Vector3(x - offset, y - 0.5f, z - offset);
		Vector3 eulerAngles = new Vector3(0, 90 * rotation, 0);

		GameObject meshObject = tileObject.transform.GetChild(0).gameObject;

		CombineInstance combine = new CombineInstance();
		combine.mesh = meshObject.GetComponent<MeshFilter>().sharedMesh;
		combine.transform = Matrix4x4.TRS(position, Quaternion.Euler(eulerAngles), Vector3.one);
		mInstances.Add(combine);
	}

	// Returns the edge data from the given pos in the given direction
	private static bool[] GetEdge (Vector3Int pos, int direction) {
		Vector3Int newPos = pos + GetDirectionVector(direction);

		if (newPos.x < 0 || newPos.x >= tileData.sizeX) {
			return new bool[] {false, false, false};
		}
		if (newPos.z < 0 || newPos.z >= tileData.sizeZ) {
			return new bool[] {false, false, false};
		}

		bool[] results = new bool[5];
		int[] edges = TileData.GetEdges(tileData.tileTypes[newPos.x, newPos.y, newPos.z]);
		int opDir = GetOppositeDirection(direction);
		int mask = (0b1000 >> opDir);

		results[0] = (edges[0] & mask) != 0;
		results[1] = (edges[1] & mask) != 0;
		results[2] = (edges[2] & mask) != 0;
		results[3] = (edges[3] & mask) != 0;
		results[4] = (edges[4] & mask) != 0;

		return results;
	}

	// Returns an integer vector corresponding to the direction that the given integer represents
	private static Vector3Int GetDirectionVector (int direction) {
		Vector3Int[] vectors = new Vector3Int[] {
			new Vector3Int(0, 0, 1),
			new Vector3Int(0, 0, -1),
			new Vector3Int(-1, 0, 0),
			new Vector3Int(1, 0, 0),
		};

		return vectors[direction];
	}

	// Returns an integer representing the opposite direction of the given direction
	// 0 represents positive Z
	// 1 represents negative Z
	// 2 represents negative X
	// 3 represents positive X
	private static int GetOppositeDirection (int direction) {
		int newDir = 0;

		switch (direction) {
			case 0:
				newDir = 1;
			break;

			case 1:
				newDir = 0;
			break;

			case 2:
				newDir = 3;
			break;

			case 3:
				newDir = 2;
			break;
		}

		return newDir;
	}

	// Rotates the given tile edge data by [90 degrees times the given amount]
	private static int RotateEdges (int edges, int amount) {
		int newEdges = edges;

		for (int i = 0; i < amount; i++) {
			int front = (newEdges & 0b0001) != 0 ? 0b1000 : 0b0000;
			int back = (newEdges & 0b0010) != 0 ? 0b0100 : 0b0000;
			int left = (newEdges & 0b1000) != 0 ? 0b0010 : 0b0000;
			int right = (newEdges & 0b0100) != 0 ? 0b0001 : 0b0000;

			newEdges = (front | back | left | right);
		}

		return newEdges;
	}


	// Initializes the lookup table for tile edges
	// private static void InitializeEdges () {
	// 	tileEdges = new Dictionary<string, int>();

	// 	// Cubes
	// 	tileEdges.Add($"{0b1111},{0b0000},{0b0000}", 0);
	// 	tileEdges.Add($"{0b1011},{0b0000},{0b0000}", 1);
	// 	tileEdges.Add($"{0b1001},{0b0000},{0b0000}", 2);
	// 	tileEdges.Add($"{0b1100},{0b0000},{0b0000}", 3);
	// 	tileEdges.Add($"{0b1000},{0b0000},{0b0000}", 4);
	// 	tileEdges.Add($"{0b0000},{0b0000},{0b0000}", 5);

	// 	// Ramps
	// 	tileEdges.Add($"{0b0000},{0b0000},{0b1011}", 6);
	// 	tileEdges.Add($"{0b0000},{0b0000},{0b1001}", 7);
	// 	tileEdges.Add($"{0b0000},{0b0000},{0b1010}", 8);
	// 	tileEdges.Add($"{0b0000},{0b0000},{0b1000}", 9);

	// 	tileEdges.Add($"{0b0000},{0b0100},{0b1011}", 10);
	// 	tileEdges.Add($"{0b0000},{0b0100},{0b1001}", 11);
	// 	tileEdges.Add($"{0b0000},{0b0100},{0b1010}", 12);
	// 	tileEdges.Add($"{0b0000},{0b0100},{0b1000}", 13);

	// 	tileEdges.Add($"{0b0000},{0b1000},{0b0011}", 14);
	// 	tileEdges.Add($"{0b0000},{0b1000},{0b0001}", 15);
	// 	tileEdges.Add($"{0b0000},{0b1000},{0b0010}", 16);
	// 	tileEdges.Add($"{0b0000},{0b1000},{0b0000}", 17);
	// }

	private static void InitializeEdges () {
		cubeEdges = new Dictionary<int, int>();

		cubeEdges.Add(0b1111, 0);
		cubeEdges.Add(0b1011, 1);
		cubeEdges.Add(0b1001, 2);
		cubeEdges.Add(0b1100, 3);
		cubeEdges.Add(0b1000, 4);
		cubeEdges.Add(0b0000, 5);

		rampEdges = new Dictionary<string, int>();

		rampEdges.Add($"{0b0000},{0b1011}", 6);
		rampEdges.Add($"{0b0000},{0b1001}", 7);
		rampEdges.Add($"{0b0000},{0b1010}", 8);
		rampEdges.Add($"{0b0000},{0b1000}", 9);

		rampEdges.Add($"{0b0100},{0b1011}", 10);
		rampEdges.Add($"{0b0100},{0b1001}", 11);
		rampEdges.Add($"{0b0100},{0b1010}", 12);
		rampEdges.Add($"{0b0100},{0b1000}", 13);

		rampEdges.Add($"{0b1000},{0b0011}", 14);
		rampEdges.Add($"{0b1000},{0b0001}", 15);
		rampEdges.Add($"{0b1000},{0b0010}", 16);
		rampEdges.Add($"{0b1000},{0b0000}", 17);
	}
}




















