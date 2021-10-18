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
        throw new System.NotImplementedException();
    }
}
