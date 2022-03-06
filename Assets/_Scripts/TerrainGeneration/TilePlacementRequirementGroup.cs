using UnityEngine;

[CreateAssetMenu(fileName = "TilePlacementRequirementGroup")]
public class TilePlacementRequirementGroup : ScriptableObject
{
    public TilePlacementRequirement[] requirements;

    public int GetRotationalFit(TileType[,,] tileTypes, Vector3Int position)
    {
        for (int i = 0; i < 4; i++)
        {
            bool good = true;
            foreach (TilePlacementRequirement requirement in requirements)
            {
                if (!requirement.Check(tileTypes, position, i))
                {
                    good = false;
                    break;
                }
            }
            if (good)
            {
                return i * 90;
            }
        }
        return -1;
    }
}
