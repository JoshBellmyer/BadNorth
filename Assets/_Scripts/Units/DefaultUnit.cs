using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultUnit : Unit
{

    public static int HEALTH
    {
        get => 100;
    }

    internal DefaultUnit() : base(HEALTH)
    {
        
    }
    protected override bool FindAttack()
    {
        return false;
    }

}
