using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public static class PrefabFactory
{
    private static GameObject ladderPrefab;
    private static GameObject gridSelectionPrefab;
    private static Sprite3D unitSelectPrefabPlayer1;
    private static Sprite3D unitSelectPrefabPlayer2;
    private static Boat boatPrefab;
    private static PlayerController playerControllerPrefab;
    private static NetworkGroup networkGroupPrefab;
    private static TerrainGenerator terrainGeneratorPrefab;


    public static void Initialize()
    {
        ladderPrefab = Resources.Load<GameObject>("Prefabs/Ladder");
        gridSelectionPrefab = Resources.Load<GameObject>("Prefabs/Selection");
        unitSelectPrefabPlayer1 = Resources.Load<Sprite3D>("Prefabs/Unit Select P1");
        unitSelectPrefabPlayer2 = Resources.Load<Sprite3D>("Prefabs/Unit Select P2");
        boatPrefab = Resources.Load<Boat>("Prefabs/Boat");
        playerControllerPrefab = Resources.Load<PlayerController>("Prefabs/Player");
        networkGroupPrefab = Resources.Load<NetworkGroup>("Prefabs/NetworkGroup");
        terrainGeneratorPrefab = Resources.Load<TerrainGenerator>("Prefabs/TerrainGenerator");
    }

    public static PlayerController CreatePlayerController()
    {
        return Object.Instantiate(playerControllerPrefab);
    }

    public static NetworkGroup CreateNetworkGroup () {
        return Object.Instantiate(networkGroupPrefab);
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

    internal static TerrainGenerator CreateTerrainGenerator()
    {
        TerrainGenerator go = Object.Instantiate(terrainGeneratorPrefab);
        if (Game.online)
        {
            go.GetComponent<NetworkObject>().Spawn();
        }
        return go;
    }

    public static Boat CreateBoat(Player player, UnitType unitType) {
        Boat boat = Object.Instantiate<Boat>(boatPrefab);
        Game.ClearNetworking(boat.gameObject);
        boat.SetPlayer(player);

        Group unitGroup = new Group($"{player.playerId}", unitType);
        boat.MountUnits(unitGroup.GetUnits());

        return boat;
    }

    public static Boat CreateBoat () {
        return Object.Instantiate<Boat>(boatPrefab);
    }
}










