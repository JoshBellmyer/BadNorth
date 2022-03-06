using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class OnlinePlayer : NetworkBehaviour {

    public int playerId;

    private NetworkObject netObj;


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


    [ServerRpc]
    public void SpawnObjectServerRpc (string prefabString) {
        
    }

    // Put handlers for incoming network messages in here
    private void InitMessageHandlers () {
        
    }
}