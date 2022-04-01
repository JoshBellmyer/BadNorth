using UnityEngine;
using System.Collections;

public static class MeshGenerator
{
	public static MeshData GenerateTerrainMeshFlatTiles(float[,] heightMap, float meshScale)
    {
        int length = heightMap.GetLength(0);
        float topLeftX = (length - 1) / -2f;
        float topLeftZ = (length - 1) / 2f;
        int area = length * length; // assuming it's a square
        int maxMeshTriangles = area * 6;
        int maxMeshVertices = maxMeshTriangles * 3;
        MeshData meshData = new MeshData(maxMeshVertices, maxMeshTriangles);

        int vertexIndex = 0;

        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < length; y++)
            {
                Vector3 uv = new Vector2(x / (float)length, y / (float)length);

                Vector3 vertexTopLeft = new Vector3(topLeftX + x - 0.5f, heightMap[x,y] * meshScale, topLeftZ - y + 0.5f);
                Vector3 vertexTopRight = new Vector3(topLeftX + x + 0.5f, heightMap[x, y] * meshScale, topLeftZ - y + 0.5f);
                Vector3 vertexBottomLeft = new Vector3(topLeftX + x - 0.5f, heightMap[x, y] * meshScale, topLeftZ - y - 0.5f);
                Vector3 vertexBottomRight = new Vector3(topLeftX + x + 0.5f, heightMap[x, y] * meshScale, topLeftZ - y - 0.5f);

                meshData.AddVertex(vertexTopLeft, uv, vertexIndex);
                meshData.AddVertex(vertexTopRight, uv, vertexIndex + 1);
                meshData.AddVertex(vertexBottomLeft, uv, vertexIndex + 2);
                meshData.AddVertex(vertexBottomRight, uv, vertexIndex + 3);

                meshData.AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + 2); // top left, top right, bottom left
                meshData.AddTriangle(vertexIndex + 1, vertexIndex + 3, vertexIndex + 2); // top right, bottom right, bottom left

                if (x < length - 1 && y < length - 1)
                {
                    meshData.AddTriangle(vertexIndex + 3, vertexIndex + 1, vertexIndex + length * 4); // bottom right, top right, other top left
                    meshData.AddTriangle(vertexIndex + 3, vertexIndex + length * 4, vertexIndex + 2 + length * 4); // bottom right, other top left, other bottom left
                    meshData.AddTriangle(vertexIndex + 3, vertexIndex + 1 + 4, vertexIndex + 2); // bottom right, other top right, bottom left ???
                    meshData.AddTriangle(vertexIndex + 2, vertexIndex + 1 + 4, vertexIndex + 4); // I don't even know anymore...
                }

                vertexIndex += 4;
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