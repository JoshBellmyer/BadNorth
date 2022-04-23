using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePlacer : MonoBehaviour {

	private static TileData tileData;
	private static TileSet tileSet;
	private static List<CombineInstance> mInstances;
	private static Mesh mesh;
	private static Mesh cMesh;
	private static GameObject organizationalParent;

	private static float offset;

	// Places tiles based on the given tileData and tileSet, and combines them into a single mesh
	public static Mesh PlaceTiles (TileData _tileData, TileSet _tileSet, MeshFilter colMesh, float offset) {
		tileData = _tileData;
		tileSet = _tileSet;
		TilePlacer.offset = offset;

		organizationalParent = new GameObject("Tiles");
		mesh = new Mesh();
		cMesh = new Mesh();

		mInstances = new List<CombineInstance>();
		CombineInstance combine = new CombineInstance();
		combine.mesh = colMesh.sharedMesh;
		combine.transform = Matrix4x4.TRS(colMesh.transform.position, colMesh.transform.rotation, colMesh.transform.localScale);
		combine.transform *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1)); // TODO: why is this scaled?
		mInstances.Add(combine);

		foreach (TileLocation tileLoc in tileData.tileLocations) {
			//if (tileLoc.type != TileType.Cube) continue;
			PlaceTile(tileLoc.pos.x, tileLoc.pos.y, tileLoc.pos.z);
			PlaceMeshTile(tileLoc.pos.x, tileLoc.pos.y, tileLoc.pos.z);
		}

		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

		cMesh.CombineMeshes(mInstances.ToArray());
		colMesh.mesh = cMesh;
		colMesh.GetComponent<MeshCollider>().sharedMesh = cMesh;

		return mesh;
	}

	// Places a tile and adds it to the mesh
	private static void PlaceTile (int x, int y, int z) {
		int rotation = 0;
		GameObject tileObject = tileSet.PickTile(tileData, new Vector3Int(x, y, z), ref rotation);
		if (tileObject == null) return;

		Vector3 position = new Vector3(x, y - 0.5f, z) - new Vector3(offset, 0, offset);
		Vector3 eulerAngles = new Vector3(0, rotation, 0);

		GameObject go = Instantiate(tileObject, organizationalParent.transform);
		go.transform.position = position;
		go.transform.rotation = Quaternion.Euler(eulerAngles);
	}

	// Places a mesh tile and adds it to the collision mesh
	private static void PlaceMeshTile (int x, int y, int z) {
		int rotation = 0;
		GameObject tileObject = tileSet.PickTile(tileData, new Vector3Int(x, y, z), ref rotation, true);
		if (tileObject == null) return;

		Vector3 position = new Vector3(x, y - 0.5f, z) + tileObject.transform.GetChild(0).localPosition - new Vector3(tileData.sizeX - 1, 0, tileData.sizeZ - 1) / 2;
		Quaternion rotationTransform = Quaternion.Euler(0, rotation, 0) * tileObject.transform.GetChild(0).rotation;
		Vector3 scale = tileObject.transform.GetChild(0).localScale;

		GameObject meshObject = tileObject.transform.GetChild(0).gameObject;

		CombineInstance combine = new CombineInstance();
		combine.mesh = meshObject.GetComponent<MeshFilter>().sharedMesh;
		combine.transform = Matrix4x4.TRS(position, rotationTransform, scale);
		mInstances.Add(combine);
	}

}




















