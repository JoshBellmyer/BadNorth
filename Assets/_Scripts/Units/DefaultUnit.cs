using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultUnit : Unit
{
    protected override bool FindAttack()
    {
        return false;
    }
}
