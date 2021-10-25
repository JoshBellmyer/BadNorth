using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHelper : MonoBehaviour
{
    /*
     * Source
     * Destination
     * Damage
     * DamageType
     * Knockback
     * -> Direction
     */
    enum DamageType
    {
        Unblockable = 0,
        Impact = 101,
        Fire = 102,
        Blunt = 201,
        Piercing = 202,
        Slashing = 203,
        HeavySlashing = 204
    }
}
