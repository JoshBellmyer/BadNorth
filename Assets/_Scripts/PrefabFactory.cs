using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrefabFactory
{
    private static GameObject ladderPrefab;
    private static GameObject gridSelectionPrefab;
    private static Sprite3D unitSelectPrefabPlayer1;
    private static Sprite3D unitSelectPrefabPlayer2;
    private static Boat boatPrefab;

    public static void Initialize()
    {
        ladderPrefab = Resources.Load<GameObject>("Prefabs/Ladder");
        gridSelectionPrefab = Resources.Load<GameObject>("Prefabs/Selection");
        unitSelectPrefabPlayer1 = Resources.Load<Sprite3D>("Prefabs/Unit Select P1");
        unitSelectPrefabPlayer2 = Resources.Load<Sprite3D>("Prefabs/Unit Select P2");
        boatPrefab = Resources.Load<Boat>("Prefabs/Boat");
    }

    public static GameObject CreateLadderVisual(Player player)
    {
        GameObject ladder = Object.Instantiate(ladderPrefab);
        ladder.layer = LayerMask.NameToLayer($"Player {player.playerId}");
        ladder.GetComponentInChildren<MeshRenderer>().gameObject.layer = LayerMask.NameToLayer($"Player {player.playerId}");
        return ladder;
    }

    public static GameObject CreateGridSelectionVisual(Player player)
    {
        GameObject gridSelection = Object.Instantiate(gridSelectionPrefab);
        gridSelection.layer = LayerMask.NameToLayer($"Player {player.playerId}");
        gridSelection.GetComponentInChildren<MeshRenderer>().gameObject.layer = LayerMask.NameToLayer($"Player {player.playerId}");
        return gridSelection;
    }

    public static GameObject CreateUnitSelectSprite(Player player, Unit unit)
    {
        Sprite3D sprite;
        if (player.playerId == 1)
        {
            sprite = Object.Instantiate(unitSelectPrefabPlayer1);
        }
        else
        {
            sprite = Object.Instantiate(unitSelectPrefabPlayer2);
        }
        sprite.camera = player.camera;
        sprite.transform.SetParent(unit.transform);
        sprite.transform.localPosition = new Vector3(0, 0.75f, 0);
        return sprite.gameObject;
    }

    public static Boat CreateBoat(Player player, UnitType unitType)
    {
        Boat boat = Object.Instantiate<Boat>(boatPrefab);
        boat.SetPlayer(player);

        System.Type type = UnitManager.UnitEnumToType(unitType);
        var typeG = typeof(Group<>).MakeGenericType(type);

        dynamic unitGroup = System.Activator.CreateInstance(typeG);
        unitGroup.Initialize($"{player.playerId}");
        unitGroup.CanMove = false;
        unitGroup.CanAttack = false;
        boat.MountUnits(unitGroup.GetUnitsBase());
        return boat;
    }
}
