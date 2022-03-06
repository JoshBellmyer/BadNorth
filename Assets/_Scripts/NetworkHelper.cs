using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class NetworkHelper : MonoBehaviour {

    private NetworkManager networkManager;


    private void Awake () {
        networkManager = GetComponent<NetworkManager>();

        Game.networkManager = networkManager;
        Game.networkHelper = this;
    }


    public void SendMessage (string messageName, string message) {
        if (!networkManager.gameObject.activeSelf || !Game.online) {
            return;
        }
        if (networkManager.ConnectedClients.Count < 2) {
            return;
        }

        ulong clientId = 0;

        if (Game.isHost) {
            clientId = networkManager.ConnectedClientsList[1].ClientId;
        }
        else {
            clientId = networkManager.ServerClientId;
        }

        using FastBufferWriter writer = new FastBufferWriter(sizeof(char) * message.Length + 8, Allocator.Temp);
        writer.WriteValueSafe(message);
        networkManager.CustomMessagingManager.SendNamedMessage(messageName, clientId, writer, NetworkDelivery.Reliable);
    }

    [ServerRpc]
    public GameObject SpawnObjectServerRpc (GameObject prefab) {
        GameObject newObj = Instantiate<GameObject>(prefab);
        newObj.GetComponent<NetworkObject>().Spawn();

        return newObj;
    }
}