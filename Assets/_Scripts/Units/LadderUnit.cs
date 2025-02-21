using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderUnit : Unit
{
    public static readonly int HEALTH = 100;

    [SerializeField] private GameObject ladder;

    private bool attaching;
    private Vector3 attachPos;
    private Vector3 attachNormal;
    private Vector3 landingPos;
    private Vector3 bottomPos;

    private Vector3 ladderStartPos;
    private Vector3 ladderStartRot;

    private bool _attached;
    private bool _occupied;

    public bool Attached {
        get => _attached;
    }

    public bool Occupied {
        get => _occupied;
        set { _occupied = value; }
    }

    protected override void UnitStart () {
        ladderStartPos = ladder.transform.localPosition;
        ladderStartRot = ladder.transform.localEulerAngles;
    }

    protected override void UnitUpdate () {
        if (attaching) {
            if (Vector3.Distance(transform.position, _destination) < 0.1f) {
                FinishAttach();
            }
        }
    }

    protected override bool FindAttack () {
        return false;
    }

    protected override void OnMove () {
        _occupied = false;
        _attached = false;
        ladder.transform.localPosition = ladderStartPos;
        ladder.transform.localEulerAngles = ladderStartRot;

        SetLadderPos(ladderStartPos, ladderStartRot, false);
    }

    public void AttachToWall (Vector3 pos, Vector3 normal) {
        if (Game.online && !Game.isHost) {
            networkUnit.AttachToWallServerRpc(pos, normal);

            return;
        }

        attachPos = pos;
        attachNormal = normal;
        landingPos = GridUtils.GetTopFromSide(pos, normal);
        attaching = true;
    }

    public Vector3 GetEdgePos () {
    	return landingPos + (transform.forward * -0.35f);
    }

    public Vector3 GetBottomPos () {
        return bottomPos;
    }

    private void FinishAttach () {
        // Group.TeleportTo(_destination);
        // Group.SetAgentEnabled(false);

        Game.SetPosition(gameObject, _destination);
        SetAgentEnabled(false);

        transform.forward = -attachNormal;
        // ladder.transform.localPosition = new Vector3(0, 0.5f, 0.5f);
        // ladder.transform.forward = -attachNormal;

        SetLadderPos(new Vector3(0, 0.5f, 0.5f), -attachNormal, true);

        bottomPos = transform.position + (attachNormal * -0.3f);

        attaching = false;
        _attached = true;
    }

    public void SetLadderPos (Vector3 newPos, Vector3 newRot, bool forward) {
        if (Game.online && Game.isHost) {
            networkUnit.SetLadderPosClientRpc(newPos, newRot, forward);
        }

        ladder.transform.localPosition = newPos;
        
        if (forward) {
            ladder.transform.forward = newRot;
        }
        else {
            ladder.transform.localEulerAngles = newRot;
        }
    }
}













