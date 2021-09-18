using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class MeshBuilder : MonoBehaviour {

	private static int vertOffset;

	
	// Combines two meshes by adding the mesh of newMeshObject to the originalMesh
	public static void AddToMesh (Mesh originalMesh, GameObject newMeshObject) {
		Mesh newMesh = newMeshObject.GetComponent<MeshFilter>().mesh;

		Vector3[] newVertices = CombineVertices(originalMesh, newMesh, newMeshObject);
		int[] newTriangles = CombineTriangles(originalMesh, newMesh, newMeshObject);
		Vector2[] newUV = CombineUV(originalMesh, newMesh, newMeshObject);
		Vector3[] newNormals = CombineNormals(originalMesh, newMesh, newMeshObject);

		originalMesh.Clear();

		originalMesh.vertices = newVertices;
		originalMesh.triangles = newTriangles;
		originalMesh.uv = newUV;
		originalMesh.normals = newNormals;
	}

	// ^ Combines vertices of the two meshes into a single vertex array
	private static Vector3[] CombineVertices (Mesh originalMesh, Mesh newMesh, GameObject newMeshObject) {
		vertOffset = originalMesh.vertices.Length;
		Vector3[] newVertices = new Vector3[vertOffset + newMesh.vertices.Length];

		for (int i = 0; i < vertOffset; i++) {
			newVertices[i] = originalMesh.vertices[i];
		}

		for (int i = 0; i < newMesh.vertices.Length; i++) {
			Vector3 newVert = newMesh.vertices[i];
			Vector3 scale = newMeshObject.transform.localScale;

			newVert = new Vector3(newVert.x * scale.x, newVert.y * scale.y, newVert.z * scale.z);
			newVert = Quaternion.Euler(newMeshObject.transform.eulerAngles) * newVert;
			newVert += newMeshObject.transform.position;

			newVertices[vertOffset + i] = newVert;
		}

		return newVertices;
	}

	// ^ Combines that triangles of the two meshes into a single triangle array
	private static int[] CombineTriangles (Mesh originalMesh, Mesh newMesh, GameObject newMeshObject) {
		int triOffset = originalMesh.triangles.Length;
		int[] newTriangles = new int[triOffset + newMesh.triangles.Length];

		for (int i = 0; i < triOffset; i++) {
			newTriangles[i] = originalMesh.triangles[i];
		}

		for (int i = 0; i < newMesh.triangles.Length; i++) {
			newTriangles[triOffset + i] = newMesh.triangles[i] + vertOffset;
		}

		return newTriangles;
	}

	// ^ Combines the UV coordinates of the two meshes into a single UV array
	private static Vector2[] CombineUV (Mesh originalMesh, Mesh newMesh, GameObject newMeshObject) {
		Vector2[] newUV = new Vector2[vertOffset + newMesh.uv.Length];

		for (int i = 0; i < vertOffset; i++) {
			newUV[i] = originalMesh.uv[i];
		}

		for (int i = 0; i < newMesh.uv.Length; i++) {
			newUV[vertOffset + i] = newMesh.uv[i];
		}

		return newUV;
	}

	// ^ Combines the normals of the two meshes into a single normal array
	private static Vector3[] CombineNormals (Mesh originalMesh, Mesh newMesh, GameObject newMeshObject) {
		Vector3[] newNormals = new Vector3[originalMesh.normals.Length + newMesh.normals.Length];

		for (int i = 0; i < originalMesh.normals.Length; i++) {
			newNormals[i] = originalMesh.normals[i];
		}

		for (int i = 0; i < newMesh.normals.Length; i++) {
			Vector3 newNorm = newMesh.normals[i];

			newNorm = Quaternion.Euler(newMeshObject.transform.eulerAngles) * newNorm;

			newNormals[originalMesh.normals.Length + i] = newNorm;
		}

		return newNormals;
	}
}


















