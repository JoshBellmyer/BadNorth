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

    private bool _attached;
    private bool _occupied;

    public bool Attached {
        get => _attached;
    }

    public bool Occupied {
        get => _occupied;
        set { _occupied = value; }
    }


    internal LadderUnit() : base(HEALTH)
    {

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
        ladder.transform.localPosition = new Vector3(0, 0.6f, 0);
        ladder.transform.forward = Vector3.up;
        ladder.transform.localEulerAngles = new Vector3(ladder.transform.localEulerAngles.x, 0, 0);
    }

    public void AttachToWall (Vector3 pos, Vector3 normal) {
        attachPos = pos;
        attachNormal = normal;
        landingPos = Game.GetTopFromSide(pos, normal);
        attaching = true;
    }

    public Vector3 GetEdgePos () {
    	return landingPos + (transform.forward * -0.35f);
    }

    public Vector3 GetBottomPos () {
        return bottomPos;
    }

    private void FinishAttach () {
        Group.TeleportTo(_destination);
        Group.SetAgentEnabled(false);
        transform.forward = -attachNormal;
        ladder.transform.localPosition = new Vector3(0, 0.5f, 0.5f);
        ladder.transform.forward = -attachNormal;
        bottomPos = transform.position + (attachNormal * -0.3f);

        attaching = false;
        _attached = true;
    }
}













