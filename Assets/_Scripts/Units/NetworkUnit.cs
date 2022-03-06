using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkUnit : NetworkBehaviour {

    private Unit unit;


    private void Awake () {
        unit = GetComponent<Unit>();

        if (!Game.online) {
            return;
        }


    }



    [ServerRpc]
    public void IssueDestinationServerRpc (Vector3 destination) {
        unit.IssueDestination(destination);
    }
}