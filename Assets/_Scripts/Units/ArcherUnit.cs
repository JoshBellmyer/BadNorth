using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

public class ArcherUnit : Unit
{
    public static readonly float MAX_SIGHT_RANGE = 8.0F;
    public static readonly float LAUNCH_SPEED = 20F;
    public static readonly int HEALTH = 100;

    public GameObject[] arrowPrefabs;
    private int teamIndex;
    private bool currentUseB = false;

    protected override void UnitStart () {
        teamIndex = int.Parse(Team) - 1;
    }

    protected override bool FindAttack () {
        if (_currentCooldown > 0) {
            _currentCooldown -= Time.deltaTime;

            if (_currentCooldown <= 0) {
                _currentCooldown = 0;
            }
            else {
                return false;
            }
        }

        IOrderedEnumerable<Unit> t = GetOrderedEnemiesWithin(MAX_SIGHT_RANGE);

        foreach (Unit u in t) {
            Vector3 displacement = u.transform.position - this.transform.position;
            Vector3? a = ArrowCalc(displacement, false);
            Vector3? b = ArrowCalc(displacement, true);

            if (a == null && b == null) {
                continue;
            }

            Vector3 launchVector;
            bool useB = false;

            if (a == null) {
                launchVector = (Vector3)b;
                useB = true;
            } 
            else {
                launchVector = (Vector3)a;
                useB = false;
            }

            LaunchArrow(launchVector, useB);
            _currentCooldown = _attackCooldown;

            // TODO: handle arrow collisions

            return true;
        }

        return false;
    }

    protected override void AttackUpdate () {
        _directive = Directive.NONE;
    }

    public void UseVector (bool use) {
        currentUseB = use;
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

    private void LaunchArrow (Vector3 launchVector, bool useB) {
        // if (Game.online) {
        //     Game.GetLocalPlayer().LaunchArrowServerRpc(GetComponent<NetworkObject>(), int.Parse(Team), launchVector, useB);

        //     return;
        // }

        teamIndex = int.Parse(Team) - 1;
        GameObject arrow = Instantiate<GameObject>(arrowPrefabs[teamIndex]);
        
        if (Game.online) {
            arrow.GetComponent<NetworkObject>().Spawn();
        }
        else {
            Game.ClearNetworking(arrow);
        }

        Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();

        arrow.GetComponent<Arrow>().Setup(Team, this, useB);
        arrow.transform.position = transform.position + (Vector3.up * 0.5f);
        arrow.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        arrowRb.velocity = launchVector;

        SoundPlayer.PlaySound(Sound.ArrowFling, 1, true);
    }
}











