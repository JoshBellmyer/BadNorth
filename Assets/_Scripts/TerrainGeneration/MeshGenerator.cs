using UnityEngine;
using System.Collections;

public static class MeshGenerator
{

    public static MeshData GenerateTerrainMesh(float[,] noiseMap)
	{
        float scale = 10;
        int length = noiseMap.GetLength(0);
        float topLeftX = (length - 1) / -2f;
        float topLeftZ = (length - 1) / 2f;
        int area = length * length; // assuming it's a square
        int maxMeshTriangles = area * 2;
        int maxMeshVertices = maxMeshTriangles * 3;
        MeshData meshData = new MeshData(maxMeshVertices, maxMeshTriangles);

		int vertexIndex = 0;

        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < length; y++)
            {
                Vector3 vertex = new Vector3(topLeftX + x, noiseMap[x, y] * scale, topLeftZ - y);
                meshData.AddVertex(vertex, new Vector2(x/(float)length, y/(float)length), vertexIndex);

                if(x < length - 1 && y < length - 1)
                {
                    meshData.AddTriangle(vertexIndex + length, vertexIndex + length + 1, vertexIndex);
                    meshData.AddTriangle(vertexIndex + 1, vertexIndex, vertexIndex + length + 1);
                }

                vertexIndex++;
            }
        }

		return meshData;

	}
}

public class MeshData
{
	Vector3[] vertices;
	int[] triangles;
	Vector2[] uvs;

	int triangleIndex;

	public MeshData(int maxNumVertices, int maxNumTriangles)
	{
		vertices = new Vector3[maxNumVertices];
		uvs = new Vector2[maxNumVertices];
		triangles = new int[maxNumTriangles * 3];
	}

	public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
	{
		vertices[vertexIndex] = vertexPosition;
		uvs[vertexIndex] = uv;
	}

	public void AddTriangle(int a, int b, int c)
	{
		triangles[triangleIndex] = a;
		triangles[triangleIndex + 1] = b;
		triangles[triangleIndex + 2] = c;
		triangleIndex += 3;
	}

	public Mesh CreateMesh()
	{
		Mesh mesh = new Mesh();
		//mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		return mesh;
	}

}