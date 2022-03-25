using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkBoat : NetworkBehaviour {

    public NetworkVariable<int> playerId = new NetworkVariable<int>();


    void Awake () {

    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerIdServerRpc (int newPlayerId) {
        playerId.Value = newPlayerId;
    }
}
