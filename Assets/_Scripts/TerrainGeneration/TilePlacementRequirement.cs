using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TilePlacementRequirement")]
public class TilePlacementRequirement : ScriptableObject 
{
    [SerializeField] Vector3Int relativePosition;
    [SerializeField] TileType requiredType;
    [SerializeField] bool invertCondition;

    public bool Check(TileType[,,] tileTypes, Vector3Int position, int rotate90Count)
    {
        try
        {
            Vector3Int relPos = relativePosition;
            while(rotate90Count > 0)
            {
                relPos = relPos.GetRotation90(Vector3Int.up);
                rotate90Count--;
            }
            Vector3Int pos = position + relPos;

            if (!invertCondition)
                return tileTypes[pos.x, pos.y, pos.z] == requiredType;
            else
                return tileTypes[pos.x, pos.y, pos.z] != requiredType;
        }
        catch (System.IndexOutOfRangeException)
        {
            return invertCondition ? true : false;
        }
    }
}
