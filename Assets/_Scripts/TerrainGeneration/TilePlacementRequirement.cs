using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TilePlacementRequirement : MonoBehaviour
{
    [SerializeField] Vector3Int relativePosition;
    [SerializeField] TileType requiredType;

    public bool Check(TileType[,,] tileTypes, Vector3Int position, int rotate90Count)
    {
        try
        {
            Vector3Int relPos = relativePosition;
            while(rotate90Count > 0)
            {
                relPos.GetRotation90(Vector3Int.up);
                rotate90Count--;
            }
            Vector3Int pos = position + relPos;
            return tileTypes[pos.x, pos.y, pos.z] == requiredType;
        }
        catch (System.IndexOutOfRangeException)
        {
            return false;
        }
    }
}
