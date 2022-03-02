using UnityEngine;
using System.Collections;

public static class Noise
{
	public static float[,] GenerateNoiseMap(int mapSize, NoiseSettings settings, Vector2 sampleCentre, int seed)
	{
		float[,] noiseMap = new float[mapSize, mapSize];

		System.Random prng = new System.Random(seed);
		Vector3[] octaveOffsets = new Vector3[settings.octaves];

		float maxPossibleHeight = 0;
		float amplitude = 1;
		float frequency = 1;

		for (int i = 0; i < settings.octaves; i++)
		{
			float offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCentre.x;
			float offsetY = prng.Next(-100000, 100000) - settings.offset.y + sampleCentre.y;
			octaveOffsets[i] = new Vector3(offsetX, offsetY);

			maxPossibleHeight += amplitude;
			amplitude *= settings.persistance;
		}

		float maxLocalNoiseHeight = float.MinValue;
		float minLocalNoiseHeight = float.MaxValue;

		float halfMapSize = mapSize / 2f;


		for (int y = 0; y < mapSize; y++)
		{
			for (int x = 0; x < mapSize; x++)
			{
				amplitude = 1;
				frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < settings.octaves; i++)
				{
					float sampleX = (x - halfMapSize + octaveOffsets[i].x) / settings.scale * frequency;
					float sampleY = (y - halfMapSize + octaveOffsets[i].y) / settings.scale * frequency;

					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;

					amplitude *= settings.persistance;
					frequency *= settings.lacunarity;
				}

				if (noiseHeight > maxLocalNoiseHeight)
				{
					maxLocalNoiseHeight = noiseHeight;
				}
				if (noiseHeight < minLocalNoiseHeight)
				{
					minLocalNoiseHeight = noiseHeight;
				}
				noiseMap[x, y] = noiseHeight;

				float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
				noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
			}
		}

		return noiseMap;
	}

}