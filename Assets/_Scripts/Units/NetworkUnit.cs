using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkUnit : NetworkBehaviour {

    private Unit unit;
    public NetworkVariable<int> team = new NetworkVariable<int>();
    public NetworkVariable<bool> inBoat = new NetworkVariable<bool>(true);
    public NetworkVariable<bool> canMove = new NetworkVariable<bool>(true);
    public NetworkVariable<bool> canAttack = new NetworkVariable<bool>(true);
    public NetworkVariable<int> health = new NetworkVariable<int>();


    private void Awake () {
        unit = GetComponent<Unit>();

        if (!Game.online) {
            return;
        }
    }

    [ClientRpc]
    public void DamageEffectClientRpc () {
        if (Game.isHost) {
            return;
        }

        unit.GetComponent<DamageHelper>().DamageEffect();
    }


    [ServerRpc(RequireOwnership = false)]
    public void IssueDestinationServerRpc (Vector3 destination) {
        unit.IssueDestination(destination);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetTeamServerRpc (string newTeam) {
        team.Value = int.Parse(newTeam);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetInBoatServerRpc (bool newInBoat) {
        inBoat.Value = newInBoat;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetAgentEnabledServerRpc (bool enabled) {
        unit.NavMeshAgent.enabled = enabled;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetCanMoveServerRpc (bool newCanMove) {
        canMove.Value = newCanMove;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetCanAttackServerRpc (bool newCanAttack) {
        canAttack.Value = newCanAttack;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetHealthServerRpc (int newHealth) {
        health.Value = newHealth;
    }
}