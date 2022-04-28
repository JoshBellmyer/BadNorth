using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class OnlinePlayer : NetworkBehaviour {

    public int playerId;

    private NetworkObject netObj;
    public Group waitingGroup;
    public Player waitingPlayer;


    private void Start () {
        netObj = GetComponent<NetworkObject>();

        DontDestroyOnLoad(gameObject);
        InitMessageHandlers();

        if (Game.networkManager.IsHost) {
            playerId = (Game.networkManager.LocalClient.PlayerObject == netObj) ? 1 : 2;
        }
        else {
            playerId = (Game.networkManager.LocalClient.PlayerObject == netObj) ? 2 : 1;
        }

        // Handle the event that the other player disconnected
        Game.networkManager.OnClientDisconnectCallback += (id) => {
            if (playerId == 1) {
                Game.instance.SwitchToMainMenu();
            }
        };
    }

    [ClientRpc]
    public void EndGameClientRpc (int losingTeam) {
        Game.instance.EndGame(losingTeam);
    }

    [ClientRpc]
    public void PlaySoundClientRpc (Sound sound, float volume, bool varyPitch) {
        SoundPlayer.PlaySoundLocal(sound, volume, varyPitch);
    }


    [ServerRpc]
    public void LaunchArrowServerRpc (NetworkObjectReference archerUnit, int team, Vector3 launchVector, bool useB) {
        NetworkObject archerObj = (NetworkObject)archerUnit;

        Debug.Log(archerObj.gameObject);

        return;

        ArcherUnit archer = archerObj.GetComponent<ArcherUnit>();

        GameObject arrow = Instantiate<GameObject>(archer.arrowPrefabs[team]);
        arrow.GetComponent<NetworkObject>().Spawn();
        Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();

        arrow.GetComponent<Arrow>().Setup($"{team}", archer, useB);
        arrow.transform.position = archer.transform.position + (Vector3.up * 0.5f);
        arrow.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        arrowRb.velocity = launchVector;
    }


    [ServerRpc]
    public void SpawnUnitServerRpc (ulong clientId, int team, UnitType unitType) {
        GameObject prefab = UnitManager.instance.GetPrefabOfType(unitType);

        if (prefab == null) {
            return;
        }

        NetworkObjectReference[] unitRef = new NetworkObjectReference[prefab.GetComponent<Unit>().groupAmount];

        NetworkGroup networkGroup = PrefabFactory.CreateNetworkGroup();
        networkGroup.GetComponent<NetworkObject>().Spawn();

        for (int i = 0; i < prefab.GetComponent<Unit>().groupAmount; i++) {
            Unit unit = UnityEngine.Object.Instantiate(prefab).GetComponent<Unit>();
            NetworkObject unitNetObj = unit.GetComponent<NetworkObject>();

            unitNetObj.Spawn();
            unitRef[i] = unitNetObj;
            TeamManager.instance.Add($"{team}", unit);
        }

        Boat boat = PrefabFactory.CreateBoat();
        boat.GetComponent<NetworkObject>().Spawn();
        boat.GetComponent<NetworkObject>().ChangeOwnership(clientId);

        SendSpawnedUnitsClientRpc(clientId, playerId, networkGroup.GetComponent<NetworkObject>(), boat.GetComponent<NetworkObject>(), unitRef);
    }

    [ClientRpc]
    public void SendSpawnedUnitsClientRpc (ulong clientId, int playerId, NetworkObjectReference networkGroup, NetworkObjectReference networkBoat, NetworkObjectReference[] networkUnits) {
        if (clientId != Game.networkManager.LocalClientId) {
            return;
        }
        if (this.playerId != playerId) {
            return;
        }

        if (waitingGroup != null) {
            waitingGroup.FinishOnlineSpawn(networkGroup, networkUnits);
        }
        if (waitingPlayer != null) {
            waitingPlayer.FinishOnlineBoat(networkBoat, waitingGroup);
        }

        waitingGroup = null;
        waitingPlayer = null;
    }

    [ServerRpc]
    public void SetParentServerRpc (NetworkObjectReference obj, NetworkObjectReference newParent) {
        NetworkObject netObj = (NetworkObject)obj;
        NetworkObject netParent = (NetworkObject)newParent;

        netObj.transform.SetParent(netParent.transform);
    }

    [ServerRpc]
    public void UnsetParentServerRpc (NetworkObjectReference obj) {
        NetworkObject netObj = (NetworkObject)obj;

        netObj.transform.SetParent(null);
    }

    [ServerRpc]
    public void SetPositionServerRpc (NetworkObjectReference obj, Vector3 pos) {
        NetworkObject netObj = (NetworkObject)obj;

        netObj.transform.position = pos;
    }

    [ServerRpc]
    public void SetRotationServerRpc (NetworkObjectReference obj, Vector3 rot) {
        NetworkObject netObj = (NetworkObject)obj;

        netObj.transform.eulerAngles = rot;
    }

    [ServerRpc]
    public void DestroyObjectServerRpc (NetworkObjectReference obj) {
        NetworkObject netObj = (NetworkObject)obj;

        if (netObj == null) {
            return;
        }

        Destroy(netObj.gameObject);
    }


    // Put handlers for incoming network messages in here
    private void InitMessageHandlers () {
        
    }
}


















