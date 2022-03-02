using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct NoiseSettings
{
	public float scale;

	[Range(1,10)]
	public int octaves;
	[Range(0, 1)]
	public float persistance;
	public float lacunarity;

	public Vector3 offset;
}
