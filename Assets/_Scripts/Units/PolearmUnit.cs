using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolearmUnit : Unit
{
    public static readonly int HEALTH = 100;

    internal PolearmUnit() : base(HEALTH)
    {

    }
    protected override bool FindAttack()
    {
        throw new System.NotImplementedException();
    }
}
