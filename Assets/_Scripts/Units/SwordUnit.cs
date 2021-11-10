using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordUnit : Unit
{

    public static readonly float MAX_MOVEMENT = 10.0f;
    public static readonly int HEALTH = 100;

    internal SwordUnit() : base(HEALTH)
    {

    }

    protected override bool FindAttack()
    {
        HashSet<Unit> units = TeamManager.instance.GetNotOnTeam(Team);

        float minDist = MAX_MOVEMENT;
        Unit target = null;

        foreach (Unit u in units) {
        	float dist = Vector3.Distance(transform.position, u.transform.position);

        	if (dist <= minDist) {
        		minDist = dist;
        		target = u;
        	}
        }

        if (target == null) {
        	return false;
        }

        return true;
    }
}
