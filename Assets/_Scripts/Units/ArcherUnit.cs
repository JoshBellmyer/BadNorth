using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArcherUnit : Unit
{
    public static readonly float MAX_SIGHT_RANGE = 10.0F;
    public static readonly float LAUNCH_SPEED = 5.0F;
    public static readonly int HEALTH = 100;

    internal ArcherUnit() : base(HEALTH)
    {

    }

    protected override bool FindAttack()
    {
        // throw new System.NotImplementedException();

        IOrderedEnumerable<Unit> t = GetOrderedEnemiesWithin(MAX_SIGHT_RANGE);
        foreach (Unit u in t)
        {
            Vector3 displacement = u.transform.position - this.transform.position;
            Vector3? a = ArrowCalc(displacement, false);
            Vector3? b = ArrowCalc(displacement, true);
            if (a != null || b != null)
            {
                Vector3 launchVector;
                if (a == null)
                {
                    launchVector = (Vector3)a;
                } else if (b == null)
                {
                    launchVector = (Vector3)b;
                } else
                {
                    launchVector = (Vector3)(Vector3.Dot(displacement, (Vector3)a) > Vector3.Dot(displacement, (Vector3)b) ? a : b);

                }
                // TODO: Insert code here for actually launching based on launchVector.
                //                                 TODO: Collisions
                break;
            }
        }

        return false;
    }

    private Vector3? ArrowCalc(Vector3 d, bool secondary)
    {
        Vector3 a = Physics.gravity;
        float r = LAUNCH_SPEED;
        double bigCalcOne = Math.Sqrt(Math.Pow(Vector3.Dot(a,d) + r * r, 2) - a.sqrMagnitude * d.sqrMagnitude);
        if (Double.IsNaN(bigCalcOne)) return null;
        double bigCalcTwo = Math.Sqrt(2 * (r * r + Vector3.Dot(a,d) + (secondary ? -bigCalcOne : bigCalcOne)));
        if (Double.IsNaN(bigCalcTwo)) return null;
        return (d * (a.magnitude / (float)bigCalcTwo)) - (a * ((float)bigCalcTwo / (2 * a.magnitude)));
    }
}
