using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderUnit : Unit
{
    public static readonly int HEALTH = 100;

    internal LadderUnit() : base(HEALTH)
    {

    }

    protected override bool FindAttack()
    {
        return false;
    }

    public Vector3 GetEdgePos () {
    	return Vector3.zero;
    }
}
