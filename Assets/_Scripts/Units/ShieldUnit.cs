using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldUnit : Unit
{
    public static readonly int HEALTH = 100;
    internal ShieldUnit() : base(HEALTH)
    {

    }

    protected override bool FindAttack()
    {
        HashSet<Unit> units = TeamManager.instance.GetNotOnTeam(Team);

        float minDist = 10;
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

        _targetEnemy = target;
        IssueTemporaryDestination(target.transform.position);

        return true;
    }
}
