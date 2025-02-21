﻿using UnityEngine;

static class GridUtils
{
	// Returns the position at the center of the grid square containing the given pos
	public static Vector3 GetGridPos(Vector3 pos)
	{
		float xPos = Mathf.Floor(pos.x) + 0.5f;
		float yPos = pos.y;
		float zPos = Mathf.Floor(pos.z) + 0.5f;

		RaycastHit hit;
		bool hitGround = Physics.Raycast(new Vector3(xPos, pos.y + 1, zPos), Vector3.down, out hit, 2.2f, LayerMask.GetMask("Terrain"));

		if (hitGround)
		{
			yPos = hit.point.y;
		}

		return new Vector3(xPos, yPos, zPos);
	}

	public static Vector3 GetGridNormal(Vector3 pos)
	{
		float xPos = Mathf.Floor(pos.x) + 0.5f;
		float zPos = Mathf.Floor(pos.z) + 0.5f;

		RaycastHit hit;
		bool hitGround = Physics.Raycast(new Vector3(xPos, pos.y + 1, zPos), Vector3.down, out hit, 2.2f, LayerMask.GetMask("Terrain"));

		if (hitGround)
		{
			return hit.normal;
		}

		return Vector3.zero;
	}

	// Returns the position at the same x and z as the given pos but, level with the ground
	public static Vector3 GetGroundLevelPos(Vector3 pos)
	{
		float yPos = pos.y;
		RaycastHit hit;
		bool hitGround = Physics.Raycast(new Vector3(pos.x, pos.y + 1, pos.z), Vector3.down, out hit, 2.2f, LayerMask.GetMask("Terrain"));

		if (hitGround)
		{
			yPos = hit.point.y;
		}

		return new Vector3(pos.x, yPos, pos.z);
	}

	public static Vector3 GetTopFromSide(Vector3 pos, Vector3 normal)
	{
		Vector3 newPos = pos + (normal * -0.5f) + new Vector3(0, 0.5f, 0);

		return GetGridPos(newPos);
	}

	public static Vector3 GetSideGridPos(Vector3 pos, Vector3 normal)
	{
		float xPos = pos.x;
		float yPos = Mathf.Floor(pos.y) + 0.5f;
		float zPos = pos.z;

		if (normal.x != 0)
		{
			zPos = Mathf.Floor(pos.z) + 0.5f;
		}
		else
		{
			xPos = Mathf.Floor(pos.x) + 0.5f;
		}

		return new Vector3(xPos, yPos, zPos);
	}

	public static bool CheckForTerrainAbove(Vector3 pos)
	{
		Collider[] cols = Physics.OverlapSphere(pos + new Vector3(0, 0.75f, 0), 0.35f, LayerMask.GetMask("Terrain"));

		return (cols.Length > 0);
	}
}
